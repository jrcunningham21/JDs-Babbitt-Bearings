CREATE TABLE [dbo].[DeliveryInfo] (
    [DeliveryInfoId]    INT           IDENTITY (1, 1) NOT NULL,
    [ShippedVia]        NVARCHAR (40) NULL,
    [DateShipped]       DATETIME2 (7) NULL,
    [DateRequired]      DATETIME2 (7) NULL,
    [TrackingNumber]    NVARCHAR (80) NULL,
    [PackedBySignOffId] INT           NULL,
    CONSTRAINT [PK_DeliveryInfo] PRIMARY KEY CLUSTERED ([DeliveryInfoId] ASC),
    CONSTRAINT [SignOff_DeliveryInfo] FOREIGN KEY ([PackedBySignOffId]) REFERENCES [dbo].[SignOff] ([SignOffId])
);



