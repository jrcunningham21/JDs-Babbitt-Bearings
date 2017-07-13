CREATE TABLE [dbo].[PartDiameterMeasurement] (
    [PartDiameterMeasurementId] INT             IDENTITY (1, 1) NOT NULL,
    [MeasurementListId]         INT             NULL,
    [MeasuredDate]              DATETIME2 (7)   NULL,
    [A]                         DECIMAL (10, 5) NULL,
    [B]                         DECIMAL (10, 5) NULL,
    [C]                         DECIMAL (10, 5) NULL,
    [ODComment] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_PartDiameterMeasurement] PRIMARY KEY CLUSTERED ([PartDiameterMeasurementId] ASC),
    CONSTRAINT [MeasurementList_PartDiameterMeasurement] FOREIGN KEY ([MeasurementListId]) REFERENCES [dbo].[MeasurementList] ([MeasurementListId])
);



