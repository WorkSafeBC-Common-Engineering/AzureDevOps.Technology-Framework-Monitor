CREATE TABLE [dbo].[Organizations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScannerTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Uri] [nvarchar](max) NOT NULL,
    CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED ([Id] ASC), 
    CONSTRAINT [FK_Organization_ToScannerType] FOREIGN KEY ([ScannerTypeId]) REFERENCES [dbo].[ScannerTypes]([Id])
);