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

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-101, -3, 'Paris Center', 'Central Paris starting point', 2.3522, 48.8566, 'paris.jpg', 'secret-paris');

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-102, -1, 'Belgrade Center', 'Belgrade downtown point', 20.4489, 44.7866, 'belgrade.jpg', 'secret-belgrade');
