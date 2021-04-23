SELECT * FROM StarWarsCharacters ORDER BY Id ASC

DECLARE @page AS INT = 1;
DECLARE @rowsPerBatch AS INT = 8;

SELECT TOP((@page + 1)*@rowsPerBatch) ROW_NUMBER() OVER (ORDER BY Id ASC) AS [RowNumber], [Id], [Name] FROM [StarWarsCharacters] ORDER BY Id ASC;

--Modeled from RepoDB's BatchQuery logic (query builder)
WITH CTE AS (SELECT TOP((@page + 1)*@rowsPerBatch) ROW_NUMBER() OVER (ORDER BY Id ASC) AS [RowNumber], [Id], [Name] FROM [StarWarsCharacters] ORDER BY Id ASC)
SELECT [Id], [Name] FROM CTE WHERE ([RowNumber] BETWEEN ((@page * @rowsPerBatch) + 1) AND ((@page + 1) * @rowsPerBatch));

SELECT
	BetweenSql = 'Between ' + CONVERT(VARCHAR(5), ((@page * @rowsPerBatch) + 1)) + ' and ' + CONVERT(VARCHAR(5), ((@page + 1) * @rowsPerBatch));