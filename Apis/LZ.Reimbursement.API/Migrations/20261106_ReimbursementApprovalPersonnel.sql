-- Run once per company before deploying the Live_Personnel-only reimbursement approval code.
-- Main and Finance must be on the same SQL Server instance. Adjust database names if needed.
-- Supply a verified, explicit old approval UserID -> Live_Personnel.UserID mapping.
-- Neither matching integer IDs nor names/phone numbers prove that two accounts belong to one person.
-- Stop approval writes while applying this migration; back up both databases first.

USE [LZ_Finance];
GO
SET XACT_ABORT ON;

DECLARE @CompanyId int = 0; -- Set the company being migrated.
DECLARE @UserMap TABLE (OldUserId int PRIMARY KEY, NewUserId int NOT NULL);
-- INSERT INTO @UserMap (OldUserId, NewUserId) VALUES (123, 456);

IF @CompanyId <= 0 OR NOT EXISTS (SELECT 1 FROM @UserMap)
    THROW 51000, 'Set CompanyId and an explicit approval user mapping before running.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @LockResult int;
    EXEC @LockResult = sys.sp_getapplock
        @Resource = N'FIN_Reimb_ApprovalPersonnelMigration',
        @LockMode = 'Exclusive',
        @LockOwner = 'Transaction';
    IF @LockResult < 0
        THROW 51006, 'Could not lock reimbursement approval migration.', 1;

    IF OBJECT_ID(N'dbo.FIN_Reimb_ApprovalPersonnelMigration', N'U') IS NULL
        CREATE TABLE dbo.FIN_Reimb_ApprovalPersonnelMigration
        (
            CompanyId int NOT NULL PRIMARY KEY,
            MigratedAt datetime2 NOT NULL DEFAULT SYSUTCDATETIME()
        );

    IF EXISTS (SELECT 1 FROM dbo.FIN_Reimb_ApprovalPersonnelMigration WHERE CompanyId = @CompanyId)
        THROW 51001, 'Approval identities for this company have already been migrated.', 1;

    IF EXISTS
    (
        SELECT 1 FROM @UserMap AS m
        LEFT JOIN [LZ_Saas].dbo.Live_Personnel AS p
            ON p.CompanyID = @CompanyId AND p.UserID = m.NewUserId
        LEFT JOIN [LZ_Saas].dbo.Live_Partner AS d
            ON d.CompanyID = @CompanyId AND d.PartnerGID = p.PartnerGID
        WHERE p.UserID IS NULL OR p.PartnerGID IS NULL OR d.PartnerGID IS NULL
    )
        THROW 51002, 'Mapped personnel or their department is missing from this company.', 1;

    IF EXISTS
    (
        SELECT 1 FROM dbo.FIN_Reimb_ApprovalNode AS n
        LEFT JOIN @UserMap AS m ON m.OldUserId = n.ApproverUserId
        WHERE n.CompanyId = @CompanyId AND m.OldUserId IS NULL
    ) OR EXISTS
    (
        SELECT 1 FROM dbo.FIN_Reimb_ApprovalProcess AS p
        LEFT JOIN @UserMap AS m ON m.OldUserId = p.CreatedBy
        WHERE p.CompanyId = @CompanyId AND m.OldUserId IS NULL
    ) OR EXISTS
    (
        SELECT 1 FROM dbo.FIN_Reimb_ApprovalRecord AS r
        LEFT JOIN dbo.FIN_Reimb_ApprovalNode AS n
            ON n.CompanyId = r.CompanyId AND n.NodeGuid = r.NodeGuid
        LEFT JOIN @UserMap AS m ON m.OldUserId = n.ApproverUserId
        WHERE r.CompanyId = @CompanyId AND m.OldUserId IS NULL
    )
        THROW 51003, 'Mapping does not cover every historical approver, operator and process creator.', 1;

    IF EXISTS
    (
        SELECT 1 FROM dbo.FIN_Reimb_ApprovalNode AS n
        JOIN dbo.FIN_Reimb_ApprovalProcess AS process
            ON process.CompanyId = n.CompanyId AND process.ProcessGuid = n.ProcessGuid
        JOIN @UserMap AS m ON m.OldUserId = n.ApproverUserId
        JOIN [LZ_Saas].dbo.Live_Personnel AS p
            ON p.CompanyID = @CompanyId AND p.UserID = m.NewUserId
        JOIN [LZ_Saas].dbo.Live_Partner AS d
            ON d.CompanyID = @CompanyId AND d.PartnerGID = p.PartnerGID
        WHERE n.CompanyId = @CompanyId AND process.IsActive = 1 AND n.IsEnabled = 1
            AND p.Enabled = 0
    )
        THROW 51004, 'An active approval node maps to disabled personnel.', 1;

    IF EXISTS
    (
        SELECT 1 FROM dbo.FIN_Reimb_ApprovalRecord AS r
        LEFT JOIN dbo.FIN_Reimb_ApprovalNode AS n
            ON n.CompanyId = r.CompanyId AND n.NodeGuid = r.NodeGuid
        LEFT JOIN @UserMap AS m ON m.OldUserId = n.ApproverUserId
        WHERE r.CompanyId = @CompanyId
            AND (n.NodeGuid IS NULL OR m.OldUserId IS NULL OR
                 r.OperatorId NOT IN (n.ApproverUserId, m.NewUserId))
    )
        THROW 51005, 'An approval record is not owned by its node; reconcile it before migration.', 1;

    UPDATE r
    SET r.OperatorId = m.NewUserId
    FROM dbo.FIN_Reimb_ApprovalRecord AS r
    JOIN dbo.FIN_Reimb_ApprovalNode AS n
        ON n.CompanyId = r.CompanyId AND n.NodeGuid = r.NodeGuid
    JOIN @UserMap AS m ON m.OldUserId = n.ApproverUserId
    WHERE r.CompanyId = @CompanyId AND r.OperatorId = n.ApproverUserId;

    UPDATE n
    SET n.ApproverUserId = m.NewUserId,
        n.ApproverUserName = p.FullName,
        n.ApproverDeptId = d.PartnerGID,
        n.ApproverDeptName = d.PartnerName
    FROM dbo.FIN_Reimb_ApprovalNode AS n
    JOIN @UserMap AS m ON m.OldUserId = n.ApproverUserId
    JOIN [LZ_Saas].dbo.Live_Personnel AS p
        ON p.CompanyID = @CompanyId AND p.UserID = m.NewUserId
    JOIN [LZ_Saas].dbo.Live_Partner AS d
        ON d.CompanyID = @CompanyId AND d.PartnerGID = p.PartnerGID
    WHERE n.CompanyId = @CompanyId;

    UPDATE process
    SET process.CreatedBy = m.NewUserId
    FROM dbo.FIN_Reimb_ApprovalProcess AS process
    JOIN @UserMap AS m ON m.OldUserId = process.CreatedBy
    WHERE process.CompanyId = @CompanyId;

    INSERT INTO dbo.FIN_Reimb_ApprovalPersonnelMigration (CompanyId) VALUES (@CompanyId);
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
