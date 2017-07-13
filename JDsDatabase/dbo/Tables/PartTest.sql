CREATE TABLE [dbo].[PartTest]
(
	[PartTestId]                   INT           IDENTITY (1, 1) NOT NULL,
    [PartId] INT NOT NULL,

	[UTComplete] BIT NULL, 
    [PTComplete] BIT NULL, 
    [UTCertPartFileId] INT NULL, 
    [PTCertPartFileId] INT NULL, 
    [UTSignoffEmpName] NVARCHAR(MAX) NULL, 
    [PTSignoffEmpName] NVARCHAR(MAX) NULL, 
	[UTPassed] BIT NULL, 
    [PTPassed] BIT NULL, 
    [TestNotes] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PartTestId] PRIMARY KEY CLUSTERED ([PartTestId] ASC),
    CONSTRAINT [PartTest_Part] FOREIGN KEY ([PartId]) REFERENCES [dbo].[Part] ([PartId]),
    CONSTRAINT [PartTest_PartFile_UT] FOREIGN KEY ([UTCertPartFileId]) REFERENCES [dbo].[PartFile] ([PartFileId]),
    CONSTRAINT [PartTest_PartFile_PT] FOREIGN KEY ([PTCertPartFileId]) REFERENCES [dbo].[PartFile] ([PartFileId]),
)
