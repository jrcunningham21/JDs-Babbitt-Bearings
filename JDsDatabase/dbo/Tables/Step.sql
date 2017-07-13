CREATE TABLE [dbo].[Step] (
    [StepId]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [StepNumber]      INT            NOT NULL,
    [ProcessId]       BIGINT         NOT NULL,
    [Title]           NVARCHAR (200) NOT NULL,
    [Version]         NVARCHAR (100) NULL,
    [DataType]        NVARCHAR (100) NULL,
    [StringValue]     NVARCHAR (MAX) NULL,
    [Notes]           NVARCHAR (MAX) NULL,
    [LastRequestedBy] NVARCHAR (200) NULL,
    [Requested]       DATETIME       NULL,
    [Created]         DATETIME       NOT NULL,
    [Updated]         DATETIME       NULL,
    CONSTRAINT [PK_StepData] PRIMARY KEY CLUSTERED ([StepId] ASC),
    CONSTRAINT [FK_Step_Process] FOREIGN KEY ([ProcessId]) REFERENCES [dbo].[Process] ([ProcessId])
);







