/* ============================================================
   Job matching service - supporting indexes
   Target DB: LZ_Saas (Main tenant / SaaS-Main)
   Date: 2026-10-03

   NOTE: comments kept in English on purpose (sqlcmd decodes
   .sql files as GBK and Chinese comments can corrupt syntax).

   1. IX_Live_JobsVisit_UserJob   : forward profile + reverse affinity
   2. IX_Live_UserEntry_UserJID   : entry profile + credibility (covering)
   3. IX_Live_User_Company_Last   : reverse candidate recall (order by LastTime)

   All statements are re-runnable (IF NOT EXISTS guards).
   ============================================================ */

USE LZ_Saas;
GO

/* ------------------------------------------------------------
   1) Live_JobsVisit (UserID, JobID)
   Serves:
     - GetUserProfile:   WHERE UserID = @u AND JobID > 0
                         AND (StartTime >= @f OR CreateTime >= @f)
     - GetJobAffinity:   WHERE UserID IN (...) AND JobID > 0
   INCLUDE columns make both queries fully covered.
   ------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID(N'dbo.Live_JobsVisit')
                 AND name = N'IX_Live_JobsVisit_UserJob')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Live_JobsVisit_UserJob
        ON dbo.Live_JobsVisit (UserID, JobID)
        INCLUDE (StartTime, CreateTime, Seconds, Keyword, Search)
        WITH (FILLFACTOR = 90);
    PRINT 'Created IX_Live_JobsVisit_UserJob';
END
ELSE
    PRINT 'IX_Live_JobsVisit_UserJob already exists, skipped';
GO

/* ------------------------------------------------------------
   2) Live_UserEntry (UserID, JID)
   Serves (covering, no lookups back to the table):
     - GetUserProfile:     WHERE UserID = @u AND JID > 0 AND (EntryTime >= @f OR CreateTime >= @f)
     - GetUserCredibility: WHERE UserID IN (...) AND JID > 0   (no time filter, full history)
   Table currently has NO secondary index at all; every profile
   query is a clustered index scan. This is the highest priority.
   ------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID(N'dbo.Live_UserEntry')
                 AND name = N'IX_Live_UserEntry_UserJID')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Live_UserEntry_UserJID
        ON dbo.Live_UserEntry (UserID, JID)
        INCLUDE (Status, EntryTime, LeaveTime, Days, CreateTime)
        WITH (FILLFACTOR = 90, ONLINE = OFF);
    PRINT 'Created IX_Live_UserEntry_UserJID';
END
ELSE
    PRINT 'IX_Live_UserEntry_UserJID already exists, skipped';
GO

/* ------------------------------------------------------------
   3) Live_User (CompanyID, LastTime DESC)
   Serves RecommendUsers candidate recall:
     WHERE Enabled = 1 AND IsBlack = 0 AND CompanyID = @c
     ORDER BY LastTime DESC
   Seek on CompanyID + backward ordered scan on LastTime
   avoids a full sort of the user table.
   Only needed if the reverse (dispatch) endpoint is enabled.
   ------------------------------------------------------------ */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID(N'dbo.Live_User')
                 AND name = N'IX_Live_User_Company_Last')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Live_User_Company_Last
        ON dbo.Live_User (CompanyID, LastTime DESC)
        INCLUDE (Enabled, IsBlack, Sex, Age)
        WITH (FILLFACTOR = 90);
    PRINT 'Created IX_Live_User_Company_Last';
END
ELSE
    PRINT 'IX_Live_User_Company_Last already exists, skipped';
GO

/* ------------------------------------------------------------
   Rollback (uncomment to remove):
   DROP INDEX IX_Live_JobsVisit_UserJob ON dbo.Live_JobsVisit;
   DROP INDEX IX_Live_UserEntry_UserJID ON dbo.Live_UserEntry;
   DROP INDEX IX_Live_User_Company_Last ON dbo.Live_User;
   ------------------------------------------------------------ */

/* ------------------------------------------------------------
   4) OPTIONAL - reverse dispatch affinity recall.
   Only needed when RecommendUsers (dispatch endpoint) is live.
   RecallAffinityUsers filters behavior tables BY JOB id, which
   the (UserID, ...) indexes above cannot serve.
   ------------------------------------------------------------ */

/* Live_JobsVisit by JobID (this table grows fastest) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID(N'dbo.Live_JobsVisit')
                 AND name = N'IX_Live_JobsVisit_Job')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Live_JobsVisit_Job
        ON dbo.Live_JobsVisit (JobID)
        INCLUDE (UserID, StartTime, CreateTime)
        WITH (FILLFACTOR = 90);
    PRINT 'Created IX_Live_JobsVisit_Job';
END
ELSE
    PRINT 'IX_Live_JobsVisit_Job already exists, skipped';
GO

/* Live_UserEntry by JID (76k rows, scan is tolerable but cheap to fix) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID(N'dbo.Live_UserEntry')
                 AND name = N'IX_Live_UserEntry_JID')
BEGIN
    CREATE NONCLUSTERED INDEX IX_Live_UserEntry_JID
        ON dbo.Live_UserEntry (JID)
        INCLUDE (UserID, Status, Days, EntryTime, CreateTime)
        WITH (FILLFACTOR = 90);
    PRINT 'Created IX_Live_UserEntry_JID';
END
ELSE
    PRINT 'IX_Live_UserEntry_JID already exists, skipped';
GO

/* ------------------------------------------------------------
   Verification: check index usage after a few days of traffic
   SELECT i.name, s.user_seeks, s.user_scans, s.user_lookups
   FROM sys.dm_db_index_usage_stats s
   JOIN sys.indexes i ON i.object_id = s.object_id AND i.index_id = s.index_id
   WHERE s.database_id = DB_ID()
     AND i.name LIKE 'IX_Live_%';
   ------------------------------------------------------------ */
