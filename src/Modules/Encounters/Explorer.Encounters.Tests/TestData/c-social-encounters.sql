-- =========================
-- DELETE existing social encounter test data
-- =========================
DELETE FROM encounters."EncounterCompletions" WHERE "ChallengeId" IN (-4, -5, -6, -7, -8, -9);
DELETE FROM encounters."ActiveSocialParticipants" WHERE "SocialEncounterId" IN (-1, -2, -3, -4, -5, -6);
DELETE FROM encounters."SocialEncounters" WHERE "Id" IN (-1, -2, -3, -4, -5, -6);
DELETE FROM encounters."Challenges" WHERE "Id" IN (-4, -5, -6, -7, -8, -9);

-- =========================
-- Challenges za SocialEncounterTests (moraju ponovo da se dodaju jer a-delete.sql ih briše)
-- Status: 0=Active, 1=Draft, 2=Archived
-- Type: 0=Location, 1=Social, 2=Misc (proveri u svom projektu!)
-- =========================
INSERT INTO encounters."Challenges" 
("Id", "Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist")
VALUES
(-4, 'Activation Test', 'Test challenge for activation', 19.8453, 45.2671, 100, 0, 1, NULL, false),
(-5, 'Distance Test', 'Test challenge for distance', 19.8453, 45.2671, 100, 0, 1, NULL, false),
(-6, 'Heartbeat Test', 'Test challenge for heartbeat', 19.8453, 45.2671, 100, 0, 1, NULL, false),
(-7, 'Leave Radius Test', 'Test challenge for leaving radius', 19.8453, 45.2671, 100, 0, 1, NULL, false),
(-8, 'Deactivate Test', 'Test challenge for deactivation', 19.8453, 45.2671, 100, 0, 1, NULL, false),
(-9, 'Already Completed Test', 'Test challenge for completed', 19.8453, 45.2671, 100, 0, 1, NULL, false);

-- Resetuj sekvencu
SELECT setval(pg_get_serial_sequence('encounters."Challenges"', 'Id'), 
              GREATEST((SELECT MAX("Id") FROM encounters."Challenges"), 1));

-- =========================
-- Social Encounters
-- =========================
INSERT INTO encounters."SocialEncounters" 
("Id", "ChallengeId", "RequiredPeople", "RadiusMeters")
VALUES
(-1, -4, 2, 50), -- Activation Test
(-2, -5, 2, 10), -- Distance Test
(-3, -6, 2, 50), -- Heartbeat Test
(-4, -7, 2, 10), -- Leave Radius Test
(-5, -8, 2, 50), -- Deactivate Test
(-6, -9, 2, 50); -- Already Completed Test

-- Resetuj sekvencu za SocialEncounters
SELECT setval(pg_get_serial_sequence('encounters."SocialEncounters"', 'Id'), 
              GREATEST((SELECT MAX("Id") FROM encounters."SocialEncounters"), 1));

-- =========================
-- Encounter Completions
-- =========================
INSERT INTO encounters."EncounterCompletions"
("UserId", "ChallengeId", "CompletedAt", "XpAwarded")
VALUES
(-2, -9, NOW(), 100); -- User -2 je već završio challenge -9