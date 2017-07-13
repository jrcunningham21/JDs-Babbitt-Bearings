CREATE TABLE [dbo].[PartType] (
    [PartTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_PartType] PRIMARY KEY CLUSTERED ([PartTypeId] ASC)
);



