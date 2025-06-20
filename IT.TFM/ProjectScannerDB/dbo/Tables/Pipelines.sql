﻿CREATE TABLE [dbo].[Pipelines]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PipelineId] INT NULL, 
    [ProjectId] INT NULL,
    [RepositoryId] INT NULL, 
    [FileId] INT NULL,
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Folder] NVARCHAR(MAX) NOT NULL, 
    [Revision] INT NOT NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [Type] NVARCHAR(20) NOT NULL, 
    [PipelineType] NVARCHAR(20) NULL, 
    [Path] NVARCHAR(MAX) NULL,
    [YamlType] NVARCHAR(20) NULL,
    [Portfolio] NVARCHAR(MAX) NULL,
    [Product] NVARCHAR(MAX) NULL,
    [BlueprintApplicationTypeId] INT NULL,
    [Source] NVARCHAR(MAX) NULL,
    [CreatedByName] NVARCHAR(MAX) NULL,
    [CreatedById] NVARCHAR(MAX) NULL,
    [CreatedDateTime] DATETIME NULL,
    [ModifiedByName] NVARCHAR(MAX) NULL,
    [ModifiedById] NVARCHAR(MAX) NULL,
    [ModifiedDateTime] DATETIME NULL,
    [IsDeleted] BIT NOT NULL DEFAULT(0),
    [IsDisabled] BIT NOT NULL DEFAULT(0),
    [SuppressCD] BIT NOT NULL DEFAULT(1),
    [LastReleaseId] INT NULL,
    [LastReleaseName] NVARCHAR(MAX) NULL,
    [Environments] NVARCHAR(MAX) NULL,
    [State] NVARCHAR(20) NULL,
    [Result] NVARCHAR(20) NULL,
    [RunId] int NULL,
    [LastRunUrl] NVARCHAR(MAX) NULL,
    [LastRunStart] DATETIME NULL,
    [LastRunEnd] DATETIME NULL,
    CONSTRAINT [FK_Pipelines_ToRepository] FOREIGN KEY ([RepositoryId]) REFERENCES Repositories([Id]), 
    CONSTRAINT [FK_Pipelines_ToFile] FOREIGN KEY ([FileId]) REFERENCES [Files]([Id]), 
    CONSTRAINT [FK_Pipelines_ToPipelineTypes] FOREIGN KEY ([BlueprintApplicationTypeId]) REFERENCES [PipelineTypes]([Id])
)
