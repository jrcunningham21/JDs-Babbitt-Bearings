CREATE TABLE [dbo].[PartProcess]
(
	[PartProcessID] INT  IDENTITY(1,1) NOT NULL , 
    [Name] VARCHAR(50) NULL
	CONSTRAINT [PK_PartProcess] PRIMARY KEY CLUSTERED ([PartProcessId] ASC),
)
