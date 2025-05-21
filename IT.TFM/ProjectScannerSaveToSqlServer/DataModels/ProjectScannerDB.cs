namespace ProjectScannerSaveToSqlServer.DataModels
{
    using Microsoft.EntityFrameworkCore;
    using System;

    public partial class ProjectScannerDB(string connection) : DbContext()
    {
        public virtual DbSet<FileProperty> FileProperties { get; set; }
        public virtual DbSet<FilePropertyType> FilePropertyTypes { get; set; }
        public virtual DbSet<FileReference> FileReferences { get; set; }
        public virtual DbSet<FileReferenceType> FileReferenceTypes { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<NuGetFeed> NuGetFeeds { get; set; }
        public virtual DbSet<NuGetPackage> NuGetPackages { get; set; }
        public virtual DbSet<NuGetTargetFramework> NuGetTargetFrameworks { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Repository> Repositories { get; set; }
        public virtual DbSet<ScannerType> ScannerTypes { get; set; }
        public virtual DbSet<Pipeline> Pipelines { get; set; }
        public virtual DbSet<PipelineType> PipelineTypes { get; set; }
        public virtual DbSet<ReleaseArtifact> ReleaseArtifacts { get; set; }
        public virtual DbSet<ProjectMetrics> ProjectMetrics { get; set; }
        private readonly string dbConnection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(dbConnection,
                              sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                                                                      maxRetryCount: 5,
                                                                      maxRetryDelay: TimeSpan.FromSeconds(30),
                                                                      errorNumbersToAdd: null));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilePropertyType>()
                .HasMany(e => e.FileProperties)
                .WithOne(e => e.FilePropertyType)
                .HasForeignKey(e => e.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileReferenceType>()
                .HasMany(e => e.FileReferences)
                .WithOne(e => e.FileReferenceType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<File>()
                .HasMany(e => e.FileProperties)
                .WithOne(e => e.File)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<File>()
                .HasMany(e => e.FileReferences)
                .WithOne(e => e.File)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FileType>()
                .HasMany(e => e.Files)
                .WithOne(e => e.FileType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.Projects)
                .WithOne(e => e.Organization)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Repositories)
                .WithOne(e => e.Project)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Pipelines)
                .WithOne(e => e.Project)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Repository>()
                .HasMany(e => e.Files)
                .WithOne(e => e.Repository)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Repository>()
                .HasMany(e => e.Pipelines)
                .WithOne(e => e.Repository)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScannerType>()
                .HasMany(e => e.Organizations)
                .WithOne(e => e.ScannerType)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity <Pipeline>()
                .HasMany(e => e.ReleaseArtifacts)
                .WithOne(e => e.Pipeline)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PipelineType>()
                .HasMany(e => e.Pipelines)
                .WithOne(e => e.BlueprintType)
                .HasForeignKey(e => e.BlueprintApplicationTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NuGetFeed>()
                .HasOne(nf => nf.Project)
                .WithMany(p => p.NuGetFeeds)
                .HasForeignKey(nf => nf.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NuGetPackage>()
                .HasOne(nf => nf.Repository)
                .WithMany(r => r.NuGetPackages)
                .HasForeignKey(nf => nf.RepositoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NuGetTargetFramework>()
                .HasOne(t => t.NuGetPackage)
                .WithMany(n => n.NuGetTargetFrameworks)
                .HasForeignKey(t => t.NuGetPackageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectMetrics>()
                    .HasOne(m => m.File)
                    .WithOne(f => f.ProjectMetrics)
                    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
