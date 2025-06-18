CREATE TABLE [dbo].[ProjectMetrics]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FileId] INT NOT NULL, 
    [MaintainabilityIndex] INT NOT NULL, 
    [ClassCoupling] INT NOT NULL,
    [CyclomaticComplexity] INT NOT NULL, 
    [DepthOfInheritance] INT NOT NULL, 
    [SourceLines] INT NOT NULL, 
    [ExecutableLines] INT NOT NULL,
    [UnitTestCodeCoverage] TINYINT NOT NULL DEFAULT (0),
    CONSTRAINT [FK_ProjectMetrics_ToFiles] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id])
)
