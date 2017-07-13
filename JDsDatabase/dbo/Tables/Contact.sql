CREATE TABLE [dbo].[Contact] (
    [ContactId]  INT             IDENTITY (1, 1) NOT NULL,
    [CustomerId] INT             NOT NULL,
    [FirstName]  NVARCHAR (40)   NULL,
    [LastName]   NVARCHAR (40)   NULL,
    [Email]      NVARCHAR (200)  NULL,
    [WorkPhone]  NVARCHAR (40)   NULL,
    [CellPhone]  NVARCHAR (40)   NULL,
    [Fax]        NVARCHAR (40)   NULL,
    [Notes]      NVARCHAR (2000) NULL,
    [IsActive]   BIT             CONSTRAINT [DF_Contact_IsActive] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([ContactId] ASC),
    CONSTRAINT [Customer_Contact] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([CustomerId])
);







