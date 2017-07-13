CREATE TABLE [dbo].[PartFile] (
    [PartFileId]          INT             IDENTITY (1, 1) NOT NULL,
    [PartId]              INT             NULL,
    [UpdatedByEmployeeId] INT             NULL,
    [FileUrl]             NVARCHAR (MAX) NULL,
    [Notes]               NVARCHAR (MAX) NULL,
    [CreatedDate]         DATETIME2 (7)   NULL,
    [LastUpdated]         DATETIME2 (7)   NULL,
    [FileName] NVARCHAR(MAX) NULL, 
    [IsFinalPrint] BIT NULL, 
    CONSTRAINT [PK_PartFile] PRIMARY KEY CLUSTERED ([PartFileId] ASC),
    CONSTRAINT [Employee_PartFile] FOREIGN KEY ([UpdatedByEmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [Part_PartFile] FOREIGN KEY ([PartId]) REFERENCES [dbo].[Part] ([PartId])
);



