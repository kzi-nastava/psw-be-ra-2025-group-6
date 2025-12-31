-- Test data for Tourist Encounter System
-- Run this after migrations are applied

-- Insert sample challenges for testing
INSERT INTO encounters."Challenges" ("Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist")
VALUES 
    ('Morning Workout', 'Do 20 push-ups at the central park', 19.845329, 45.267136, 50, 'Active', 'Misc', NULL, false),
    ('River Jump', 'Jump over the small stream near the bridge', 19.850123, 45.270234, 75, 'Active', 'Misc', NULL, false),
    ('Tree Climb', 'Climb the big oak tree in the botanical garden', 19.833456, 45.260789, 100, 'Active', 'Misc', NULL, false),
    ('Stairs Sprint', 'Run up the stadium stairs 3 times', 19.842567, 45.255432, 60, 'Active', 'Misc', NULL, false),
    ('Balance Beam', 'Walk across the balance beam in the fitness park', 19.848901, 45.268345, 40, 'Active', 'Misc', NULL, false);

-- Note: Tourist XP Profiles and Encounter Completions will be created automatically when tourists interact with the system
-- You can manually insert a test profile if needed:
-- INSERT INTO encounters."TouristXpProfiles" ("UserId", "CurrentXP", "Level", "LevelUpHistory")
-- VALUES 
--     (1, 0, 1, '[{"Level":1,"Timestamp":"2025-01-01T10:00:00Z"}]');

-- Sample tourist-created challenge (requires level 10 tourist)
-- This would be created through the API after a tourist reaches level 10
-- INSERT INTO encounters."Challenges" ("Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist")
-- VALUES 
--     ('Advanced Parkour', 'Complete the advanced parkour route', 19.855678, 45.272901, 150, 'Draft', 'Misc', 123, true);

-- Verify data
SELECT * FROM encounters."Challenges";
SELECT * FROM encounters."TouristXpProfiles";
SELECT * FROM encounters."EncounterCompletions";
