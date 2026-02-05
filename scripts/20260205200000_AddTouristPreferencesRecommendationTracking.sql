START TRANSACTION;

ALTER TABLE stakeholders."TouristPreferences" DROP CONSTRAINT "FK_TouristPreferences_Users_TouristId";

UPDATE stakeholders."TouristPreferences" AS tp
SET "TouristId" = p."Id"
FROM stakeholders."People" AS p
WHERE p."UserId" = tp."TouristId";

ALTER TABLE stakeholders."TouristPreferences" ADD "LastNotifiedAt" timestamp with time zone;

ALTER TABLE stakeholders."TouristPreferences" ADD "LastSeenRecommendationsAt" timestamp with time zone;

ALTER TABLE stakeholders."TouristPreferences" ADD CONSTRAINT "FK_TouristPreferences_People_TouristId" FOREIGN KEY ("TouristId") REFERENCES stakeholders."People" ("Id") ON DELETE CASCADE;

INSERT INTO stakeholders."__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260205200000_AddTouristPreferencesRecommendationTracking', '8.0.11');

COMMIT;

