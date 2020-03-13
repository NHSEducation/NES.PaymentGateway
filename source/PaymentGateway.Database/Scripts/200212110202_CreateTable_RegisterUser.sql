/* Migration Script */
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RegisterUser' and xtype='U')
BEGIN
	BEGIN
		CREATE TABLE [dbo].[RegisterUser](
			[UserID] INT NOT NULL IDENTITY(1, 1),
			[Username] VARCHAR(MAX),
			[Password] VARCHAR(MAX),
			[EmailID] VARCHAR(MAX),
			[Created] DATETIME
		) ON [PRIMARY]
	END

	BEGIN
		ALTER TABLE [dbo].[RegisterUser]
			ADD CONSTRAINT [PK_RegisterUser] PRIMARY KEY CLUSTERED ([UserID]) WITH (FILLFACTOR=88) ON [PRIMARY]
	END

	BEGIN
		-- Encrypted password for s6aNeSpuZi
		INSERT INTO RegisterUser ([Username], [Password], [Created])
		VALUES ('NesPGAdmin', 'D5VeoGOlCTKtohLNCzy+8A==', GETDATE())
	END
END
GO
