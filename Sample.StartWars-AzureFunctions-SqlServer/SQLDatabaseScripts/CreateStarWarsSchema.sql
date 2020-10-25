IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsCharacters', N'U'))
BEGIN
	CREATE TABLE [StarWarsCharacters](
		Id INTEGER  NOT NULL,
		Name VARCHAR(30) NOT NULL,
		PRIMARY KEY (Id)
	);

	INSERT INTO StarWarsCharacters(id,name) VALUES 
		(1000,'Luke Skywalker'),
		(1001,'Darth Vader'),
		(1002,'Han Solo'),
		(1003,'Leia Organa'),
		(1004,'Wilhuff Tarkin'),
		(1005, 'Jango Fett'),
		(2000,'C-3PO'),
		(2001,'R2-D2');
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsAppearances', N'U'))
BEGIN
	CREATE TABLE StarWarsAppearances(
	  CharacterId INTEGER  NOT NULL,
	  Title VARCHAR(30) NOT NULL,

	  PRIMARY KEY (CharacterId, title),
	  FOREIGN KEY (CharacterId) REFERENCES StarWarsCharacters(Id),
	);

	INSERT INTO StarWarsAppearances(characterId, title) VALUES 
		(1000,'NEW_HOPE'),
		(1000,'EMPIRE'),
		(1000,'JEDI'),
		(1001,'NEW_HOPE'),
		(1001,'EMPIRE'),
		(1001,'JEDI'),
		(1002,'NEW_HOPE'),
		(1002,'EMPIRE'),
		(1002,'JEDI'),
		(1003,'NEW_HOPE'),
		(1003,'EMPIRE'),
		(1003,'JEDI'),
		(1004,'NEW_HOPE'),
		(1005,'ATTACK_OF_THE_CLONES'),
		(2000,'NEW_HOPE'),
		(2000,'EMPIRE'),
		(2000,'JEDI'),
		(2001,'NEW_HOPE'),
		(2001,'EMPIRE'),
		(2001,'JEDI');
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsFriends', N'U'))
BEGIN
	CREATE TABLE StarWarsFriends(
		CharacterId INTEGER  NOT NULL,
		FriendId INTEGER NOT NULL,
		PRIMARY KEY (CharacterId, FriendId),
		FOREIGN KEY (CharacterId) REFERENCES StarWarsCharacters(Id),
		FOREIGN KEY (FriendId) REFERENCES StarWarsCharacters(Id)
	);

	INSERT INTO StarWarsFriends(CharacterId, FriendId) VALUES 
		(1000,1002),
		(1000,1003),
		(1000,2000),
		(1000,2001),
		(1001,1004),
		(1002,1000),
		(1002,1003),
		(1002,2001),
		(1003,1000),
		(1003,1002),
		(1003,2000),
		(1003,2001),
		(1004,1001),
		(2000,1000),
		(2000,1002),
		(2000,1003),
		(2000,2001),
		(2001,1000),
		(2001,1002),
		(2001,1003)
END
GO

CREATE OR ALTER VIEW ViewStarWarsFriends AS
	SELECT c.*, c.Id as FriendOfId
	FROM StarWarsCharacters c 
		join StarWarsFriends j on c.Id = j.CharacterId
		join StarWarsCharacters f on j.FriendId = f.Id;
GO
