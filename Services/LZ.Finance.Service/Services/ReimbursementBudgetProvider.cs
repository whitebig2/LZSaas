using System.Text.Json;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Enums.Reimbusement;
using SqlSugar;

namespace LZ.Finance.Service.Services;

public sealed class ReimbursementBudgetProvider : ServicesBase<FIN_Reimb_BudgetReservation>,
    IReimbursementBudgetProvider, IScopedDependency
{
    private readonly IBudgetPoolLocator _locator;
    private readonly string _updHoldLock = "WITH (UPDLOCK, ROWLOCK)";

    public ReimbursementBudgetProvider(IBudgetPoolLocator locator) => _locator = locator;

    public async Task<BudgetDecision> CheckAndReserveAsync(int companyId, int userId, string applyNo,
        int costTypeId, decimal amount, CancellationToken cancellationToken)
    {
        if (companyId <= 0 || userId <= 0 || costTypeId <= 0 || amount <= 0 || string.IsNullOrWhiteSpace(applyNo))
            throw new ArgumentException("预算预留参数无效");
        cancellationToken.ThrowIfCancellationRequested();
        var pool = await _locator.LocateAsync(Context, companyId, userId, costTypeId, DateTime.Now, cancellationToken);
        await Context.Ado.BeginTranAsync();
        try
        {
            var reservation = await LockedReservationAsync(companyId, applyNo);
            if (reservation?.ReservationStatus == (byte)BudgetReservationStatus.Committed)
                throw new InvalidOperationException("预算已确认，不能重新预留");
            if (reservation?.ReservationStatus == (byte)BudgetReservationStatus.Reserved &&
                reservation.ReservedAmount == amount && reservation.ExternalBudgetId == pool.ID.ToString())
            {
                await Context.Ado.CommitTranAsync();
                return new BudgetDecision
                {
                    WithinBudget = true,
                    Reserved = true,
                    ExternalBudgetId = reservation.ExternalBudgetId,
                    AvailableBefore = reservation.AvailableBefore,
                    ResponseSnapshot = reservation.ResponseSnapshot,
                    ReservationChanged = false
                };
            }
            if (reservation?.ReservationStatus == (byte)BudgetReservationStatus.Reserved)
                await ReleasePoolAsync(reservation);

            // The update predicate is evaluated by SQL Server against current available money.
            var availableBefore = pool.AvailableAmount;
            var withinBudget = pool.DirectlyPass || availableBefore >= amount;
            var reserved = withinBudget && !pool.DirectlyPass;
            if (reserved)
            {
                var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
                    .SetColumns(x => x.LockAmount == x.LockAmount + amount)
                    .SetColumns(x => x.AvailableAmount == x.AvailableAmount - amount)
                    .Where(x => x.ID == pool.ID && x.CompanyID == companyId && x.AvailableAmount >= amount)
                    .ExecuteCommandAsync();
                if (changed != 1) throw new InvalidOperationException("预算额度已变化，请重试");
            }
            var snapshot = JsonSerializer.Serialize(new
            {
                poolId = pool.ID,
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
                    ExternalRequestNo = applyNo,
                    CreatedAt = DateTime.Now
                };
                reservation.CostTypeName = pool.ExpensesTitle;
                reservation.ExternalBudgetId = pool.ID.ToString();
                reservation.CheckResult = withinBudget ? (byte)1 : (byte)2;
                reservation.ReservationStatus = reserved ? (byte)BudgetReservationStatus.Reserved : (byte)BudgetReservationStatus.None;
                reservation.AvailableBefore = availableBefore;
                reservation.ReservedAmount = reserved ? amount : 0;
                reservation.ResponseSnapshot = snapshot;
                await Context.Insertable(reservation).IgnoreColumns(x => new { x.Id, x.RowVersion }).ExecuteCommandAsync();
            }
            else
            {
                await Context.Updateable<FIN_Reimb_BudgetReservation>()
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
            }
            await Context.Ado.CommitTranAsync();
            return new BudgetDecision
            {
                WithinBudget = withinBudget,
                Reserved = reserved,
                DirectlyPass = pool.DirectlyPass,
                ExternalBudgetId = pool.ID.ToString(),
                AvailableBefore = availableBefore,
                ResponseSnapshot = snapshot,
                ReservationChanged = reserved
            };
        }
        catch { await Context.Ado.RollbackTranAsync(); throw; }
    }

    public async Task CommitAsync(string externalRequestNo, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Context.Ado.BeginTranAsync();
        try
        {
            var reservation = await LockedReservationAsync(externalRequestNo);
            if (reservation is null || reservation.ReservationStatus == (byte)BudgetReservationStatus.Released)
                throw new InvalidOperationException("预算预留不存在或已释放");
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Committed)
            { await Context.Ado.CommitTranAsync(); return; }
            if (reservation.ReservationStatus != (byte)BudgetReservationStatus.Reserved || reservation.ApplyGuid == Guid.Empty)
                throw new InvalidOperationException("预算尚未预留");
            var poolId = ParsePoolId(reservation);
            var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
                .SetColumns(x => x.LockAmount == x.LockAmount - reservation.ReservedAmount)
                .SetColumns(x => x.UsedAmount == x.UsedAmount + reservation.ReservedAmount)
                .Where(x => x.ID == poolId && x.CompanyID == reservation.CompanyId &&
                    x.LockAmount >= reservation.ReservedAmount).ExecuteCommandAsync();
            if (changed != 1) throw new InvalidOperationException("预算池锁定金额不足");
            await Context.Updateable<FIN_Reimb_BudgetReservation>()
                .SetColumns(x => new FIN_Reimb_BudgetReservation
                { ReservationStatus = (byte)BudgetReservationStatus.Committed, UpdatedAt = DateTime.Now })
                .Where(x => x.ReservationGuid == reservation.ReservationGuid).ExecuteCommandAsync();
            await Context.Ado.CommitTranAsync();
        }
        catch { await Context.Ado.RollbackTranAsync(); throw; }
    }

    public async Task ReleaseAsync(string externalRequestNo, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Context.Ado.BeginTranAsync();
        try
        {
            var reservation = await LockedReservationAsync(externalRequestNo);
            if (reservation is null || reservation.ReservationStatus == (byte)BudgetReservationStatus.Released)
            { await Context.Ado.CommitTranAsync(); return; }
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Committed)
                throw new InvalidOperationException("已确认预算不能释放");
            if (reservation.ReservationStatus == (byte)BudgetReservationStatus.Reserved)
                await ReleasePoolAsync(reservation);
            await Context.Updateable<FIN_Reimb_BudgetReservation>()
                .SetColumns(x => new FIN_Reimb_BudgetReservation
                { ReservationStatus = (byte)BudgetReservationStatus.Released, UpdatedAt = DateTime.Now })
                .Where(x => x.ReservationGuid == reservation.ReservationGuid).ExecuteCommandAsync();
            await Context.Ado.CommitTranAsync();
        }
        catch { await Context.Ado.RollbackTranAsync(); throw; }
    }

    private async Task<FIN_Reimb_BudgetReservation?> LockedReservationAsync(int companyId, string applyNo) =>
        await Context.Queryable<FIN_Reimb_BudgetReservation>()
            .With(_updHoldLock)
            .Where(x => x.CompanyId == companyId && x.ExternalRequestNo == applyNo)
            .FirstAsync();

    private async Task<FIN_Reimb_BudgetReservation?> LockedReservationAsync(string applyNo) =>
        await Context.Queryable<FIN_Reimb_BudgetReservation>()
            .With(_updHoldLock)
            .Where(x => x.ExternalRequestNo == applyNo)
            .FirstAsync();

    private async Task ReleasePoolAsync(FIN_Reimb_BudgetReservation reservation)
    {
        var poolId = ParsePoolId(reservation);
        var changed = await Context.Updateable<FIN_Reimb_BudgetManage>()
            .SetColumns(x => x.LockAmount == x.LockAmount - reservation.ReservedAmount)
            .SetColumns(x => x.AvailableAmount == x.AvailableAmount + reservation.ReservedAmount)
            .Where(x => x.ID == poolId && x.CompanyID == reservation.CompanyId &&
                x.LockAmount >= reservation.ReservedAmount).ExecuteCommandAsync();
        if (changed != 1) throw new InvalidOperationException("预算池锁定金额不足，无法释放");
    }

    private static int ParsePoolId(FIN_Reimb_BudgetReservation reservation) =>
        int.TryParse(reservation.ExternalBudgetId, out var id) && id > 0 ? id :
            throw new InvalidOperationException("预算池标识无效");
}
