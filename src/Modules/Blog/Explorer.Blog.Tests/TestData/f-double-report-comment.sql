INSERT INTO blog."CommentReports"
(
    "BlogId",
    "CommentId",
    "UserId",
    "Reason",
    "AdditionalInfo",
    "CreatedAt",
    "ReportStatus"
)
VALUES
    (
        -1,
        (SELECT "Id"
         FROM blog."Comments"
         WHERE "BlogId" = -1 AND "Text" = 'Komentar A (autor -11)'
         ORDER BY "Id" DESC
            LIMIT 1),
    -12,
    1,
    'Dupli report – treba da pukne',
    NOW(),
    0
    );
