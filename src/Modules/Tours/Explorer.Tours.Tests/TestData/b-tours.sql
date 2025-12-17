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

-- Mark as published for tourist view tests (Status=CONFIRMED and PublishedTime set)
UPDATE tours."Tours" SET "PublishedTime" = NOW() WHERE "Id" = -3;

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-101, -3, 'Paris Center', 'Central Paris starting point', 2.3522, 48.8566, 'paris.jpg', 'secret-paris');

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-102, -1, 'Belgrade Center', 'Belgrade downtown point', 20.4489, 44.7866, 'belgrade.jpg', 'secret-belgrade');

-- Marketplace search seed data near Novi Sad
INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -10,
    'Danube Walk',
    'Lagana šetnja uz Dunav u centru Novog Sada',
    0,
    ARRAY['dunav', 'šetnja', 'novi sad'],
    0,
    1,
    4,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -11,
    'Fortress Loop',
    'Kružna tura oko Petrovaradinske tvrđave',
    1,
    ARRAY['tvrđava', 'kružna', 'novi sad'],
    0,
    1,
    4,
    0,
    '[]'::jsonb
);

INSERT INTO tours."Tours"
("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm", "Duration")
VALUES 
(
    -12,
    'Far Tour',
    'Primer ture van Novog Sada (Beograd)',
    0,
    ARRAY['beograd', 'primer'],
    0,
    1,
    4,
    0,
    '[]'::jsonb
);

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-103, -10, 'Kej kod Železničkog mosta', 'Pogled na Dunav', 19.8335, 45.2671, 'danube-walk.jpg', 'secret-danube-walk');

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-104, -11, 'Glavni plato', 'Centralni deo tvrđave', 19.8610, 45.2520, 'fortress-loop.jpg', 'secret-fortress');

INSERT INTO tours."KeyPoints" ("Id", "TourId", "Name", "Description", "Longitude", "Latitude", "ImagePath", "Secret")
VALUES (-105, -12, 'Trg republike', 'Centralna tačka u Beogradu', 20.4612, 44.8125, 'far-tour.jpg', 'secret-far');
