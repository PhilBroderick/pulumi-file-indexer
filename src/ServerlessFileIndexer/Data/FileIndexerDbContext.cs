using Microsoft.EntityFrameworkCore;

namespace ServerlessFileIndexer.Data;

public class FileIndexerDbContext : DbContext
{
    public FileIndexerDbContext(DbContextOptions<FileIndexerDbContext> options) 
        : base(options)
    {
    }

    public DbSet<FileIndex> FileIndices { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileIndex>()
            .Property(f => f.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<FileIndex>()
            .Property(f => f.Name)
            .IsRequired();
        
        modelBuilder.Entity<FileIndex>()
            .Property(f => f.UploadedTimestamp)
            .IsRequired();
    }
}