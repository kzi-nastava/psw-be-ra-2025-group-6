INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -1,
    'Tura Londona',
    'Vidi glavni grad Engleske',
    1,
    ARRAY['europe', '7 days'],
    0,
    0,
    3,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -2,
    'Tura Beograda',
    '',
    0,
    ARRAY[]::text[],
    0,
    0,
    3,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -3,
    'Tura Pariza',
    'Pravo u Luvr',
    0,
    ARRAY['europe', '7 days'],
    100,
    1,
    4,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -101,
    'Novi Sad Walk',
    'Center stroll',
    0,
    ARRAY['city','serbia'],
    15,
    1,
    3,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -102,
    'Belgrade Loop',
    'Farther away',
    1,
    ARRAY['city'],
    20,
    1,
    3,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -103,
    'Draft Nearby',
    'Not yet published',
    0,
    ARRAY['draft'],
    0,
    0,
    3,
    0,
    '[]'::jsonb
);
