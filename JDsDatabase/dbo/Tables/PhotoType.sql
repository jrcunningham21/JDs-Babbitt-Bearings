CREATE TABLE [dbo].[PhotoType] (
    [PhotoTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (40) NULL,
    CONSTRAINT [PK_PhotoType] PRIMARY KEY CLUSTERED ([PhotoTypeId] ASC)
);



