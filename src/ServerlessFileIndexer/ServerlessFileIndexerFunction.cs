using System;
using System.IO;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServerlessFileIndexer
{
    public class ServerlessFileIndexerFunction
    {
        private readonly ILogger _logger;

        public ServerlessFileIndexerFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServerlessFileIndexerFunction>();
        }

        [Function("ServerlessFileIndexerFunction")]
        public void Run([BlobTrigger("file-uploads/{name}", Connection = "StorageConnectionString")] string fileData, string name)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {fileData}");
        }
    }
}
