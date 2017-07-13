CREATE TABLE [dbo].[EmployeeFile] (
    [EmployeeFileId] INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]     INT            NOT NULL,
    [CreatedDate]    DATETIME2 (7)  NULL,
    [Filename]       NVARCHAR (100) NULL,
    [FileURL]        NVARCHAR (512) NULL,
    [Notes]          NVARCHAR (512) NULL,
    CONSTRAINT [PK_EmployeeFile] PRIMARY KEY CLUSTERED ([EmployeeFileId] ASC),
    CONSTRAINT [FK_EmployeeFile_Employee] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId])
);





