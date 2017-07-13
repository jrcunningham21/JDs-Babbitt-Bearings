CREATE TABLE [dbo].[Activity] (
    [ActivityId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]       NVARCHAR (200) NOT NULL,
    [IsActive]   BIT            CONSTRAINT [DF_Activity_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([ActivityId] ASC)
);

