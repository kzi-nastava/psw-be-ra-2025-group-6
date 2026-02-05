-- First ensure Notifications table has Type column
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'stakeholders' 
        AND table_name = 'Notifications' 
        AND column_name = 'Type'
    ) THEN
        ALTER TABLE stakeholders."Notifications" ADD COLUMN "Type" integer NOT NULL DEFAULT 0;
    END IF;
END $$;

-- Delete data in correct order
DELETE FROM stakeholders."TourProblemMessages";
DELETE FROM stakeholders."Notifications";
DELETE FROM stakeholders."ReviewApp";
DELETE FROM stakeholders."UserProfiles";
DELETE FROM stakeholders."TourProblems";
DELETE FROM stakeholders."TouristPositions";
DELETE FROM stakeholders."Clubs";
DELETE FROM stakeholders."People";
DELETE FROM stakeholders."Users";
