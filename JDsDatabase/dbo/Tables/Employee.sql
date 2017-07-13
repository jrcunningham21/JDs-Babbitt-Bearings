CREATE TABLE [dbo].[Employee] (
    [EmployeeId]          INT             IDENTITY (1, 1) NOT NULL,
    [AddressId]           INT             NULL,
    [Name]                NVARCHAR (40)   NOT NULL,
    [Email]               NVARCHAR (40)   NULL,
    [Phone]               NVARCHAR (40)   NULL,
    [HireDate]            DATETIME2 (7)   NULL,
    [PIN]                 NVARCHAR (4)    NOT NULL,
    [EmergencyContact]    NVARCHAR (200)  NULL,
    [EmergencyPhone]      NVARCHAR (40)   NULL,
    [Notes]               NVARCHAR (1024) NULL,
    [IsActive]            BIT             CONSTRAINT [DF_Employee_IsActive] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([EmployeeId] ASC),
    CONSTRAINT [Address_Employee] FOREIGN KEY ([AddressId]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [IX_EmployeePIN] UNIQUE NONCLUSTERED ([PIN] ASC)
);











