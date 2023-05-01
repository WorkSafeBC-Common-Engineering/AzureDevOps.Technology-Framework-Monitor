namespace ProjectScannerSaveToSqlServer.DataModels
{
    using System.Data.Entity;

    public partial class ProjectScannerDB : DbContext
    {
        public ProjectScannerDB()
            : base("name=ProjectScannerDB")
        {
        }

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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilePropertyType>()
                .HasMany(e => e.FileProperties)
                .WithRequired(e => e.FilePropertyType)
                .HasForeignKey(e => e.PropertyTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FileReferenceType>()
                .HasMany(e => e.FileReferences)
                .WithRequired(e => e.FileReferenceType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.FileProperties)
                .WithRequired(e => e.File)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<File>()
                .HasMany(e => e.FileReferences)
                .WithRequired(e => e.File)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FileType>()
                .HasMany(e => e.Files)
                .WithRequired(e => e.FileType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Organization>()
                .HasMany(e => e.Projects)
                .WithRequired(e => e.Organization)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Project>()
                .HasMany(e => e.Repositories)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Repository>()
                .HasMany(e => e.Files)
                .WithRequired(e => e.Repository)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ScannerType>()
                .HasMany(e => e.Organizations)
                .WithRequired(e => e.ScannerType)
                .WillCascadeOnDelete(false);
        }
    }
}
