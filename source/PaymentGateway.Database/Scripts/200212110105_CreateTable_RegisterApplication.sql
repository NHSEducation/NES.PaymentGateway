/* Migration Script */
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RegisterApplication' and xtype='U')
BEGIN
	BEGIN
		CREATE TABLE [dbo].[RegisterApplication]
		(
			[ApplicationID] INT NOT NULL IDENTITY(1, 1),
			[Description] VARCHAR(MAX),
			[Name] VARCHAR(200),
			[Created] DATETIME,
			[IsActive] BIT
		) ON [PRIMARY]
	END
	
	BEGIN
		ALTER TABLE [dbo].[RegisterApplication] 
			ADD CONSTRAINT [PK_RegisterApplication] PRIMARY KEY CLUSTERED  ([ApplicationID]) WITH (FILLFACTOR=80) ON [PRIMARY]
	END
END
GO