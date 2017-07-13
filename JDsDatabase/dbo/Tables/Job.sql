CREATE TABLE [dbo].[Job] (
    [JobId]                   INT           IDENTITY (1, 1) NOT NULL,
    [CreatedByEmployeeId]     INT           NULL,
    [CustomerContactId]       INT           NULL,
    [JobStatusId]             INT           NULL,
    [CustomerJobNumber]       NVARCHAR (40) NULL,
    [PurchaseOrderNumber]     NVARCHAR (40) NULL,
    [ReceivedDate]            DATETIME2 (7) NULL,
    [RequiredDate]            DATETIME2 (7) NULL,
    [ShipByDate]              DATETIME2 (7) NULL,
    [ShippedDate]             DATETIME2 (7) NULL,
    [BilledDate]              DATETIME2 (7) NULL,
    [OvertimeRequired]        BIT           NULL,
    [HoldForCustomerApproval] BIT           NULL,
    [JobNotes]                NVARCHAR (40) NULL,
    [LastUpdated]             DATETIME2 (7) NULL,
    [QuoteOnly]               BIT           NULL,
    [AllPartsRequireUT]       BIT           NULL,
    [AllPartsRequirePT]       BIT           NULL,
    [IsActive]                BIT           NULL,
    CONSTRAINT [PK_Job] PRIMARY KEY CLUSTERED ([JobId] ASC),
    CONSTRAINT [Contact_Job] FOREIGN KEY ([CustomerContactId]) REFERENCES [dbo].[Contact] ([ContactId]),
    CONSTRAINT [Employee_Job] FOREIGN KEY ([CreatedByEmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [JobStatus_Job] FOREIGN KEY ([JobStatusId]) REFERENCES [dbo].[JobStatus] ([JobStatusId])
);











