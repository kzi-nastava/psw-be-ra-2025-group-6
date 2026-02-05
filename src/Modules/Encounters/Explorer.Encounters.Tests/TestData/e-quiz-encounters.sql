-- Test data for Quiz Encounter Integration Tests
-- Delete any existing quiz test data first
DELETE FROM encounters."QuizCompletions" WHERE "ChallengeId" IN (-201, -202, -203, -204, -205, -206, -207, -208, -209, -210);
DELETE FROM encounters."QuizQuestions" WHERE "QuizEncounterId" IN (SELECT "Id" FROM encounters."QuizEncounters" WHERE "ChallengeId" IN (-201, -202, -203, -204, -205, -206, -207, -208, -209, -210));
DELETE FROM encounters."QuizEncounters" WHERE "ChallengeId" IN (-201, -202, -203, -204, -205, -206, -207, -208, -209, -210);
DELETE FROM encounters."Challenges" WHERE "Id" IN (-201, -202, -203, -204, -205, -206, -207, -208, -209, -210);

-- Insert Quiz-type challenges for testing
-- Type: 3=Quiz (Social=0, Location=1, Misc=2, Quiz=3)
-- Status: Active, Draft, Archived
INSERT INTO encounters."Challenges" ("Id", "Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ImagePath", "ActivationRadiusMeters", "KeyPointId", "IsRequiredForSecret")
VALUES
    (-201, 'Quiz: Petrovaradin Fortress History', 'Test your knowledge about the famous Clock Tower', 19.866142, 45.254348, 150, 'Active', 'Quiz', -11, false, '/images/fortress.jpg', 50, -1, true),
    (-202, 'Quiz: Danube River Facts', 'Questions about the Danube', 19.850000, 45.250000, 100, 'Active', 'Quiz', -11, false, null, 50, -2, false),
    (-203, 'Quiz: Serbian History', 'Test your history knowledge', 19.845000, 45.255000, 120, 'Active', 'Quiz', -11, false, null, 50, -3, false),
    (-204, 'Quiz: Architecture Styles', 'Questions about architecture', 19.855000, 45.245000, 130, 'Active', 'Quiz', -11, false, null, 50, -4, false),
    (-205, 'Quiz: Hard History Test', 'Difficult history quiz', 19.860000, 45.260000, 200, 'Active', 'Quiz', -11, false, null, 50, -5, true),
    (-206, 'Quiz: General Knowledge', 'Mixed questions', 19.865000, 45.265000, 80, 'Active', 'Quiz', -11, false, null, 50, -6, false),
    (-207, 'Quiz: Local Culture', 'Cultural questions', 19.870000, 45.270000, 90, 'Active', 'Quiz', -11, false, null, 50, -7, false),
    (-208, 'Quiz: To Be Deleted', 'This will be deleted', 19.875000, 45.275000, 50, 'Active', 'Quiz', -11, false, null, 50, -8, false),
    (-209, 'Quiz: Author Collection 1', 'First in collection', 19.880000, 45.280000, 70, 'Active', 'Quiz', -11, false, null, 50, -9, false),
    (-210, 'Quiz: Author Collection 2', 'Second in collection', 19.885000, 45.285000, 75, 'Active', 'Quiz', -11, false, null, 50, -10, false);

-- Ensure XP profiles exist for test users
INSERT INTO encounters."TouristXpProfiles" ("UserId", "CurrentXP", "Level", "LevelUpHistory")
SELECT -11, 1000, 10, NULL
WHERE NOT EXISTS (SELECT 1 FROM encounters."TouristXpProfiles" WHERE "UserId" = -11);

INSERT INTO encounters."TouristXpProfiles" ("UserId", "CurrentXP", "Level", "LevelUpHistory")
SELECT -21, 500, 8, NULL
WHERE NOT EXISTS (SELECT 1 FROM encounters."TouristXpProfiles" WHERE "UserId" = -21);

-- Note: QuizEncounters, QuizQuestions, and QuizCompletions will be created by the tests themselves
-- as they test the creation functionality
