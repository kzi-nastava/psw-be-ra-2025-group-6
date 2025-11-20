-- Profile data uses negative IDs matching related people records
INSERT INTO stakeholders."Profiles"(
    "Id", "Name", "Surname", "Biography", "Motto", "ProfilePictureUrl", "PersonId")
VALUES (-201, 'Pera', 'Peric', 'Planinar i ljubitelj prirode.', 'Carpe diem', 'http://example.com/pera.jpg', -21);

INSERT INTO stakeholders."Profiles"(
    "Id", "Name", "Surname", "Biography", "Motto", "ProfilePictureUrl", "PersonId")
VALUES (-202, 'Milan', 'Milic', 'Uzivam u duzim pesackim turama.', 'Samo hrabro', NULL, -22);

INSERT INTO stakeholders."Profiles"(
    "Id", "Name", "Surname", "Biography", "Motto", "ProfilePictureUrl", "PersonId")
VALUES (-301, 'Ana', 'Anic', 'Autorka avanturistickih tura.', 'Pisem dok lutam.', NULL, -11);
