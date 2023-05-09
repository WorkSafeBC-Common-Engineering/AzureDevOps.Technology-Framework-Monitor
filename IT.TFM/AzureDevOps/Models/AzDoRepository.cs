﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AzureDevOps.Models
{
    public class AzDoRepository
    {
        public string DefaultBranch { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public bool IsDisabled { get; set; } = false;

        public bool IsFork { get; set; } = false;

        public string Name { get; set; } = string.Empty;

        public int Size { get; set; }

        public string Url { get; set; } = string.Empty;

        public string WebUrl { get; set; } = string.Empty;
    }

    public class AzDoRepositoryList
    {
        public int Count { get; set; }

        public AzDoRepository[] Value { get; set; } = Array.Empty<AzDoRepository>();
    }
}
