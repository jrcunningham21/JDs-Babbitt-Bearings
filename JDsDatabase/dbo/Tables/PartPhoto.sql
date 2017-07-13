CREATE TABLE [dbo].[PartPhoto] (
    [PartPhotoId]            INT           IDENTITY (1, 1) NOT NULL,
    [PartId]                 INT           NULL,
    [PhotoTakenByEmployeeId] INT           NULL,
    [PhotoTypeId]            INT           NULL,
    [PhotoUrl]               NVARCHAR (MAX) NULL,
    [Notes]                  NVARCHAR (MAX) NULL,
    [DateTaken]              DATETIME2 (7) NULL,
    CONSTRAINT [PK_PartPhoto] PRIMARY KEY CLUSTERED ([PartPhotoId] ASC),
    CONSTRAINT [Employee_PartPhoto] FOREIGN KEY ([PhotoTakenByEmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [Part_PartPhoto] FOREIGN KEY ([PartId]) REFERENCES [dbo].[Part] ([PartId]),
    CONSTRAINT [PhotoType_PartPhoto] FOREIGN KEY ([PhotoTypeId]) REFERENCES [dbo].[PhotoType] ([PhotoTypeId])
);



