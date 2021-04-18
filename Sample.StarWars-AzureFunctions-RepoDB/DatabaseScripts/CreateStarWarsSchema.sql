IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsCharacters', N'U'))
BEGIN
	CREATE TABLE [StarWarsCharacters](
		[Id] INTEGER  NOT NULL,
		[Name] VARCHAR(30) NOT NULL,
		[HomePlanet] VARCHAR(30),
		[PrimaryFunction] VARCHAR(30),
		PRIMARY KEY ([Id])
	);

	INSERT INTO [StarWarsCharacters]([Id], [Name], [HomePlanet], [PrimaryFunction]) VALUES 
		(1000,'Luke Skywalker', 'Tatooine', null),
		(1001,'Darth Vader', 'Tatooine', null),
		(1002,'Han Solo', null, null),
		(1003,'Leia Organa', 'Alderaan', null),
		(1004,'Wilhuff Tarkin', null, null),
		(1005, 'Jango Fett', null, null),
		(2000,'C-3PO', null, 'Protocol'),
		(2001,'R2-D2', null, 'Astromech');
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsStarships', N'U'))
BEGIN
	CREATE TABLE [StarWarsStarships](
		[Id] INTEGER  NOT NULL,
		[Name] VARCHAR(30) NOT NULL,
		[Length] FLOAT,
		PRIMARY KEY ([Id])
	);

	INSERT INTO [StarWarsStarships]([Id], [Name], [Length]) VALUES 
		(3000,'TIE Advanced x1', 9.2);

END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsAppearances', N'U'))
BEGIN
	CREATE TABLE StarWarsAppearances(
	  [CharacterId] INTEGER  NOT NULL,
	  [Title] VARCHAR(30) NOT NULL,

	  PRIMARY KEY ([CharacterId], [Title]),
	  FOREIGN KEY ([CharacterId]) REFERENCES StarWarsCharacters([Id]),
	);

	INSERT INTO StarWarsAppearances([CharacterId], [Title]) VALUES 
		(1000,'NewHope'),
		(1000,'Empire'),
		(1000,'Jedi'),
		(1001,'NewHope'),
		(1001,'Empire'),
		(1001,'Jedi'),
		(1002,'NewHope'),
		(1002,'Empire'),
		(1002,'Jedi'),
		(1003,'NewHope'),
		(1003,'Empire'),
		(1003,'Jedi'),
		(1004,'NewHope'),
		(1005,'AttackOfTheClones'),
		(2000,'NewHope'),
		(2000,'Empire'),
		(2000,'Jedi'),
		(2001,'NewHope'),
		(2001,'Empire'),
		(2001,'Jedi');
END

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsFriends', N'U'))
BEGIN
	CREATE TABLE StarWarsFriends(
		[CharacterId] INTEGER  NOT NULL,
		[FriendId] INTEGER NOT NULL,
		PRIMARY KEY ([CharacterId], [FriendId]),
		FOREIGN KEY ([CharacterId]) REFERENCES StarWarsCharacters([Id]),
		FOREIGN KEY ([FriendId]) REFERENCES StarWarsCharacters([Id])
	);

	INSERT INTO StarWarsFriends([CharacterId], [FriendId]) VALUES 
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

--Create A View to greatly simplify the Code view into the Friends relationship...
CREATE OR ALTER VIEW ViewStarWarsFriends AS
	SELECT c.*, f.Id as FriendOfId
	FROM StarWarsCharacters c 
		join StarWarsFriends j on c.Id = j.CharacterId
		join StarWarsCharacters f on j.FriendId = f.Id;
GO

