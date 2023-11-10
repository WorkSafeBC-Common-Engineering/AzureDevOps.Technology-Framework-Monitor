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

        #endregion

        #region IFileParser Implementation

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
                var fields = cleanDependency.Split(new char[] { ':' });
                if (fields.Length != 2)
                {
                    dependency = dependency.Next;
                    continue;
                }

                var id = fields[0].Trim();
                var version = fields[1].Trim(); ;
                
                var comparator = version.Substring(0, 1);
                
                // if we have a ^ or ~ in the version, strip it
                if (comparator == "^" || comparator == "~")
                {
                    version = version.Substring(1).Trim();
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
