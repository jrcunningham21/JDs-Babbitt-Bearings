CREATE TABLE [dbo].[PartStatus] (
    [PartStatusId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (40) NULL,
    CONSTRAINT [PK_PartStatus] PRIMARY KEY CLUSTERED ([PartStatusId] ASC)
);



