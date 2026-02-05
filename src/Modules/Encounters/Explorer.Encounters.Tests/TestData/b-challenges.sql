-- Seed challenges
INSERT INTO encounters."Challenges" ("Id", "Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ImagePath", "ActivationRadiusMeters") VALUES
(-1, 'Find the Hidden Statue', 'Find the statue using only the picture hint', 19.84, 45.25, 50, 'Active', 'Location', NULL, false, '/images/challenges/statue.jpg', 50),
(-2, 'Secret Garden Challenge', 'Discover the secret garden', 19.85, 45.26, 30, 'Active', 'Location', NULL, false, '/images/challenges/garden.jpg', 50),
(-3, 'Archived Location', 'Old challenge', 19.86, 45.27, 20, 'Archived', 'Location', NULL, false, NULL, 50),
(-4, 'Misc Challenge 1', 'Simple self-check challenge', 19.87, 45.28, 10, 'Active', 'Misc', NULL, false, NULL, 50),
(-5, 'Tourist Created Challenge', 'Created by level 10+ tourist', 19.88, 45.29, 40, 'Draft', 'Location', 1, true, '/images/challenges/tourist-spot.jpg', 50);

-- Seed XP Profiles
INSERT INTO encounters."TouristXpProfiles" ("Id", "UserId", "CurrentXP", "Level", "LevelUpHistory") VALUES
(-1, 1, 0, 1, NULL),
(-2, 2, 550, 10, NULL),
(-11, -11, 1000, 10, NULL);

-- Seed Encounter Completions
INSERT INTO encounters."EncounterCompletions" ("Id", "UserId", "ChallengeId", "CompletedAt", "XpAwarded") VALUES
(-1, 1, -4, '2025-01-10 10:00:00+00', 10);

-- Seed Hidden Location Attempts
INSERT INTO encounters."HiddenLocationAttempts" ("Id", "UserId", "ChallengeId", "StartedAt", "CompletedAt", "IsSuccessful", "SecondsInRadius", "LastPositionUpdate") VALUES
(-1, 1, -1, '2025-01-13 20:00:00+00', '2025-01-13 20:01:00+00', true, 30, '2025-01-13 20:01:00+00'),
(-2, 2, -2, '2025-01-13 21:00:00+00', NULL, false, 15, '2025-01-13 21:00:15+00');
