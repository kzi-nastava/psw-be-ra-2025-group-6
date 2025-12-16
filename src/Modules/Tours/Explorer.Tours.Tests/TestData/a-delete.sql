DELETE FROM tours."PublicEntityRequests";
DELETE FROM tours."TouristEquipment";
DELETE FROM tours."Journals";
DELETE FROM tours."Meetups";
DELETE FROM tours."KeyPoints";  -- KeyPoints has FK to Tours
DELETE FROM tours."Equipment";  -- Equipment has FK to Tours
DELETE FROM tours."Facility";
DELETE FROM tours."AnnualAwards";
DELETE FROM tours."Monuments";
DELETE FROM tours."Tours";  -- Delete Tours last
ALTER SEQUENCE tours."Facility_Id_seq" RESTART WITH 100;
