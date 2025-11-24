INSERT INTO tours."Journals"(
    "Id", "TouristId", "Name", "Location", "TravelDate", "Status", "DateCreated", "DateModified")
VALUES 
    -- SVI RADOVI KORISTE TouristId 1 i datum 2020-01-01
    (-1, 1, 'Testni Dnevnik 1', 'Novi Sad', '2020-01-01 10:00:00.000000 +00:00', 0, '2020-01-01 10:00:00.000000 +00:00', '2020-01-01 10:00:00.000000 +00:00'),
    (-2, 1, 'Testni Dnevnik 2', 'Beograd', '2020-02-01 10:00:00.000000 +00:00', 1, '2020-02-01 10:00:00.000000 +00:00', '2020-02-01 10:00:00.000000 +00:00'),
    (-3, 1, 'Tuđi Dnevnik za test', 'Subotica', '2020-03-01 10:00:00.000000 +00:00', 0, '2020-03-01 10:00:00.000000 +00:00', '2020-03-01 10:00:00.000000 +00:00');