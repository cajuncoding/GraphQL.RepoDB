IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsCharacters', N'U'))
BEGIN
	CREATE TABLE [StarWarsCharacters](
	  id INTEGER  NOT NULL PRIMARY KEY,
	  name VARCHAR(30) NOT NULL
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

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.StarWarsAppearances', N'U'))
BEGIN
	CREATE TABLE StarWarsAppearances(
	  characterId INTEGER  NOT NULL,
	  title VARCHAR(30) NOT NULL

	  PRIMARY KEY (characterId, title)
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
		(1004, 'ATTACK_OF_THE_CLONES'),
		(2000,'NEW_HOPE'),
		(2000,'EMPIRE'),
		(2000,'JEDI'),
		(2001,'NEW_HOPE'),
		(2001,'EMPIRE'),
		(2001,'JEDI');
END


