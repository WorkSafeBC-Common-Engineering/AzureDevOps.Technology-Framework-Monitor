﻿using ProjectData;

namespace RepoScan.DataModels
{
    public class FileItem
    {
        public RepositoryItem Repository { get; set; }

        public FileItemType FileType { get; set; }

        public string Id { get; set; }

        public string Path { get; set; }

        public string Url { get; set; }

        public string CommitId { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Path: {Path}, FileType: {FileType}, RepositoryId: {Repository.RepositoryName}";
        }
    }
}
