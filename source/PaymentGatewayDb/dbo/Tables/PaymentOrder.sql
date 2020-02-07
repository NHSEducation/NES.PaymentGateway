CREATE TABLE [dbo].[PaymentOrder] (
    [ID]            INT              IDENTITY (1, 1) NOT NULL,
    [TokenID]       UNIQUEIDENTIFIER NOT NULL,
    [BookingID]     INT              NULL,
    [Amount]        DECIMAL (18, 2)  NULL,
    [CostCentre]    VARCHAR (10)     NULL,
    [AmountCode]    VARCHAR (10)     NULL,
    [ProjectCode]   VARCHAR (10)     NULL,
    [VPSTxID]       VARCHAR (30)     NULL,
    [Success]       BIT              NULL,
    [ProcessedDate] DATETIME         NULL,
    CONSTRAINT [PK_PaymentOrder] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (FILLFACTOR = 80)
);

