UPDATE blog."CommentReports"
SET
    "ReportStatus" = 1,      -- APPROVED
    "ReviewedAt" = NOW(),
    "ReviewerId" = -999,
    "AdminNote" = 'Admin approve iz SQL testa'
WHERE "Id" = (
    SELECT "Id"
    FROM blog."CommentReports"
    WHERE "BlogId" = -1
    ORDER BY "CreatedAt"
    LIMIT 1
    );