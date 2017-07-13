CREATE TABLE [dbo].[Skill] (
    [SkillId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]    NVARCHAR (40) NOT NULL,
    CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED ([SkillId] ASC)
);



