CREATE TABLE [dbo].[ReleaseArtifacts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[PipelineId] INT NOT NULL,
	[SourceId] NVARCHAR(MAX) NULL,
	[Type] NVARCHAR(MAX) NULL,
	[Alias] NVARCHAR(MAX) NULL,
	[Url]  NVARCHAR(MAX) NULL,
	[DefaultVersionType] NVARCHAR(MAX) NULL,
	[DefinitionId] NVARCHAR(MAX) NULL,
	[DefinitionName] NVARCHAR(MAX) NULL,
	[Project] NVARCHAR(MAX) NULL,
	[ProjectId] NVARCHAR(MAX) NULL,
	[IsPrimary] BIT NOT NULL DEFAULT(0),
	[IsRetained] BIT NOT NULL DEFAULT(0), 
    CONSTRAINT [FK_ReleaseArtifacts_ToPipeline] FOREIGN KEY ([PipelineId]) REFERENCES [Pipelines]([Id])
)
