CREATE TABLE [dbo].[Vacation] (
    [VacationId]                INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeId]                INT           NOT NULL,
    [VacationSignOffId]         INT           NULL,
    [StartDate]                 DATETIME2 (7) NOT NULL,
    [NumberOfVacationHoursUsed] INT           NOT NULL,
    [EndDate]                   DATETIME2 (7) NOT NULL,
    CONSTRAINT [PK_Vacation] PRIMARY KEY CLUSTERED ([VacationId] ASC),
    CONSTRAINT [Employee_Vacation] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId]),
    CONSTRAINT [SignOff_Vacation] FOREIGN KEY ([VacationSignOffId]) REFERENCES [dbo].[SignOff] ([SignOffId])
);





