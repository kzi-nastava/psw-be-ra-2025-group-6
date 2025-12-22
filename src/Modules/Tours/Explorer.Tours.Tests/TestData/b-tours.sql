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
        -4,
        'Arhivirana',
        'Arhivirana tura',
        0,
        ARRAY['Kanjiza', '7 days'],
        100,
        2,
        4,
        0,
        '[]'::jsonb
    );

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -5,
    'Another Confirmed Tour',
    'Another confirmed tour',
    2,
    ARRAY['test'],
    150,
    0,
    4,
    0,
    '[]'::jsonb
);
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
        '[]'::jsonb
    );
