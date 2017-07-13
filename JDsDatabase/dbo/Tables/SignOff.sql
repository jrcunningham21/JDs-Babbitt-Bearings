CREATE TABLE [dbo].[SignOff] (
    [SignOffId]       INT           IDENTITY (1, 1) NOT NULL,
    [RequiredSkillId] INT           NULL,
    [EmployeeId]      INT           NULL,
    [SignOffDate]     DATETIME2 (7) NULL,
    CONSTRAINT [PK_SignOff] PRIMARY KEY CLUSTERED ([SignOffId] ASC),
    CONSTRAINT [Employee_SignOff] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [Skill_SignOff] FOREIGN KEY ([RequiredSkillId]) REFERENCES [dbo].[Skill] ([SkillId])
);



