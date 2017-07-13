CREATE TABLE [dbo].[ProcessType] (
    [ProcessTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (200) NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [IsActive]      BIT            NOT NULL,
    CONSTRAINT [PK_ProcessType] PRIMARY KEY CLUSTERED ([ProcessTypeId] ASC)
);

