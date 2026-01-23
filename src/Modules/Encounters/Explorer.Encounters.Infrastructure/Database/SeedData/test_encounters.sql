-- Test data for Tourist Encounter System
-- Run this after migrations are applied

-- Insert sample challenges for testing
INSERT INTO encounters."Challenges" ("Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ActivationRadiusMeters")
VALUES 
    ('Morning Workout', 'Do 20 push-ups at the central park', 19.845329, 45.267136, 50, 'Active', 'Misc', NULL, false, 50),
    ('River Jump', 'Jump over the small stream near the bridge', 19.850123, 45.270234, 75, 'Active', 'Misc', NULL, false, 50),
    ('Tree Climb', 'Climb the big oak tree in the botanical garden', 19.833456, 45.260789, 100, 'Active', 'Misc', NULL, false, 50),
    ('Stairs Sprint', 'Run up the stadium stairs 3 times', 19.842567, 45.255432, 60, 'Active', 'Misc', NULL, false, 50),
    ('Balance Beam', 'Walk across the balance beam in the fitness park', 19.848901, 45.268345, 40, 'Active', 'Misc', NULL, false, 50);

-- Note: Tourist XP Profiles and Encounter Completions will be created automatically when tourists interact with the system
-- You can manually insert a test profile if needed:
-- INSERT INTO encounters."TouristXpProfiles" ("UserId", "CurrentXP", "Level", "LevelUpHistory")
-- VALUES 
--     (1, 0, 1, '[{"Level":1,"Timestamp":"2025-01-01T10:00:00Z"}]');

-- Sample tourist-created challenge (requires level 10 tourist)
-- This would be created through the API after a tourist reaches level 10
-- INSERT INTO encounters."Challenges" ("Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ActivationRadiusMeters")
-- VALUES 
--     ('Advanced Parkour', 'Complete the advanced parkour route', 19.855678, 45.272901, 150, 'Draft', 'Misc', 123, true, 50);

-- Verify data
SELECT * FROM encounters."Challenges";
SELECT * FROM encounters."TouristXpProfiles";
SELECT * FROM encounters."EncounterCompletions";
-- Test data for Social Encounters
-- Insert Social type challenges for Social Encounter testing
INSERT INTO encounters."Challenges" ("Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ActivationRadiusMeters")
VALUES 
    ('Group Meetup at Park', 'Meet with other tourists at the central fountain', 19.845329, 45.267136, 100, 'Active', 'Social', NULL, false, 50),
    ('Street Performance', 'Gather a crowd for street performance', 19.850123, 45.270234, 150, 'Active', 'Social', NULL, false, 50),
    ('Fitness Group Challenge', 'Join the outdoor fitness group session', 19.833456, 45.260789, 120, 'Active', 'Social', NULL, false, 50),
    ('Picnic Gathering', 'Organize a group picnic', 19.842567, 45.255432, 80, 'Active', 'Social', NULL, false, 50);

-- Insert corresponding Social Encounter configurations
-- Note: Get the actual Challenge ID's after insert, these are examples
-- You'll need to adjust the ChallengeId values based on actual IDs from the Challenges table

-- For 'Group Meetup at Park' - requires 5 people within 15m
INSERT INTO encounters."SocialEncounters" ("ChallengeId", "RequiredPeople", "RadiusMeters")
SELECT "Id", 5, 15.0 
FROM encounters."Challenges" 
WHERE "Title" = 'Group Meetup at Park';

-- For 'Street Performance' - requires 10 people within 20m
INSERT INTO encounters."SocialEncounters" ("ChallengeId", "RequiredPeople", "RadiusMeters")
SELECT "Id", 10, 20.0 
FROM encounters."Challenges" 
WHERE "Title" = 'Street Performance';

-- For 'Fitness Group Challenge' - requires 8 people within 25m
INSERT INTO encounters."SocialEncounters" ("ChallengeId", "RequiredPeople", "RadiusMeters")
SELECT "Id", 8, 25.0 
FROM encounters."Challenges" 
WHERE "Title" = 'Fitness Group Challenge';

-- For 'Picnic Gathering' - requires 6 people within 30m
INSERT INTO encounters."SocialEncounters" ("ChallengeId", "RequiredPeople", "RadiusMeters")
SELECT "Id", 6, 30.0 
FROM encounters."Challenges" 
WHERE "Title" = 'Picnic Gathering';

-- Verify Social Encounter data
SELECT c."Id", c."Title", c."Type", se."RequiredPeople", se."RadiusMeters"
FROM encounters."Challenges" c
LEFT JOIN encounters."SocialEncounters" se ON c."Id" = se."ChallengeId"
WHERE c."Type" = 'Social';