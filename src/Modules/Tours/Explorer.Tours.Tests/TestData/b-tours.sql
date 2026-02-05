INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
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
    '[]'::jsonb,
    NULL
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
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
    '[]'::jsonb,
    NULL
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
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
    '[]'::jsonb,
    NULL
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
VALUES
    (
        -4,
        'Arhivirana',
        'Arhivirana tura',
        0,
        ARRAY['Kanjiza', '7 days'],
        100,
        2,
        4,
        0,
        '[]'::jsonb,
        NULL
    );

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
VALUES
    (
        -5,
        'Another Confirmed Tour',
        'Konfirmovana tura',
        0,
        ARRAY['Confirmed Tour', '7 days'],
        100,
        1,
        4,
        0,
        '[]'::jsonb,
        NULL
    );

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
VALUES
(
    -10,
    'Novi Sad Center Walk',
    'Setnja kroz centar Novog Sada',
    0,
    ARRAY['city', 'short'],
    0,
    1,
    3,
    0,
    '[]'::jsonb,
    NULL
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
VALUES
(
    -11,
    'Novi Sad Riverside',
    'Setnja uz Dunav',
    1,
    ARRAY['river', 'easy'],
    0,
    1,
    4,
    0,
    '[]'::jsonb,
    NULL
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration", "CoverImage")
VALUES
(
    -12,
    'Subotica Scenic Tour',
    'Duz Subotice',
    1,
    ARRAY['north', 'day'],
    0,
    1,
    4,
    0,
    '[]'::jsonb,
    NULL
);
