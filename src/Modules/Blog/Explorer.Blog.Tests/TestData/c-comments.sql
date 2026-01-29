INSERT INTO blog."Comments"
("BlogId", "UserId", "AuthorName", "AuthorProfilePicture", "Text", "CreatedAt", "LastUpdatedAt", "IsHidden")
VALUES
    (-1, -11, 'Autor -11', '', 'Komentar A (autor -11)', NOW(), NULL, false),
    (-1, -12, 'Autor -12', '', 'Komentar B (autor -12)', NOW() + INTERVAL '1 second', NULL, false),
    (-2, -12, 'Autor -12', '', 'Komentar C (blog -2)', NOW(), NULL, false),
    (-3, -21, 'Autor -21', '', 'Komentar D (blog -3)', NOW(), NULL, false);
