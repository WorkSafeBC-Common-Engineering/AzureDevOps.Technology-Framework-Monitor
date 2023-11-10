using ProjectData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

namespace ProjectScanner
{
    public static class Settings
    {
        #region Private Members

        private const string separator = "||";
        private static readonly string[] fieldSeparator = { separator };

        private const string scannerKey = "Scanner";

        private const int maxConfigurationFields = 2;

        private static readonly object initLock = new object();
        private static Dictionary<string, string> scanners;

        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();
        private static bool isInitialized = false;


        #endregion

        #region Public Properties

        public static IEnumerable<string> Scanners 
        {
            get { return scanners.Keys.AsEnumerable(); }
        }

        public static bool IsInitialized
        {
            get
            {
                cacheLock.EnterReadLock();
                try
                {
                    return isInitialized;
                }
                finally
                {
                    cacheLock.ExitReadLock();
                }
            }
            set
            {
                cacheLock.EnterWriteLock();
                try
                {
                    isInitialized = value;
                }
                finally
                {
                    cacheLock.ExitWriteLock();
                }
            }
        }

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            lock(initLock)
            {
                if (!IsInitialized)
                {
                    var appSettings = ConfigurationManager.AppSettings;
                    scanners = new Dictionary<string, string>();

                    var allKeys = appSettings.AllKeys.Where(k => k.StartsWith(scannerKey));
                    foreach (var key in allKeys)
                    {
                        scanners.Add(key[(key.IndexOf(separator) + separator.Length)..], appSettings[key]);
                    }

                    IsInitialized = true;
                }
            }
        }

        public static ProjectSource GetSource(string name)
        {
            IsValidName(name);

            var configuration = scanners[name];
            var fields = configuration.Split(fieldSeparator, StringSplitOptions.None);
            var enumName = Enum.GetNames(typeof(ProjectSource)).SingleOrDefault(e => e.Equals(fields[0], StringComparison.InvariantCultureIgnoreCase));

            if (enumName == null)
            {
                throw new ArgumentException($"Invalid Scanner type for configuration {name} : {configuration}");
            }

            return (ProjectSource)Enum.Parse(typeof(ProjectSource), enumName);
        }

        public static string GetConfigurationData(string name)
        {
            IsValidName(name);

            var configuration = scanners[name];
            var fields = configuration.Split(fieldSeparator, StringSplitOptions.None);

            if (fields.Length < maxConfigurationFields)
            {
                throw new ApplicationException($"Configuration data for {name} scanner is missing: {configuration}");
            }

            return fields[1];
        }

        #endregion

        #region Private Methods

        private static void IsValidName(string name)
        {
            if (!scanners.ContainsKey(name))
            {
                throw new ArgumentException($"Scanner not defined in appSettings: {name}");
            }
        }

        #endregion
    }
}
