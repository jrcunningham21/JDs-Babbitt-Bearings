CREATE TABLE [dbo].[TimesheetEntry] (
    [TimesheetEntryId]      INT            IDENTITY (1, 1) NOT NULL,
    [EmployeeId]            INT            NULL,
    [Day]                   DATETIME2 (7)  NOT NULL,
    [NumHoursWorked]        DECIMAL (6, 3) NOT NULL,
    [StartTime]             DATETIME2 (7)  NULL,
    [EndTime]               DATETIME2 (7)  NULL,
    [WorkedThroughMealtime] BIT            NULL,
    [TotalHours]            DECIMAL (6, 3) NULL,
    [StraightTime]          DECIMAL (6, 3) NULL,
    [OverTime]              DECIMAL (6, 3) NULL,
    [DoubleTime]            DECIMAL (6, 3) NULL,
    CONSTRAINT [PK_TimesheetEntry] PRIMARY KEY CLUSTERED ([TimesheetEntryId] ASC),
    CONSTRAINT [Employee_TimesheetEntry] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employee] ([EmployeeId])
);





