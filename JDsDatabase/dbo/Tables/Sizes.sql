CREATE TABLE [dbo].[Sizes] (
    [SizesId]                            INT           IDENTITY (1, 1) NOT NULL,
    [PartId]                             INT           NULL,
    [Shaft]                              NVARCHAR (128) NULL,
    [Clearance]                          NVARCHAR (128) NULL,
    [BoreSize]                           NVARCHAR (128) NULL,
    [BoreSizeHorizontal]                 NVARCHAR (128) NULL,
    [ShimSize]                           NVARCHAR (128) NULL,
    [Tolerance]                          NVARCHAR (128) NULL,
    [SealSize]                           NVARCHAR (128) NULL,
    [SpecialNotes]                       VARCHAR (MAX) NULL,
    [CustomerSuppliedMeasurementsListId] INT           NULL,
    CONSTRAINT [PK_Sizes] PRIMARY KEY CLUSTERED ([SizesId] ASC)
);


