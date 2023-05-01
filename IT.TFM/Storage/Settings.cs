﻿using ProjectData;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Storage
{
    public static class Settings
    {
        #region Private Members

        private const string storageKey = "StorageType";
        private const string configurationKey = "StorageConfiguration";

        #endregion

        #region Constructors

        static Settings()
        {
            var appSettings = ConfigurationManager.AppSettings;
            Storage = appSettings[storageKey];
            Configuration = appSettings[configurationKey];
        }

        #endregion

        #region Public Properties

        internal static string Storage { get; private set; }

        internal static string Configuration { get; private set; }

        #endregion
    }
}
