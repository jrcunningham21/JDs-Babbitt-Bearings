CREATE TABLE [dbo].[Certificate] (
    [CertificateId]      INT            IDENTITY (1, 1) NOT NULL,
    [CertificateDate]    NVARCHAR (50)  NOT NULL,
    [CertificateExpires] NVARCHAR (50)  NULL,
    [Notes]              NVARCHAR (512) NULL,
    [CompanyName]        VARCHAR (80)   NULL,
    [Name]               VARCHAR (40)   NULL,
    [CertificateFileId]  INT            NULL,
    CONSTRAINT [PK_Certificate] PRIMARY KEY CLUSTERED ([CertificateId] ASC),
    CONSTRAINT [CertificateFile_Certificate] FOREIGN KEY ([CertificateFileId]) REFERENCES [dbo].[CertificateFile] ([CertificateFileId])
);





