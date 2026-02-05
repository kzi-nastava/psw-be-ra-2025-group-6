-- Seed challenges attached to KeyPoints
-- These challenges use KeyPointId instead of their own coordinates
-- Assuming KeyPoints with Ids -11 and -10 exist from Tours module

INSERT INTO encounters."Challenges" ("Id", "Title", "Description", "Longitude", "Latitude", "XP", "Status", "Type", "CreatorId", "IsCreatedByTourist", "ImagePath", "ActivationRadiusMeters", "KeyPointId", "IsRequiredForSecret") VALUES
-- Challenges for KeyPoint -11 (Eiffel Tower) - owned by Author 1
(-100, 'Eiffel Tower Photo Challenge', 'Take a photo from the exact spot where the original image was taken', 2.2945, 48.8584, 100, 'Active', 'Location', 1, false, '/images/challenges/eiffel-spot.jpg', 50, -11, true),
(-101, 'Eiffel Social Meetup', 'Meet 3 other tourists at the Eiffel Tower', 2.2945, 48.8584, 50, 'Active', 'Social', 1, false, NULL, 75, -11, false),
(-102, 'Eiffel Tower Trivia', 'Answer trivia questions about the Eiffel Tower', 2.2945, 48.8584, 30, 'Active', 'Misc', 1, false, NULL, 50, -11, false),

-- Challenges for KeyPoint -10 (Louvre) - owned by Author 2 (for testing authorization)
(-103, 'Louvre Art Hunt', 'Find the hidden artwork in the museum area', 2.3376, 48.8606, 80, 'Active', 'Location', 2, false, '/images/challenges/louvre-art.jpg', 50, -10, true),
(-104, 'Louvre History Challenge', 'Learn about the history of Louvre', 2.3376, 48.8606, 40, 'Active', 'Misc', 2, false, NULL, 50, -10, false);
