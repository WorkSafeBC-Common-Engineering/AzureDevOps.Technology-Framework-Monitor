CREATE TABLE [dbo].[Projects]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrganizationId] [int] NOT NULL,
    [ProjectId] NVARCHAR(50) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Abbreviation] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [LastUpdate] DATETIME NULL, 
    [Revision] BIGINT NOT NULL DEFAULT 0, 
    [State] NVARCHAR(50) NULL, 
    [Url] NVARCHAR(MAX) NOT NULL, 
    [Visibility] NVARCHAR(50) NULL, 
    [Deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Projects] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Projects_ToOrganization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organizations]([Id]),
)
