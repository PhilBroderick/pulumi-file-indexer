using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Storage;

public static class StorageConfig
{
    private static readonly Config Config = new("file-indexer-storage");
    
    public static string FileUploadFolderName => Config.Require("file-upload-folder-name");
    
    public static string StorageSkuName => Config.Require("storage-sku-name");
}