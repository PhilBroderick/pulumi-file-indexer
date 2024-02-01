using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ServerlessFileIndexer.Data;

namespace ServerlessFileIndexer
{
    public class ServerlessFileIndexerFunction
    {
        private readonly FileIndexerDbContext _dbContext;
        private readonly ILogger _logger;

        public ServerlessFileIndexerFunction(
            FileIndexerDbContext dbContext,
            ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext;
            _logger = loggerFactory.CreateLogger<ServerlessFileIndexerFunction>();
        }

        [Function("ServerlessFileIndexerFunction")]
        public async Task Run([BlobTrigger("%ContainerName%/{name}", Connection = "StorageConnectionString")] string fileData, string name)
        {
            _logger.LogInformation("Function triggered by new blob: {Name}", name);
            
            var fileIndex = new FileIndex
            {
                Name = name,
                UploadedTimestamp = DateTime.UtcNow
            };

            await _dbContext.FileIndices.AddAsync(fileIndex);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation("New index added to database for blob: {Name}", name);
        }
    }
}
