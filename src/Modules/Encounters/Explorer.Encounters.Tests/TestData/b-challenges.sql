-- Seed challenges
INSERT INTO encounters."Challenges" ("Id", "Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ImagePath", "ActivationRadiusMeters", "KeyPointId", "IsRequiredForSecret") VALUES
(-1, 'Quiz Challenge 1', 'Test quiz for challenge 1', 19.84, 45.25, 50, 'Active', 'Quiz', 1, false, NULL, 50, -11, false),
(-2, 'Quiz Challenge 2', 'Test quiz for challenge 2', 19.85, 45.26, 30, 'Active', 'Quiz', 1, false, NULL, 50, -11, false),
(-3, 'Quiz Challenge 3', 'Test quiz for challenge 3', 19.86, 45.27, 40, 'Active', 'Quiz', 1, false, NULL, 50, -10, false),
(-4, 'Quiz Challenge 4', 'Test quiz for challenge 4', 19.87, 45.28, 100, 'Active', 'Quiz', 1, false, NULL, 50, -10, false),
(-5, 'Quiz Challenge 5', 'Test quiz for challenge 5', 19.88, 45.29, 50, 'Active', 'Quiz', 1, false, NULL, 50, -11, false),
(-6, 'Quiz Challenge 6', 'Test quiz for challenge 6', 19.89, 45.30, 30, 'Active', 'Quiz', 1, false, NULL, 50, -11, false),
(-7, 'Quiz Challenge 7', 'Test quiz for challenge 7', 19.90, 45.31, 60, 'Active', 'Quiz', 1, false, NULL, 50, -10, false),
(-8, 'Quiz Challenge 8', 'Test quiz for challenge 8', 19.91, 45.32, 40, 'Active', 'Quiz', 1, false, NULL, 50, -10, false),
(-9, 'Quiz Challenge 9', 'Test quiz for challenge 9', 19.92, 45.33, 50, 'Active', 'Quiz', 1, false, NULL, 50, -11, false),
(-10, 'Quiz Challenge 10', 'Test quiz for challenge 10', 19.93, 45.34, 70, 'Active', 'Quiz', 1, false, NULL, 50, -11, false),
(-11, 'Location Challenge', 'Find the hidden spot', 19.94, 45.35, 50, 'Active', 'Location', NULL, false, '/images/challenges/statue.jpg', 50, NULL, false),
(-12, 'Archived Challenge', 'Old challenge', 19.95, 45.36, 20, 'Archived', 'Location', NULL, false, NULL, 50, NULL, false);

-- Seed XP Profiles
INSERT INTO encounters."TouristXpProfiles" ("Id", "UserId", "CurrentXP", "Level", "LevelUpHistory") VALUES
(-1, 1, 0, 1, '[]'),
(-2, 2, 550, 10, '[]'),
(-3, 3, 100, 2, '[]'),
(-4, 4, 200, 3, '[]'),
(-5, 5, 0, 1, '[]');

-- Seed Encounter Completions
INSERT INTO encounters."EncounterCompletions" ("Id", "UserId", "ChallengeId", "CompletedAt", "XpAwarded") VALUES
(-1, 1, -11, '2025-01-10 10:00:00+00', 50);

-- Seed Hidden Location Attempts
INSERT INTO encounters."HiddenLocationAttempts" ("Id", "UserId", "ChallengeId", "StartedAt", "CompletedAt", "IsSuccessful", "SecondsInRadius", "LastPositionUpdate") VALUES
(-1, 1, -1, '2025-01-13 20:00:00+00', '2025-01-13 20:01:00+00', true, 30, '2025-01-13 20:01:00+00'),
(-2, 2, -2, '2025-01-13 21:00:00+00', NULL, false, 15, '2025-01-13 21:00:15+00');
