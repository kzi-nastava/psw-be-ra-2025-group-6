CREATE SCHEMA IF NOT EXISTS stakeholders;

DROP TABLE IF EXISTS stakeholders."ReviewApp" CASCADE;
DROP TABLE IF EXISTS stakeholders."UserProfiles" CASCADE;
DROP TABLE IF EXISTS stakeholders."TourProblems" CASCADE;
DROP TABLE IF EXISTS stakeholders."People" CASCADE;
DROP TABLE IF EXISTS stakeholders."Clubs" CASCADE;
DROP TABLE IF EXISTS stakeholders."Users" CASCADE;

CREATE TABLE IF NOT EXISTS stakeholders."Users"(
    "Id" bigserial PRIMARY KEY,
    "Username" text NOT NULL,
    "Password" text NOT NULL,
    "Role" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "UQ_Users_Username" UNIQUE ("Username")
);

CREATE TABLE IF NOT EXISTS stakeholders."People"(
    "Id" bigserial PRIMARY KEY,
    "UserId" bigint NOT NULL REFERENCES stakeholders."Users"("Id") ON DELETE CASCADE,
    "Name" text NOT NULL,
    "Surname" text NOT NULL,
    "Email" text NOT NULL
);

CREATE TABLE IF NOT EXISTS stakeholders."UserProfiles"(
    "Id" bigserial PRIMARY KEY,
    "UserId" bigint NOT NULL REFERENCES stakeholders."Users"("Id") ON DELETE CASCADE,
    "Name" text NOT NULL,
    "Surname" text NOT NULL,
    "ProfilePicture" text NOT NULL,
    "Biography" text NOT NULL,
    "Quote" text NOT NULL
);

CREATE TABLE IF NOT EXISTS stakeholders."ReviewApp"(
    "Id" bigserial PRIMARY KEY,
    "UserId" bigint NOT NULL REFERENCES stakeholders."Users"("Id") ON DELETE CASCADE,
    "Rating" integer NOT NULL,
    "Comment" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone
);

CREATE TABLE IF NOT EXISTS stakeholders."Clubs"(
    "Id" bigserial PRIMARY KEY,
    "Name" text NOT NULL,
    "Description" text,
    "ImageUris" text[] NOT NULL,
    "OwnerId" bigint NOT NULL
);

CREATE TABLE IF NOT EXISTS stakeholders."TourProblems"(
    "Id" bigserial PRIMARY KEY,
    "TourId" bigint NOT NULL,
    "TouristId" bigint NOT NULL,
    "Category" integer NOT NULL,
    "Priority" integer NOT NULL,
    "Description" text NOT NULL,
    "ReportedAt" timestamp without time zone NOT NULL
);
