CREATE TABLE [dbo].[CertificateFile] (
    [CertificateFileId] INT            IDENTITY (1, 1) NOT NULL,
    [CreatedDate]       DATETIME2 (7)  NULL,
    [Filename]          NVARCHAR (100) NULL,
    [FileURL]           NVARCHAR (512) NULL,
    [OriginalFilename]  NVARCHAR (100) NULL,
    [Notes]             NVARCHAR (512) NULL,
    CONSTRAINT [PK_CertificateFile] PRIMARY KEY CLUSTERED ([CertificateFileId] ASC)
);





