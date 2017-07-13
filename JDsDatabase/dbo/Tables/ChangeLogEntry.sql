CREATE TABLE [dbo].[ChangeLogEntry] (
    [ChangeLogEntryId] INT            IDENTITY (1, 1) NOT NULL,
    [JobId]            INT            NULL,
    [ChangeTime]       DATETIME2 (7)  NULL,
    [Message]          NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ChangeLogEntry] PRIMARY KEY CLUSTERED ([ChangeLogEntryId] ASC),
    CONSTRAINT [Job_ChangeLogEntry] FOREIGN KEY ([JobId]) REFERENCES [dbo].[Job] ([JobId])
);



