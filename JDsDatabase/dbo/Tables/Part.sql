CREATE TABLE [dbo].[Part] (
    [PartId]                    INT             IDENTITY (1, 1) NOT NULL,
    [JobId]                     INT             NULL,
    [PartTypeId]                INT             NULL,
    [PartStatusId]              INT             NULL,
    [PartContactId]             INT             NULL,
    [WorkScope]                 NVARCHAR (2048) NULL,
    [RequiredDate]              DATETIME2 (7)   NULL,
    [ShipByDate]                DATETIME2 (7)   NULL,
    [ShippedDate]               DATETIME2 (7)   NULL,
    [NonConformanceReportNotes] NVARCHAR (2048) NULL,
    [RequiresPT]                BIT             NULL,
    [RequiresUT]                BIT             NULL,
    [PartProcessId]             INT             NULL,
    [ItemCode]                  VARCHAR (MAX)    NULL,
    [IdentifyingInfo]           VARCHAR (MAX)   NULL,
    [IsActive]                  BIT             CONSTRAINT [DF_Part_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Part] PRIMARY KEY CLUSTERED ([PartId] ASC),
    CONSTRAINT [Contact_Part] FOREIGN KEY ([PartContactId]) REFERENCES [dbo].[Contact] ([ContactId]),
    CONSTRAINT [Job_Part] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job] ([JobId]),
    CONSTRAINT [PartProcess_Part] FOREIGN KEY ([PartProcessId]) REFERENCES [dbo].[PartProcess] ([PartProcessID]),
    CONSTRAINT [PartStatus_Part] FOREIGN KEY ([PartStatusId]) REFERENCES [dbo].[PartStatus] ([PartStatusId]),
    CONSTRAINT [PartType_Part] FOREIGN KEY ([PartTypeId]) REFERENCES [dbo].[PartType] ([PartTypeId])
);





