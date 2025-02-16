CREATE TABLE [dbo].[PipelineParameters]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineId] INT NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_PipelineParameters_ToPipelines] FOREIGN KEY ([PipelineId]) REFERENCES [Pipelines]([Id])
)
