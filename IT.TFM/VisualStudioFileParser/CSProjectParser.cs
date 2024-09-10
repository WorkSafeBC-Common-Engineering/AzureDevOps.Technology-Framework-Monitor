using Parser.Interfaces;
using ProjectData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private readonly static StringBuilder packageIssuesJson = new();

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
            await RunPackageIssuesDetectionAsync(file, PackageDetectionType.Outdated.ToString().ToLower());
            await RunPackageIssuesDetectionAsync(file, PackageDetectionType.Deprecated.ToString().ToLower());
            await RunPackageIssuesDetectionAsync(file, PackageDetectionType.Vulnerable.ToString().ToLower());
        }

        private static async Task RunPackageIssuesDetectionAsync(FileItem file, string action)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "dotnet",
                Arguments = $"list {file.LocalFilePath} package --{action} --include-transitive --format json",
                RedirectStandardOutput = true,
                RedirectStandardError = false,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            packageIssuesJson.Clear();

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
                throw new InvalidProgramException($"Unable to parse {action} packages in {file.Path}.");
            }

            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
            await process.WaitForExitAsync();
            process.Close();

            Console.WriteLine(packageIssuesJson.ToString());
        }

        #endregion
    }
}
