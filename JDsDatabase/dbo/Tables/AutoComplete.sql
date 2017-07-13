CREATE TABLE [dbo].[AutoComplete] (
    [AutoCompleteId] INT        IDENTITY (1, 1) NOT NULL,
    [ControlId]      INT        NOT NULL,
    [Value]          NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_AutoComplete] PRIMARY KEY CLUSTERED ([AutoCompleteId] ASC)
);

