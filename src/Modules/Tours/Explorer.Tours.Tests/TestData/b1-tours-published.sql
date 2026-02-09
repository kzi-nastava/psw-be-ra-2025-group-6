UPDATE tours."Tours" SET "PublishedTime" = TIMESTAMPTZ '2024-11-01 00:00:00+00' WHERE "Id" = -3;
UPDATE tours."Tours" SET "PublishedTime" = TIMESTAMPTZ '2024-10-01 00:00:00+00' WHERE "Id" = -5;
UPDATE tours."Tours" SET "PublishedTime" = TIMESTAMPTZ '2024-12-15 00:00:00+00' WHERE "Id" = -10;
UPDATE tours."Tours" SET "PublishedTime" = TIMESTAMPTZ '2025-02-10 00:00:00+00' WHERE "Id" = -11;
UPDATE tours."Tours" SET "PublishedTime" = TIMESTAMPTZ '2024-12-01 00:00:00+00' WHERE "Id" = -12;
UPDATE tours."Tours" SET "PublishedTime" = TIMESTAMPTZ '2025-02-05 00:00:00+00' WHERE "Id" = -13;

UPDATE tours."Tours" SET "Duration" = '[{{"TravelType":0,"Minutes":60}}]'::jsonb WHERE "Id" = -13;
