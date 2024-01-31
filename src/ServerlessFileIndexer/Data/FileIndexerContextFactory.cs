using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ServerlessFileIndexer.Data;

public class FileIndexerContextFactory : IDesignTimeDbContextFactory<FileIndexerDbContext>
{
    public FileIndexerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FileIndexerDbContext>();
        optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));

        return new FileIndexerDbContext(optionsBuilder.Options);
    }
}