using ProjectData;
using ProjectData.Interfaces;

using PythonFileParser;

using Moq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PythonVersionUnitTests
{
    internal static class PythonVersionTestDataSeeder
    {
        private static readonly object _seedLock = new();
        private static bool _isSeeded;

        internal static void SeedPythonVersions()
        {
            lock (_seedLock)
            {
                if (_isSeeded)
                {
                    return;
                }

                SeedPythonVersionsInternal();
                _isSeeded = true;
            }
        }

        private static void SeedPythonVersionsInternal()
        {
            var mockReader = new Mock<IStorageReader>();
            mockReader.Setup(reader => reader.GetEolVersions()).Returns(new List<EolVersion>
            {
                new() { Version = "python 3.9", EolDate = new DateOnly(2025, 10, 31) },
                new() { Version = "python 3.10", EolDate = new DateOnly(2026, 10, 31) },
                new() { Version = "python 3.11", EolDate = new DateOnly(2027, 10, 31) },
                new() { Version = "python 3.12", EolDate = new DateOnly(2028, 10, 31) }
            });

            PopulatePythonCommonCache(mockReader.Object);
        }

        private static void PopulatePythonCommonCache(IStorageReader storageReader)
        {
            var assembly = typeof(PythonProjectTomlParser).Assembly;
            var pythonCommonType = assembly.GetType("PythonFileParser.PythonCommon")
                ?? throw new InvalidOperationException("Unable to find PythonCommon type.");
            var pythonVersionType = assembly.GetType("PythonFileParser.PythonVersion")
                ?? throw new InvalidOperationException("Unable to find PythonVersion type.");

            var pythonVersionsField = pythonCommonType.GetField("_pythonVersions", BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new InvalidOperationException("Unable to find PythonCommon._pythonVersions field.");

            if (pythonVersionsField.GetValue(null) is not IDictionary pythonVersionsDictionary)
            {
                throw new InvalidOperationException("Unable to access PythonCommon._pythonVersions dictionary.");
            }

            pythonVersionsDictionary.Clear();

            var versionProperty = pythonVersionType.GetProperty("Version")
                ?? throw new InvalidOperationException("Unable to find PythonVersion.Version property.");
            var eolDateProperty = pythonVersionType.GetProperty("EolDate")
                ?? throw new InvalidOperationException("Unable to find PythonVersion.EolDate property.");

            foreach (var eolVersion in storageReader.GetEolVersions().Where(v => v.Version.StartsWith("python", StringComparison.OrdinalIgnoreCase)))
            {
                var pythonVersionInstance = Activator.CreateInstance(pythonVersionType)
                    ?? throw new InvalidOperationException("Unable to create PythonVersion instance.");

                versionProperty.SetValue(pythonVersionInstance, eolVersion.Version);
                eolDateProperty.SetValue(pythonVersionInstance, eolVersion.EolDate);

                pythonVersionsDictionary.Add(eolVersion.Version, pythonVersionInstance);
            }
        }
    }
}
