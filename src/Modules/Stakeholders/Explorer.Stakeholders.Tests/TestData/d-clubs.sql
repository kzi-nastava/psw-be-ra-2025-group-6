-- Klub -21 je aktivan (0)
INSERT INTO stakeholders."Clubs"("Id", "Name", "Description", "ImageUris", "OwnerId", "Status") 
VALUES (-21, 'Klub putnika', 'Klub za ljubitelje putovanja.', ARRAY['slika1.png'], -21, 0);

-- Klub -2  Closed (1) za potrebe testiranja zabrane slanja zahteva
INSERT INTO stakeholders."Clubs"("Id", "Name", "Description", "ImageUris", "OwnerId", "Status") 
VALUES (-2, 'Planinarski klub', 'Klub za planinare.', ARRAY['slika2.png'], -22, 1);

-- Klub -3 je aktivan (0)
INSERT INTO stakeholders."Clubs"("Id", "Name", "Description", "ImageUris", "OwnerId", "Status") 
VALUES (-3, 'Biciklistički klub', 'Klub za bicikliste i avanturiste.', ARRAY['slika3.png'], -21, 0);