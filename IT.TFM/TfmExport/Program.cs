using TfmExport;

var outputFile = "TfmData.sql";
var connection = $"{Environment.GetEnvironmentVariable("TFM_DbConnection")};TrustServerCertificate=True";

var exporter = new Exporter();

exporter.Initialize(outputFile, connection);

var clearTables = new List<TfmTable>
{
    new() { Name = "AIQueryTable", HasIdentity = false },
    new() { Name = "Reports", HasIdentity = false }
};

exporter.Clear(clearTables);

var tables = new List<TfmTable>
{
    new() { Name = "ReleaseArtifacts", HasIdentity = true },
    new() { Name = "FileProperties", HasIdentity = true },
    new() { Name = "FileReferences", HasIdentity = true },
    new() { Name = "dotNetEndOfLife", HasIdentity = false },
    new() { Name = "Pipelines", HasIdentity = true },
    new() { Name = "Files", HasIdentity = true },
    new() { Name = "NugetTargetFrameworks", HasIdentity = true },
    new() { Name = "NuGetPackages", HasIdentity = true },
    new() { Name = "NuGetFeeds", HasIdentity = true },
    new() { Name = "Repositories", HasIdentity = true },
    new() { Name = "Projects", HasIdentity = true },
    new() { Name = "Organizations", HasIdentity = true },
    new() { Name = "FilePropertyTypes", HasIdentity = false },
    new() { Name = "FileReferenceTypes", HasIdentity = false },
    new() { Name = "FileTypes", HasIdentity = false },
    new() { Name = "PackageIssues", HasIdentity = true },
    new() { Name = "ScannerTypes", HasIdentity = false },
    new() { Name = "FrameworkProducts", HasIdentity = true },
};

exporter.Run(tables);

exporter.Close();