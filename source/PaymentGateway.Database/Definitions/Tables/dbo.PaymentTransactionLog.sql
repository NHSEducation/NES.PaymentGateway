SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[PaymentTransactionLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VendorTxCode] [varchar](20) COLLATE Latin1_General_CI_AS NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[VPSTxID] [varchar](100) COLLATE Latin1_General_CI_AS NULL,
	[RegistrationStatus] [varchar](10) COLLATE Latin1_General_CI_AS NULL,
	[RegistrationStatusDetail] [varchar](max) COLLATE Latin1_General_CI_AS NULL,
	[RegistrationTime] [datetime] NULL,
	[SecurityKey] [varchar](15) COLLATE Latin1_General_CI_AS NULL,
	[AuthorisationStatus] [varchar](5) COLLATE Latin1_General_CI_AS NULL,
	[AuthorisationStatusDetail] [varchar](max) COLLATE Latin1_General_CI_AS NULL,
	[AuthorisationTime] [datetime] NULL,
	[CardType] [varchar](10) COLLATE Latin1_General_CI_AS NULL,
	[LastFourDigits] [varchar](4) COLLATE Latin1_General_CI_AS NULL,
 CONSTRAINT [PK_PaymentTransactionLog] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

