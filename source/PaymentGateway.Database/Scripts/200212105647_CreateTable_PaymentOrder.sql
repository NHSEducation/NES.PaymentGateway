/* Migration Script */
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PaymentOrder' and xtype='U')
BEGIN
	BEGIN
		CREATE TABLE [dbo].[PaymentOrder]
		(
			[ID] INT NOT NULL IDENTITY(1, 1),
			[VendorTxCode] VARCHAR(20) NOT NULL,
			[BookingID] INT,
			[Amount] DECIMAL(18, 2),
			[CostCentre] VARCHAR(10),
			[AccountCode] VARCHAR(10),
			[ProjectCode] VARCHAR(10),
			[VPSTxID] VARCHAR(100),
			[Status] VARCHAR(10),
			[StatusDetail] VARCHAR(MAX),
			[ProcessedDate] DATETIME
		) ON [PRIMARY]
	END

	BEGIN
		ALTER TABLE [dbo].[PaymentOrder] 
			ADD CONSTRAINT [PK_PaymentOrder] PRIMARY KEY CLUSTERED  ([ID]) WITH (FILLFACTOR=80) ON [PRIMARY]
	END
END