CREATE TABLE [dbo].[StepActivity] (
    [StepActivityId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [StepId]         BIGINT         NOT NULL,
    [ActivityId]     INT            NOT NULL,
    [Employee]       NVARCHAR (100) NULL,
    [Manager]        NVARCHAR (100) NULL,
    [Created]        DATETIME       NOT NULL,
    CONSTRAINT [PK_StepActivity] PRIMARY KEY CLUSTERED ([StepActivityId] ASC),
    CONSTRAINT [FK_StepActivity_Activity] FOREIGN KEY ([ActivityId]) REFERENCES [dbo].[Activity] ([ActivityId]),
    CONSTRAINT [FK_StepActivity_Step] FOREIGN KEY ([StepId]) REFERENCES [dbo].[Step] ([StepId])
);

