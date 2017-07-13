CREATE TABLE [dbo].[Customer] (
    [CustomerId]        INT             IDENTITY (1, 1) NOT NULL,
    [PrimaryContactId]  INT             NULL,
    [BillingAddressId]  INT             NULL,
    [ShippingAddressId] INT             NULL,
    [CompanyName]       NVARCHAR (40)   NULL,
    [Code]              NVARCHAR (40)   NULL,
    [Notes]             NVARCHAR (4000) NULL,
    [IsActive]          BIT             CONSTRAINT [DF_Customer_IsActive] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerId] ASC),
    CONSTRAINT [Address_Customer] FOREIGN KEY ([BillingAddressId]) REFERENCES [dbo].[Address] ([AddressId]),
    CONSTRAINT [Contact_Customer] FOREIGN KEY ([PrimaryContactId]) REFERENCES [dbo].[Contact] ([ContactId])
);





