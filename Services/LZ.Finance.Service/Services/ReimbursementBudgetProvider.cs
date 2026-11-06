using System.Text.Json;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums.Budget;
using LZ.Model.Enums.Reimbusement;
using Microsoft.Extensions.Logging;
using SqlSugar;
using SqlSugar.IOC;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementBudgetProvider : ServicesBase<FIN_Reimb_BudgetReservation>,
    IReimbursementBudgetProvider, IScopedDependency
{
    private const string Source = "Live_BudgetManage";
    private const string UpdateLock = "WITH (UPDLOCK, HOLDLOCK)";
    private readonly IBudgetPoolLocator _locator;
    private readonly ILogger<ReimbursementBudgetProvider> _logger;
    private readonly ISqlSugarClient _main;

    public ReimbursementBudgetProvider(IBudgetPoolLocator locator, ILogger<ReimbursementBudgetProvider> logger)
    {
        _locator = locator;
        _logger = logger;
        _main = DbScoped.SugarScope.GetConnection("Main");
    }

    public async Task<BudgetDecision> CheckAndReserveAsync(int companyId, int userId, string externalRequestNo,
        int costTypeId, string expenseName, decimal amount, CancellationToken cancellationToken)
    {
        if (companyId <= 0 || userId <= 0 || costTypeId <= 0 || amount <= 0 ||
            string.IsNullOrWhiteSpace(externalRequestNo) || string.IsNullOrWhiteSpace(expenseName))
            throw new ArgumentException("预算预留参数无效");
        cancellationToken.ThrowIfCancellationRequested();
        var selected = await _locator.LocateAsync(_main, companyId, userId, costTypeId,
            expenseName, DateTime.Now, cancellationToken);
        return await InBudgetTransactionAsync(externalRequestNo, async () =>
        {
            if (Guid.TryParseExact(externalRequestNo, "N", out var applyGuid))
            {
                var previous = await Context.Queryable<FIN_Reimb_BudgetReservation>()
                    .Where(x => x.CompanyId == companyId && x.ApplyGuid == applyGuid &&
                                x.ExternalRequestNo != externalRequestNo).FirstAsync();
                if (previous is not null)
                    throw new InvalidOperationException("该申请存在旧版报销预算流水，请先核对并迁移");
            }
            var reservation = await LockedReservationAsync(externalRequestNo);
            if (reservation is not null && reservation.CompanyId != companyId)
                throw new InvalidOperationException("预算请求号已由其他企业使用");
            var ledger = await FindLatestOperationAsync(companyId, externalRequestNo);
            var financeVersion = ReadVersion(reservation);
            var version = Math.Max(financeVersion, ledger?.Version ?? 0);
            var pending = ledger is { Operation: BudgetOperation.Reserve } &&
                          (reservation is null || ledger.Version > financeVersion);
            if (ledger is { Operation: BudgetOperation.Commit } &&
                reservation?.ReservationStatus != (byte)BudgetReservationStatus.Committed)
                throw new InvalidOperationException("老预算已核销但报销流水不存在，请核对后处理");
            if (pending && (ledger!.PoolId != selected.ID || ledger.Amount != amount))
                throw new InvalidOperationException("该请求存在尚未恢复的老预算预留，请使用原申请参数重试");
            if (reservation?.ReservationStatus == (byte)BudgetReservationStatus.Committed)
                throw new InvalidOperationException("预算已确认，不能重新预留");
            if (reservation?.ReservationStatus == (byte)BudgetReservationStatus.Reserved &&
                reservation.ReservedAmount == amount && reservation.ExternalBudgetId == selected.ID.ToString() &&
                !await OperationExistsAsync(selected.ID, companyId,
                    OperationKey(externalRequestNo, version, BudgetOperation.Release)))
                return Decision(reservation, false);

            if (reservation?.ReservationStatus == (byte)BudgetReservationStatus.Reserved)
            {
                var former = await LockedPoolAsync(ParsePoolId(reservation), companyId);
                await ApplyOperationAsync(former, reservation.ReservedAmount, externalRequestNo,
                    version, BudgetOperation.Release);
            }
            var pool = await LockedPoolAsync(selected.ID, companyId);
            if (pool.Hidden || pool.PartnerGID != selected.PartnerGID || pool.Year != selected.Year ||
                pool.CostType != costTypeId || pool.ExpensesTitle != expenseName)
                throw new InvalidOperationException("老预算池配置已变化，请重试");
            var nextVersion = pending
                ? version : version + 1;
            var priorReserve = await FindOperationAsync(pool.ID, companyId,
                OperationKey(externalRequestNo, nextVersion, BudgetOperation.Reserve));
            if (priorReserve is not null && priorReserve.Money != amount)
                throw new InvalidOperationException("老预算操作金额与幂等流水不一致");
            var availableBefore = priorReserve is null
                ? pool.AvailableAmount : priorReserve.AvailableAmount + amount;
            var reserved = priorReserve is not null || (!pool.DirectlyPass && pool.AvailableAmount >= amount);
            var withinBudget = pool.DirectlyPass || reserved;
            if (reserved && priorReserve is null)
                await ApplyOperationAsync(pool, amount, externalRequestNo, nextVersion, BudgetOperation.Reserve);
            var snapshot = JsonSerializer.Serialize(new
            {
                source = Source,
                version = nextVersion,
                poolId = pool.ID,
                expenseName,
                availableBefore,
                amount,
                withinBudget,
                reserved,
                directlyPass = pool.DirectlyPass
            });
            if (reservation is null)
            {
                reservation = new FIN_Reimb_BudgetReservation
                {
                    ReservationGuid = Guid.NewGuid(),
                    CompanyId = companyId,
                    ApplyGuid = Guid.Empty,
                    ExternalRequestNo = externalRequestNo,
                    CostTypeName = pool.ExpensesTitle,
                    ExternalBudgetId = pool.ID.ToString(),
                    CheckResult = withinBudget ? (byte)1 : (byte)2,
                    ReservationStatus = reserved ? (byte)BudgetReservationStatus.Reserved : (byte)BudgetReservationStatus.None,
                    AvailableBefore = availableBefore,
                    ReservedAmount = reserved ? amount : 0,
                    ResponseSnapshot = snapshot,
                    CreatedAt = DateTime.Now
                };
                await Context.Insertable(reservation).IgnoreColumns(x => new { x.Id, x.RowVersion })
                    .ExecuteCommandAsync();
            }
            else
            {
                var changed = await Context.Updateable<FIN_Reimb_BudgetReservation>()
                    .SetColumns(x => new FIN_Reimb_BudgetReservation
                    {
                        CostTypeName = pool.ExpensesTitle,
                        ExternalBudgetId = pool.ID.ToString(),
                        CheckResult = withinBudget ? (byte)1 : (byte)2,
                        ReservationStatus = reserved ? (byte)BudgetReservationStatus.Reserved : (byte)BudgetReservationStatus.None,
                        AvailableBefore = availableBefore,
                        ReservedAmount = reserved ? amount : 0,
                        ResponseSnapshot = snapshot,
                        UpdatedAt = DateTime.Now
                    })
                    .Where(x => x.ReservationGuid == reservation.ReservationGuid && x.CompanyId == companyId)
                    .ExecuteCommandAsync();
                if (changed != 1) throw new InvalidOperationException("预算预留流水已变化，请重试");
            }
            return new BudgetDecision
            {
                WithinBudget = withinBudget,
                Reserved = reserved,
                DirectlyPass = pool.DirectlyPass && !reserved,
                ExternalBudgetId = pool.ID.ToString(),
                AvailableBefore = availableBefore,
                ResponseSnapshot = snapshot,
                ReservationChanged = reserved && priorReserve is null
            };
        });
    }

    public Task CommitAsync(string externalRequestNo, CancellationToken cancellationToken) =>
        InBudgetTransactionAsync(externalRequestNo, async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var reservation = await LockedReservationAsync(externalRequestNo)
                ?? throw new InvalidOperationException("预算预留不存在");
            var version = ReadVersion(reservation);
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Committed) return true;
            if (reservation.ReservationStatus != (byte)BudgetReservationStatus.Reserved ||
                reservation.ApplyGuid == Guid.Empty)
                throw new InvalidOperationException("预算尚未预留或已释放");
            var pool = await LockedPoolAsync(ParsePoolId(reservation), reservation.CompanyId);
            await ApplyOperationAsync(pool, reservation.ReservedAmount, externalRequestNo,
                version, BudgetOperation.Commit);
            var changed = await Context.Updateable<FIN_Reimb_BudgetReservation>()
                .SetColumns(x => new FIN_Reimb_BudgetReservation
                { ReservationStatus = (byte)BudgetReservationStatus.Committed, UpdatedAt = DateTime.Now })
                .Where(x => x.ReservationGuid == reservation.ReservationGuid &&
                            x.ReservationStatus == (byte)BudgetReservationStatus.Reserved).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("预算预留状态已变化");
            return true;
        });

    public Task ReleaseAsync(string externalRequestNo, CancellationToken cancellationToken) =>
        InBudgetTransactionAsync(externalRequestNo, async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var reservation = await LockedReservationAsync(externalRequestNo);
            var ledger = await FindLatestOperationAsync(null, externalRequestNo);
            if (reservation is null)
            {
                if (ledger is { Operation: BudgetOperation.Reserve })
                {
                    if (!Guid.TryParseExact(externalRequestNo, "N", out var applyGuid))
                        throw new InvalidOperationException("预算请求号无效，无法核对报销申请");
                    var applyExists = await Context.Queryable<FIN_Reimb_Apply>()
                        .AnyAsync(x => x.CompanyId == ledger.CompanyId && x.ApplyGuid == applyGuid);
                    if (applyExists)
                        throw new InvalidOperationException("报销申请已提交，不能释放其预算预留");
                    var pool = await LockedPoolAsync(ledger.PoolId, ledger.CompanyId);
                    await ApplyOperationAsync(pool, ledger.Amount, externalRequestNo,
                        ledger.Version, BudgetOperation.Release);
                }
                return true;
            }
            var version = ReadVersion(reservation);
            if (ledger is { Operation: BudgetOperation.Reserve } && ledger.Version > version)
            {
                var pendingPool = await LockedPoolAsync(ledger.PoolId, reservation.CompanyId);
                await ApplyOperationAsync(pendingPool, ledger.Amount, externalRequestNo,
                    ledger.Version, BudgetOperation.Release);
            }
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Released)
                return true;
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Committed)
                throw new InvalidOperationException("已确认预算不能释放");
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Reserved)
            {
                var pool = await LockedPoolAsync(ParsePoolId(reservation), reservation.CompanyId);
                await ApplyOperationAsync(pool, reservation.ReservedAmount, externalRequestNo,
                    version, BudgetOperation.Release);
            }
            var changed = await Context.Updateable<FIN_Reimb_BudgetReservation>()
                .SetColumns(x => new FIN_Reimb_BudgetReservation
                { ReservationStatus = (byte)BudgetReservationStatus.Released, UpdatedAt = DateTime.Now })
                .Where(x => x.ReservationGuid == reservation.ReservationGuid &&
                            x.ReservationStatus == reservation.ReservationStatus).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("预算预留状态已变化");
            return true;
        });

    private async Task<T> InBudgetTransactionAsync<T>(string requestNo, Func<Task<T>> action)
    {
        await Context.Ado.BeginTranAsync();
        try { await _main.Ado.BeginTranAsync(); }
        catch
        {
            await Context.Ado.RollbackTranAsync();
            throw;
        }
        var mainCommitted = false;
        try
        {
            var result = await action();
            await _main.Ado.CommitTranAsync();
            mainCommitted = true;
            await Context.Ado.CommitTranAsync();
            return result;
        }
        catch (Exception ex)
        {
            if (!mainCommitted)
            {
                try { await _main.Ado.RollbackTranAsync(); }
                catch (Exception rollback) { _logger.LogError(rollback, "老预算事务回滚失败"); }
            }
            try { await Context.Ado.RollbackTranAsync(); }
            catch (Exception rollback) { _logger.LogError(rollback, "报销预算事务回滚失败"); }
            if (mainCommitted)
                _logger.LogCritical(ex, "老预算已提交但报销预算流水未确认，请按请求号核对：{RequestNo}", requestNo);
            throw;
        }
    }

    private Task<FIN_Reimb_BudgetReservation?> LockedReservationAsync(string requestNo) =>
        Context.Queryable<FIN_Reimb_BudgetReservation>().With(UpdateLock)
            .Where(x => x.ExternalRequestNo == requestNo).FirstAsync();

    private async Task<Live_BudgetManage> LockedPoolAsync(int poolId, int companyId) =>
        await _main.Queryable<Live_BudgetManage>().With(UpdateLock)
            .FirstAsync(x => x.ID == poolId && x.CompanyID == companyId)
        ?? throw new InvalidOperationException("老预算池不存在或不属于当前企业");

    private Task<Live_BudgetRechargeLog?> FindOperationAsync(int poolId, int companyId, string key) =>
        _main.Queryable<Live_BudgetRechargeLog>()
            .FirstAsync(x => x.CompanyID == companyId && x.CID == poolId && x.Content == key);

    private async Task<bool> OperationExistsAsync(int poolId, int companyId, string key) =>
        await FindOperationAsync(poolId, companyId, key) is not null;

    private async Task<BudgetOperationLog?> FindLatestOperationAsync(int? companyId, string requestNo)
    {
        var prefix = $"REIMB:{requestNo}:";
        var logs = await _main.Queryable<Live_BudgetRechargeLog>()
            .Where(x => x.Content.StartsWith(prefix))
            .WhereIF(companyId.HasValue, x => x.CompanyID == companyId)
            .ToListAsync();
        BudgetOperationLog? latest = null;
        foreach (var log in logs)
        {
            var suffix = log.Content[prefix.Length..].Split(':');
            if (suffix.Length != 2 || !int.TryParse(suffix[0], out var version) || version <= 0 ||
                !byte.TryParse(suffix[1], out var operation) ||
                !Enum.IsDefined(typeof(BudgetOperation), operation) ||
                log.CompanyID is not int owner || log.CID is not int poolId)
                throw new InvalidOperationException("老预算幂等日志格式无效，请核对后处理");
            if (latest is null || version > latest.Version ||
                version == latest.Version && operation > (byte)latest.Operation)
                latest = new BudgetOperationLog(owner, poolId, version,
                    (BudgetOperation)operation, log.Money);
        }
        return latest;
    }

    private async Task ApplyOperationAsync(Live_BudgetManage pool, decimal amount, string requestNo,
        int version, BudgetOperation operation)
    {
        var key = OperationKey(requestNo, version, operation);
        var previous = await FindOperationAsync(pool.ID, pool.CompanyID!.Value, key);
        if (previous is not null)
        {
            if (previous.Money != amount)
                throw new InvalidOperationException("老预算操作金额与幂等流水不一致");
            return;
        }
        var changed = operation switch
        {
            BudgetOperation.Reserve => await _main.Updateable<Live_BudgetManage>()
                .SetColumns(x => x.AvailableAmount == x.AvailableAmount - amount)
                .SetColumns(x => x.LockAmount == x.LockAmount + amount)
                .Where(x => x.ID == pool.ID && x.CompanyID == pool.CompanyID && x.AvailableAmount >= amount)
                .ExecuteCommandAsync(),
            BudgetOperation.Commit => await _main.Updateable<Live_BudgetManage>()
                .SetColumns(x => x.LockAmount == x.LockAmount - amount)
                .SetColumns(x => x.UsedAmount == x.UsedAmount + amount)
                .Where(x => x.ID == pool.ID && x.CompanyID == pool.CompanyID && x.LockAmount >= amount)
                .ExecuteCommandAsync(),
            BudgetOperation.Release => await _main.Updateable<Live_BudgetManage>()
                .SetColumns(x => x.LockAmount == x.LockAmount - amount)
                .SetColumns(x => x.AvailableAmount == x.AvailableAmount + amount)
                .Where(x => x.ID == pool.ID && x.CompanyID == pool.CompanyID && x.LockAmount >= amount)
                .ExecuteCommandAsync(),
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
        if (changed != 1) throw new InvalidOperationException("老预算余额已变化，请核对后重试");
        switch (operation)
        {
            case BudgetOperation.Reserve:
                pool.AvailableAmount -= amount;
                pool.LockAmount += amount;
                break;
            case BudgetOperation.Commit:
                pool.LockAmount -= amount;
                pool.UsedAmount += amount;
                break;
            case BudgetOperation.Release:
                pool.LockAmount -= amount;
                pool.AvailableAmount += amount;
                break;
        }
        var partner = pool.PartnerGID is Guid partnerGid
            ? await _main.Queryable<Live_Partner>().FirstAsync(x => x.CompanyID == pool.CompanyID &&
                                                         x.PartnerGID == partnerGid)
            : null;
        await _main.Insertable(new Live_BudgetRechargeLog
        {
            CompanyID = pool.CompanyID,
            CompanyGID = pool.CompanyGID,
            CID = pool.ID,
            Year = pool.Year,
            UserName = "报销系统",
            Content = key,
            CreateTime = DateTime.Now,
            Money = amount,
            PartnerName = partner?.PartnerName,
            CostName = pool.CostType is int costType ? ((CostType)costType).ToString() : null,
            Title = pool.ExpensesTitle,
            GenerateName = pool.GenerateType is int generateType ? ((GenerationType)generateType).ToString() : null,
            Type = operation switch
            {
                BudgetOperation.Reserve => (int)BudgetManageRecordActionEnum.客服申请扣款,
                BudgetOperation.Commit => (int)BudgetManageRecordActionEnum.已打款,
                _ => (int)BudgetManageRecordActionEnum.作废
            },
            TotalAmount = pool.TotalAmount,
            LockAmount = pool.LockAmount,
            UsedAmount = pool.UsedAmount,
            AvailableAmount = pool.AvailableAmount,
            HeadOfficeUsedAmount = pool.HeadOfficeUsedAmount,
            DecreaseAmount = operation == BudgetOperation.Commit ? amount : 0
        }).IgnoreColumns(x => x.ID).ExecuteCommandAsync();
    }

    private static int ReadVersion(FIN_Reimb_BudgetReservation? reservation)
    {
        if (reservation is null) return 0;
        try
        {
            using var snapshot = JsonDocument.Parse(reservation.ResponseSnapshot);
            if (snapshot.RootElement.TryGetProperty("source", out var source) &&
                source.GetString() == Source &&
                snapshot.RootElement.TryGetProperty("version", out var version) &&
                version.TryGetInt32(out var number) && number > 0)
                return number;
        }
        catch (JsonException) { }
        throw new InvalidOperationException("该申请存在旧版报销预算流水，请先核对并迁移");
    }

    private static BudgetDecision Decision(FIN_Reimb_BudgetReservation reservation, bool changed) => new()
    {
        WithinBudget = reservation.CheckResult == 1,
        Reserved = reservation.ReservationStatus == (byte)BudgetReservationStatus.Reserved,
        DirectlyPass = false,
        ReservationChanged = changed,
        ExternalBudgetId = reservation.ExternalBudgetId,
        AvailableBefore = reservation.AvailableBefore,
        ResponseSnapshot = reservation.ResponseSnapshot
    };

    private static string OperationKey(string requestNo, int version, BudgetOperation operation) =>
        $"REIMB:{requestNo}:{version}:{(byte)operation}";

    private static int ParsePoolId(FIN_Reimb_BudgetReservation reservation) =>
        int.TryParse(reservation.ExternalBudgetId, out var id) && id > 0 ? id :
            throw new InvalidOperationException("老预算池标识无效");

    private enum BudgetOperation : byte { Reserve = 1, Commit = 2, Release = 3 }

    private sealed record BudgetOperationLog(int CompanyId, int PoolId, int Version,
        BudgetOperation Operation, decimal Amount);
}
