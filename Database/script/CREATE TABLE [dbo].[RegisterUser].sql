CREATE TABLE [dbo].[RegisterUser]
(
	[UserID] INT NOT NULL IDENTITY(1, 1),
	[Username] VARCHAR(MAX),
	[Password] VARCHAR(MAX),
	[EmailID] VARCHAR(MAX),
	[Created] DATETIME
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RegisterUser] 
	ADD CONSTRAINT [PK_RegisterUser] PRIMARY KEY CLUSTERED  ([UserID]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO

-- Encrypted password for s6aNeSpuZi
INSERT INTO RegisterUser ([Username], [Password], [Created])
VALUES ('NesPGAdmin', 'D5VeoGOlCTKtohLNCzy+8A==', GETDATE())
GO