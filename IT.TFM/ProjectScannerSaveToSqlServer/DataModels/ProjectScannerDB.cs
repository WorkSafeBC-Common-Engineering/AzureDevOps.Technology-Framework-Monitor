namespace ProjectScannerSaveToSqlServer.DataModels
{
    using Microsoft.EntityFrameworkCore;

    public partial class ProjectScannerDB(string connection) : DbContext()
    {
        public virtual DbSet<FileProperty> FileProperties { get; set; }
        public virtual DbSet<FilePropertyType> FilePropertyTypes { get; set; }
        public virtual DbSet<FileReference> FileReferences { get; set; }
        public virtual DbSet<FileReferenceType> FileReferenceTypes { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<FileType> FileTypes { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Repository> Repositories { get; set; }
        public virtual DbSet<ScannerType> ScannerTypes { get; set; }
        public virtual DbSet<Pipeline> Pipelines { get; set; }
        public virtual DbSet<ReleaseArtifact> ReleaseArtifacts { get; set; }
        public virtual DbSet<PackageIssue> PackageIssues { get; set; }

        private readonly string dbConnection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(dbConnection);
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

            modelBuilder.Entity<File>()
                .HasMany(e => e.PackageIssues)
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
        }
    }
}
