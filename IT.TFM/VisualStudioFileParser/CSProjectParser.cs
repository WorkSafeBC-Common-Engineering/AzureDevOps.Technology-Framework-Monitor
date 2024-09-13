using Parser.Interfaces;

using ProjectData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

using VisualStudioFileParser.Models;

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
            var path = await GetProjectPathAsync(file);

            if (await RestorePackagesAsync(file, path))
            {
                await RunPackageIssuesDetectionAsync(file, path, PackageDetectionType.Outdated);
                await RunPackageIssuesDetectionAsync(file, path, PackageDetectionType.Deprecated);
                await RunPackageIssuesDetectionAsync(file, path, PackageDetectionType.Vulnerable);
            }

            CleanupTempProject();
        }

        private static async Task<string> GetProjectPathAsync(FileItem file)
        {
            var directory = Path.GetDirectoryName(file.LocalFilePath);
            var configFile = Path.Combine(directory, "packages.config");
            if (File.Exists(configFile))
            {
                var path = Path.Combine(Environment.CurrentDirectory, "wcbbc/tempProject");
                await CreateTempProject(path);

                var xmlDoc = new XmlDocument();
                xmlDoc.Load(configFile);

                var nodes = xmlDoc.SelectNodes("/packages/package");
                foreach ( XmlNode node in nodes)
                {
                    var packageId = node.Attributes["id"].Value;
                    var packageVersion = node.Attributes["version"].Value;
                    string framework = node.Attributes["framework"]?.Value ?? string.Empty;
                    if (framework != string.Empty)
                    {
                        framework = $"--framework {framework}";
                    }

                    await AddPackage(path, packageId, packageVersion, framework);
                    // dotnet add $path package $packageId --version $packageVersion $framework
                }

                return Path.Combine(path, "tempProject.csproj");
            }

            return file.LocalFilePath;
        }

        private static async Task CreateTempProject(string projectPath)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"new console -o {projectPath}",
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using var process = new Process();
            process.StartInfo = startInfo;

            if (!process.Start())
            {
                throw new InvalidProgramException($"Unable to create console app in {projectPath}.");
            }

            await process.WaitForExitAsync();

            var returnCode = process.ExitCode;
            process.Close();

            if (returnCode != 0)
            {
                throw new InvalidProgramException("Unable to create temp project for package scanning");
            }
        }

        private static async Task AddPackage(string projectPath, string packageId, string packageVersion, string framework)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"add {projectPath} package {packageId} --version {packageVersion} --no-restore {framework}",
                UseShellExecute = false,
                CreateNoWindow = false
            };

            using var process = new Process();
            process.StartInfo = startInfo;

            if (!process.Start())
            {
                throw new InvalidProgramException($"Unable to add package {packageVersion}.{packageVersion} to project in {projectPath}.");
            }

            await process.WaitForExitAsync();

            var returnCode = process.ExitCode;
            process.Close();

            if (returnCode != 0)
            {
                throw new InvalidProgramException($"Unable to add package {packageVersion}.{packageVersion} to project in {projectPath}.");
            }
        }

        private static void CleanupTempProject()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "wcbbc/tempProject");

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private static async Task<bool> RestorePackagesAsync(FileItem file, string projectPath)
        {
            //ProcessStartInfo startInfo = new()
            //{
            //    FileName = "dotnet",
            //    Arguments = $"restore {projectPath} --source \"https://api.nuget.org/v3/index.json\" --source \"https://wcbbc.pkgs.visualstudio.com/_packaging/WSBC_Cloud_NuGet/nuget/v3/index.json\" -v diag",
            //    UseShellExecute = false,
            //    CreateNoWindow = false
            //};

            ProcessStartInfo startInfo = new()
            {
                FileName = "nuget",
                Arguments = $"restore {projectPath} -SolutionDirectory {Path.Combine(Environment.CurrentDirectory, "wcbbc")}",
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

            var returnCode = process.ExitCode;
            process.Close();

            if (returnCode != 0)
            {
                file.PackageReferencesIssues.Add(new PackageReferenceIssue
                {
                    Id = 0,
                    ScanType = $"Restore Error: {returnCode}",
                    Framework = string.Empty,
                    IsTopLevel = true,
                    PackageName = string.Empty
                });

                return false;
            }

            return true;
        }

        private static async Task RunPackageIssuesDetectionAsync(FileItem file, string projectPath, PackageDetectionType actionType)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"list {projectPath} package --{actionType.ToString().ToLower()} --include-transitive --source \"https://api.nuget.org/v3/index.json\" --format json",
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
                if (packageInfo.Projects == null)
                {
                    return;
                }

                foreach (var project in packageInfo.Projects)
                {
                    if (project.Frameworks == null)
                    {
                        continue;
                    }

                    foreach (var framework in project.Frameworks)
                    {
                        if (framework.TopLevelPackages != null)
                        {
                            foreach (var topPackage in  framework.TopLevelPackages)
                            {
                                file.PackageReferencesIssues.Add(new PackageReferenceIssue
                                {
                                    Id = 0,
                                    ScanType = action.ToString(),
                                    Framework = framework.FrameworkVersion,
                                    IsTopLevel = true,
                                    PackageName = topPackage.Id,
                                    RequestedVersion = topPackage.RequestedVersion,
                                    ResolvedVersion = topPackage.ResolvedVersion,
                                    LatestVersion = topPackage.LatestVersion,
                                    Severity = (topPackage.Vulnerabilities == null || topPackage.Vulnerabilities.Length == 0) ? null : topPackage.Vulnerabilities[0].Severity,
                                    AdvisoryUrl = (topPackage.Vulnerabilities == null || topPackage.Vulnerabilities.Length == 0) ? null : topPackage.Vulnerabilities[0].AdvisoryUrl
                                });
                            }
                        }

                        if (framework.TransitivePackages != null)
                        {
                            foreach (var transitivePackage in framework.TransitivePackages)
                            {
                                file.PackageReferencesIssues.Add(new PackageReferenceIssue
                                {
                                    Id = 0,
                                    ScanType = action.ToString(),
                                    Framework = framework.FrameworkVersion,
                                    IsTopLevel = false,
                                    PackageName = transitivePackage.Id,
                                    RequestedVersion = transitivePackage.RequestedVersion,
                                    ResolvedVersion = transitivePackage.ResolvedVersion,
                                    LatestVersion = transitivePackage.LatestVersion,
                                    Severity = (transitivePackage.Vulnerabilities == null || transitivePackage.Vulnerabilities.Length == 0) ? null : transitivePackage.Vulnerabilities[0].Severity,
                                    AdvisoryUrl = (transitivePackage.Vulnerabilities == null || transitivePackage.Vulnerabilities.Length == 0) ? null : transitivePackage.Vulnerabilities[0].AdvisoryUrl
                                });
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
