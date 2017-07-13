CREATE TABLE [dbo].[AutoCompleteEntry] (
    [AutoCompleteEntryId] INT              IDENTITY (1, 1) NOT NULL,
    [AssociatedTextField] UNIQUEIDENTIFIER NULL,
    [Entry]               NVARCHAR (512)   NULL,
    [IsDeleteable]        BIT              NULL,
    CONSTRAINT [PK_AutoCompleteEntry] PRIMARY KEY CLUSTERED ([AutoCompleteEntryId] ASC)
);



