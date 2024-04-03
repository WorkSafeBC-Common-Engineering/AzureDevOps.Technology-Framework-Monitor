using Newtonsoft.Json.Linq;
using Parser.Interfaces;
using ProjectData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpmFileParser
{
    public class PackageParser : IFileParser
    {
        #region Private Members

        private const string jsonJoinChar = " ";
        private static readonly char[] separator = [':'];

        private const string dependenciesProperty = "dependencies";
        private const string devDependenciesProperty = "devDependencies";

        private const string npmPackageType = "NPM";
        private const string npmDevPackageType = "NPM-DEV";

        #endregion

        #region IFileParser Implementation

        void IFileParser.Initialize(object data)
        {
            // no op
        }

        void IFileParser.Parse(FileItem file, string[] content)
        {
            var fileContents = string.Join(jsonJoinChar, content);
            var root = JObject.Parse(fileContents);

            ParseDependencies(file, root, dependenciesProperty, npmPackageType);

            ParseDependencies(file, root, devDependenciesProperty, npmDevPackageType);
        }

        #endregion

        #region Private Methods

        private static void ParseDependencies(FileItem file, JObject root, string dependenceTag, string packageType)
        {
            var dependencies = root[dependenceTag];
            if (dependencies == null)
            {
                return;
            }

            var dependency = dependencies.First;
            while (dependency != null)
            {
                var dependencyString = dependency.ToString();
                var cleanDependency = dependencyString.Replace("\"", string.Empty)
                                                      .Replace(@"://", "-");
                var fields = cleanDependency.Split(separator);
                if (fields.Length != 2)
                {
                    dependency = dependency.Next;
                    continue;
                }

                var id = fields[0].Trim();
                var version = fields[1].Trim();

                var comparator = version[..1];

                // if we have a ^ or ~ in the version, strip it
                if (comparator == "^" || comparator == "~")
                {
                    version = version[1..].Trim();
                }
                else
                {
                    comparator = string.Empty;
                }

                file.AddPackageReference(packageType, id, $"{version}", string.Empty, comparator);

                dependency = dependency.Next;
            }
        }

        #endregion
    }
}
