using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VisualStudioFileParser
{
    internal enum PackageDetectionType
    {
        Outdated,
        Deprecated,
        Vulnerable
    }

    class CSProjectParser : Parser.ParseXmlFile, IFileParser
    {
        #region Private Members

        private readonly static JsonSerializerOptions serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            buildProperties = data as Dictionary<string, string>;
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var xmlDoc = GetAsXml(content);
            var rootNode = xmlDoc.DocumentElement;

            if (rootNode == null || !rootNode.Name.Equals(xmlProject, StringComparison.InvariantCultureIgnoreCase))
            {
                InvalidVSProject(file);
                return;
            }

            var value = rootNode.Attributes[xmlAttrToolsVer]?.Value;
            if (value != null)
            {
                file.AddProperty(propertyToolsVersion, value);
            }

            value = rootNode.Attributes[xmlAttrSdk]?.Value;
            if (value != null)
            {
                file.AddProperty(propertySdk, value);
            }

            WriteNonNullNodeProperty(rootNode, xmlOutputType, file, propertyOutputType);
            WriteNonNullNodeProperty(rootNode, xmlTargetFrameworkVersion, file, propertyTargetFWVer);
            WriteNonNullNodeProperty(rootNode, xmlTargetFramework, file, propertyTargetFW);
            WriteNonNullNodeProperty(rootNode, xmlAzureFunction, file, propertyAzureFunction);

            WriteTrueNodeProperty(rootNode, xmlAndroidApp, file, propertyIsAndroid, "Yes");
            WriteIfExistsProperty(rootNode, xmliOSApp, file, propertyIsiOS, "Yes");

            WriteVSProjectReferences(rootNode, xmlReferences, file);
            WriteVSProjectPackageReference(rootNode, xmlPkgReference, file);

            CleanupReferences(file);

            FindPackageIssuesAsync(file).Wait();
        }

        #endregion

        #region Private Methods

        // For the File References, we may have duplicates - some from NuGet package.configs, some from different areas of a Project file
        // We want to look at each set of these, and determine if there are duplicates. If so, we will clean up these references before saving.

        private static void CleanupReferences(FileItem file)
        {
            var packageReferences = file.PackageReferences
                                        .Where(r => r.PackageType == "Project")
                                        .OrderBy(r => r.Id)
                                        .ThenBy(r => r.Version)
                                        .ToArray();

            var referenceCount = packageReferences.Length;
            for (int index = 0; index < referenceCount; index++)
            {
                var reference = packageReferences[index];

                var fileReferences = file.References
                                        .Where(r => r.Equals(reference.Id, StringComparison.InvariantCultureIgnoreCase))
                                        .ToArray();

                foreach (var item in fileReferences)
                {
                    file.References.Remove(item);
                }
            }
        }

        private static async Task FindPackageIssuesAsync(FileItem file)
        {
            await ProcessPackageIssues(file, PackageDetectionType.Outdated);
            await ProcessPackageIssues(file, PackageDetectionType.Deprecated);
            await ProcessPackageIssues(file, PackageDetectionType.Vulnerable);
        }

        private static async Task ProcessPackageIssues(FileItem file, PackageDetectionType actionType)
        {
            await RestorePackagesAsync(file);
            await RunPackageIssuesDetectionAsync(file, PackageDetectionType.Outdated);
        }

        private static async Task RestorePackagesAsync(FileItem file)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"restore {file.LocalFilePath}",
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using var process = new Process();
            process.StartInfo = startInfo;

            if (!process.Start())
            {
                throw new InvalidProgramException($"Unable to restore packages in {file.Path}.");
            }

            await process.WaitForExitAsync();
            process.Close();
        }

        private static async Task RunPackageIssuesDetectionAsync(FileItem file, PackageDetectionType actionType)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"list {file.LocalFilePath} package --{actionType.ToString().ToLower()} --include-transitive --format json",
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var packageIssuesJson = new StringBuilder();

            using var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    packageIssuesJson.AppendLine(args.Data);
                }
            };


            if (!process.Start())
            {
                throw new InvalidProgramException($"Unable to parse {actionType.ToString().ToLower()} packages in {file.Path}.");
            }

            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            await process.WaitForExitAsync();
            process.Close();

            Console.WriteLine(packageIssuesJson.ToString());
            await ProcessPackageIssuesAsync(file, actionType, packageIssuesJson.ToString());
        }

        private static async Task ProcessPackageIssuesAsync(FileItem file, PackageDetectionType action, string output)
        {
            var jsonData = Encoding.UTF8.GetBytes(output);
            using var stream = new MemoryStream(jsonData);
            var packageInfo = await JsonSerializer.DeserializeAsync<PackageIssuesInstance>(stream, serializerOptions);

            if ((packageInfo?.Problems?.Length ?? 0) > 0)
            {
                var problem = packageInfo.Problems[0];
                Console.WriteLine($"Problem: Level={problem.Level}, {problem.Text}\n{problem.Project}");
            }
            else
            {
                Console.WriteLine("Valid {action}");
            }
        }

        #endregion
    }


    public class PackageIssuesInstance
    {
        public int Version { get; set; }
        public string Parameters { get; set; }
        public Problem[] Problems { get; set; }
        public string[] Sources { get; set; }
        public Project[] Projects { get; set; }
    }

    public class Problem
    {
        public string Project { get; set; }
        public string Level { get; set; }
        public string Text { get; set; }
    }

    public class Project
    {
        public string Path { get; set; }

        public Framework[] Frameworks { get; set; }
    }

    public class Framework
    {
        [JsonPropertyName("framework")]
        public string FrameworkVersion { get; set; }
        public Toplevelpackage[] TopLevelPackages { get; set; }
        public Transitivepackage[] TransitivePackages { get; set; }
    }

    public class Toplevelpackage
    {
        public string Id { get; set; }
        public string RequestedVersion { get; set; }
        public string ResolvedVersion { get; set; }
        public string LatestVersion { get; set; }
    }

    public class Transitivepackage
    {
        public string Id { get; set; }
        public string ResolvedVersion { get; set; }
        public string LatestVersion { get; set; }
    }

}
