INSERT INTO blog."CommentLikes"
("BlogId", "CommentId", "UserId")
VALUES
    (   -1,
        (SELECT "Id"
         FROM blog."Comments"
         WHERE "BlogId" = -1 AND "Text" = 'Komentar B (autor -12)'
         ORDER BY "Id" DESC
            LIMIT 1),
        -11
    )
