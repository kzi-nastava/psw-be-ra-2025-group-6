INSERT INTO stakeholders."TouristPreferences"
("Id", "TouristId", "PreferredDifficulty", "WalkRating", "BikeRating", "CarRating", "BoatRating", "Tags", "CreatedAt", "UpdatedAt")
VALUES
(-201, -21, 1, 3, 1, 2, 0, ARRAY['city', 'river'], NOW(), NOW()),
(-202, -22, 0, 3, 3, 1, 0, ARRAY['nature', 'short'], NOW(), NOW());
