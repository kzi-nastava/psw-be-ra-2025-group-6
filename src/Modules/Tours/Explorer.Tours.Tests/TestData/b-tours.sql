INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm")
VALUES (-1, 'Tura Londona', 'Vidi glavni grad Engleske', 1, ARRAY['europe', '7 days'], 0, 0, 3, 0);

INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status", "AuthorId", "DistanceInKm")
VALUES (-2, 'Tura Beograda', '', 0, ARRAY[]::text[], 0, 0, 3, 0);

INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status")
VALUES (-3, 'Tura Pariza', 'Pravo u Luvr', 0, ARRAY['europe', '7 days'], 100, 1);

INSERT INTO tours."Tours"("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status")
VALUES (-4, 'Arhivirana tura', 'Test arhivirana tura za shopping cart', 0, ARRAY['test', 'archived'], 150, 2);

INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Price", "Status")
VALUES (-5, 'Another Confirmed Tour', 'Another confirmed tour', 2, ARRAY['test'], 150, 1);
