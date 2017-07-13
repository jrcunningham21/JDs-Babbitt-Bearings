CREATE TABLE [dbo].[Process] (
    [ProcessId]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProcessTypeId] INT            NOT NULL,
    [PartId]        INT            NOT NULL,
    [Name]          NVARCHAR (200) NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [Notes]         NVARCHAR (MAX) NULL,
    [IsActive]      BIT            CONSTRAINT [DF_Process_IsActive] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Process] PRIMARY KEY CLUSTERED ([ProcessId] ASC),
    CONSTRAINT [FK_Process_Part] FOREIGN KEY ([PartId]) REFERENCES [dbo].[Part] ([PartId]),
    CONSTRAINT [FK_Process_ProcessType] FOREIGN KEY ([ProcessTypeId]) REFERENCES [dbo].[ProcessType] ([ProcessTypeId])
);





