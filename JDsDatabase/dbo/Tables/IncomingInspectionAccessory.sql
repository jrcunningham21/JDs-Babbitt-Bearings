CREATE TABLE [dbo].[IncomingInspectionAccessory] (
    [IncomingInspectionAccessoryId] INT           IDENTITY (1, 1) NOT NULL,
    [PartId]                        INT           NULL,
    [Name]                          NVARCHAR (80) NULL,
    [Quantity]                      INT           NULL,
    [IsInstalled]                   BIT           CONSTRAINT [DF_IncomingInspectionAccessory_IsInstalled] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IncomingInspectionAccessory] PRIMARY KEY CLUSTERED ([IncomingInspectionAccessoryId] ASC),
    CONSTRAINT [Part_IncomingInspectionAccessory] FOREIGN KEY ([PartId]) REFERENCES [dbo].[Part] ([PartId])
);





