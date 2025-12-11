DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'tours' AND table_name = 'Equipment'
    ) THEN
        DELETE FROM tours."Equipment";
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'blog' AND table_name = 'Blogs'
    ) THEN
        TRUNCATE blog."Blogs" RESTART IDENTITY CASCADE;
    END IF;
END $$;
