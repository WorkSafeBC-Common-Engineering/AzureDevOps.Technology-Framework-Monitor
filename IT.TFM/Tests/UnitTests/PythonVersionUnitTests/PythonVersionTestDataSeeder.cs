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
                new() { Version = "python 2.6", EolDate = new DateOnly(2013, 10, 29) },
                new() { Version = "python 2.7", EolDate = new DateOnly(2020, 01, 01) },
                new() { Version = "python 3.0", EolDate = new DateOnly(2009, 06, 27) },
                new() { Version = "python 3.1", EolDate = new DateOnly(2012, 04, 09) },
                new() { Version = "python 3.13", EolDate = new DateOnly(2029, 10, 31) },
                new() { Version = "python 3.14", EolDate = new DateOnly(2030, 10, 31) },
                new() { Version = "python 3.2", EolDate = new DateOnly(2016, 02, 20) },
                new() { Version = "python 3.3", EolDate = new DateOnly(2017, 09, 29) },
                new() { Version = "python 3.4", EolDate = new DateOnly(2019, 03, 18) },
                new() { Version = "python 3.5", EolDate = new DateOnly(2020, 09, 30) },
                new() { Version = "python 3.6", EolDate = new DateOnly(2021, 12, 23) },
                new() { Version = "python 3.7", EolDate = new DateOnly(2023, 06, 27) },
                new() { Version = "python 3.8", EolDate = new DateOnly(2024, 10, 07) },
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
