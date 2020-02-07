CREATE TABLE [dbo].[PaymentTransactionLog]
(
	[ID] INT NOT NULL IDENTITY(1, 1),
	[VendorTxCode] VARCHAR(20) NOT NULL,
	[Amount] DECIMAL(18, 2),
	[VPSTxID] VARCHAR(100),
	[RegistrationStatus] VARCHAR(5),
	[RegistrationStatusDetail] VARCHAR(MAX),
	[RegistrationTime] DATETIME,
	[SecurityKey] VARCHAR(15),
	[AuthorisationStatus] VARCHAR(5),
	[AuthorisationStatusDetail] VARCHAR(MAX),
	[AuthorisationTime] DATETIME,
	[CardType] VARCHAR(10),
	[LastFourDigits] VARCHAR(4)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[PaymentTransactionLog] 
	ADD CONSTRAINT [PK_PaymentTransactionLog] PRIMARY KEY CLUSTERED  ([ID]) WITH (FILLFACTOR=80) ON [PRIMARY]
GO

