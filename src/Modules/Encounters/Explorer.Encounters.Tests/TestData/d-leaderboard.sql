-- Seed Leaderboard Entries (using NEGATIVE IDs that match existing test users)
INSERT INTO encounters."LeaderboardEntries" ("Id", "UserId", "Username", "TotalXP", "CompletedChallenges", "CompletedTours", "AdventureCoins", "CurrentRank", "LastUpdated", "ClubId") VALUES
(-1, -21, 'turista1@gmail.com', 500, 5, 2, 250, 2, '2025-01-24 12:00:00+00', NULL),
(-2, -22, 'turista2@gmail.com', 1000, 10, 5, 500, 1, '2025-01-24 12:00:00+00', -1),
(-3, -23, 'turista3@gmail.com', 250, 2, 1, 125, 3, '2025-01-24 12:00:00+00', -1),
(-4, -11, 'autor1@gmail.com', 100, 1, 0, 50, 4, '2025-01-24 12:00:00+00', NULL);

-- Seed Club Leaderboards
INSERT INTO encounters."ClubLeaderboards" ("Id", "ClubId", "ClubName", "TotalXP", "TotalCompletedChallenges", "TotalCompletedTours", "TotalAdventureCoins", "MemberCount", "CurrentRank", "LastUpdated") VALUES
(-1, -1, 'Test Club 1', 1250, 12, 6, 625, 2, 1, '2025-01-24 12:00:00+00'),
(-2, -2, 'Test Club 2', 500, 5, 2, 250, 1, 2, '2025-01-24 12:00:00+00');
