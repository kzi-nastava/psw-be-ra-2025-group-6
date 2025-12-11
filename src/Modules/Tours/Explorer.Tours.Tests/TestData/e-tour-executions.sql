INSERT INTO tours."TourExecutions"("Id", "TourId", "TouristId", "Status", "StartTime", "EndTime", "InitialPositionJson", "ExecutionKeyPointsJson", "LastActivity", "CompletedKeyPointsJson", "ProgressPercentage", "CurrentKeyPointId")
VALUES (-1, -3, 1, 'active', '2024-01-15 10:00:00', NULL, '{{"Latitude": 48.8566, "Longitude": 2.3522}}', '[]', '2024-01-15 10:00:00', '[]', 0, -11);

INSERT INTO tours."TourExecutions"("Id", "TourId", "TouristId", "Status", "StartTime", "EndTime", "InitialPositionJson", "ExecutionKeyPointsJson", "LastActivity", "CompletedKeyPointsJson", "ProgressPercentage", "CurrentKeyPointId")
VALUES (-2, -3, 2, 'completed', '2024-01-14 09:00:00', '2024-01-14 15:00:00', '{{"Latitude": 48.8566, "Longitude": 2.3522}}', '[-11, -10]', '2024-01-14 15:00:00', '[{{"KeyPointId": -11, "KeyPointName": "Start", "UnlockedSecret": "secret-start", "CompletedAt": "2024-01-14T10:30:00Z"}}, {{"KeyPointId": -10, "KeyPointName": "Eiffel", "UnlockedSecret": "secret-eiffel", "CompletedAt": "2024-01-14T14:00:00Z"}}]', 100, NULL);


INSERT INTO tours."TourExecutions"("Id", "TourId", "TouristId", "Status", "StartTime", "EndTime", "InitialPositionJson", "ExecutionKeyPointsJson", "LastActivity", "CompletedKeyPointsJson", "ProgressPercentage", "CurrentKeyPointId")
VALUES (-3, -3, 3, 'active', '2024-01-15 08:00:00', NULL, '{{"Latitude": 48.8566, "Longitude": 2.3522}}', '[-11]', '2024-01-15 11:00:00', '[{{"KeyPointId": -11, "KeyPointName": "Start", "UnlockedSecret": "secret-start", "CompletedAt": "2024-01-15T09:00:00Z"}}]', 50, -10);

INSERT INTO tours."TourExecutions"("Id", "TourId", "TouristId", "Status", "StartTime", "EndTime", "InitialPositionJson", "ExecutionKeyPointsJson", "LastActivity", "CompletedKeyPointsJson", "ProgressPercentage", "CurrentKeyPointId")
VALUES (-4, -3, 4, 'abandoned', '2024-01-10 10:00:00', '2024-01-10 12:00:00', '{{"Latitude": 48.8566, "Longitude": 2.3522}}', '[]', '2024-01-10 12:00:00', '[]', 0, -11);