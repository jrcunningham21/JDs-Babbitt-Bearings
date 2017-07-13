CREATE TABLE [dbo].[EmployeeSkill] (
    [EmployeeId] INT NOT NULL,
    [SkillId]    INT NOT NULL,
    CONSTRAINT [PK_EmployeeSkill] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [SkillId] ASC),
    CONSTRAINT [Employee_EmployeeSkill] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [Skill_EmployeeSkill] FOREIGN KEY ([SkillId]) REFERENCES [dbo].[Skill] ([SkillId])
);



