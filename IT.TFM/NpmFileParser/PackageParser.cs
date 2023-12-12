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
        private const string dependenciesProperty = "dependencies";
        private const string npmPackageType = "NPM";
        private static readonly char[] separator = [':'];

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

            var dependencies = root[dependenciesProperty];
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
                var version = fields[1].Trim(); ;
                
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

                file.AddPackageReference(npmPackageType, id, $"{version}", string.Empty, comparator);

                dependency = dependency.Next;
            }
        }

        #endregion
    }
}
