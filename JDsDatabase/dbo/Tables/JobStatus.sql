CREATE TABLE [dbo].[JobStatus] (
    [JobStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (40) NULL,
    CONSTRAINT [PK_JobStatus] PRIMARY KEY CLUSTERED ([JobStatusId] ASC)
);



