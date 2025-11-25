-- Insert user with a profile
INSERT INTO stakeholders."Users"(
	"Id", "Username", "Password", "Role", "IsActive")
	VALUES (-100, 'profileuser1@gmail.com', 'test1234', 2, true);
INSERT INTO stakeholders."People"(
	"Id", "UserId", "Name", "Surname", "Email")
	VALUES (-100, -100, 'Test', 'UserOne', 'profileuser1@gmail.com');
INSERT INTO stakeholders."UserProfiles"(
	"Id", "UserId", "Name", "Surname", "ProfilePicture", "Biography", "Quote")
	VALUES (-100, -100, 'Test', 'UserOne', 'testpic1.jpg', 'Bio for test user 1.', 'Quote for test user 1.');

-- Insert user without a profile
INSERT INTO stakeholders."Users"(
	"Id", "Username", "Password", "Role", "IsActive")
	VALUES (-101, 'profileuser2@gmail.com', 'test5678', 2, true);
INSERT INTO stakeholders."People"(
	"Id", "UserId", "Name", "Surname", "Email")
	VALUES (-101, -101, 'Test', 'UserTwo', 'profileuser2@gmail.com');

-- Insert user for 'not found' test
INSERT INTO stakeholders."Users"(
	"Id", "Username", "Password", "Role", "IsActive")
	VALUES (-102, 'profilenotfound@gmail.com', 'test9012', 2, true);
INSERT INTO stakeholders."People"(
	"Id", "UserId", "Name", "Surname", "Email")
	VALUES (-102, -102, 'Test', 'UserThree', 'profilenotfound@gmail.com');
