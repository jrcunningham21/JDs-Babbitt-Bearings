CREATE TABLE [dbo].[Address] (
    [AddressId]    INT           IDENTITY (1, 1) NOT NULL,
    [AddressLine1] NVARCHAR (40) NULL,
    [AddressLine2] NVARCHAR (40) NULL,
    [City]         NVARCHAR (40) NULL,
    [State]        NVARCHAR (40) NULL,
    [Zip]          NVARCHAR (40) NULL,
    [Country]      NVARCHAR (40) NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([AddressId] ASC)
);



