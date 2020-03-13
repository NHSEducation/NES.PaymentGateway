SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
CREATE TABLE [dbo].[PaymentOrder](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VendorTxCode] [varchar](20) COLLATE Latin1_General_CI_AS NOT NULL,
	[BookingID] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[CostCentre] [varchar](10) COLLATE Latin1_General_CI_AS NULL,
	[AccountCode] [varchar](10) COLLATE Latin1_General_CI_AS NULL,
	[ProjectCode] [varchar](10) COLLATE Latin1_General_CI_AS NULL,
	[VPSTxID] [varchar](100) COLLATE Latin1_General_CI_AS NULL,
	[Status] [varchar](10) COLLATE Latin1_General_CI_AS NULL,
	[StatusDetail] [varchar](max) COLLATE Latin1_General_CI_AS NULL,
	[ProcessedDate] [datetime] NULL,
 CONSTRAINT [PK_PaymentOrder] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

