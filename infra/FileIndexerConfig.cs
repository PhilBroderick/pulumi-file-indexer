using Pulumi;

namespace ServerlessFileIndexerInfrastructure;

public static class FileIndexerConfig
{
    private static readonly Config Config = new("file-indexer");

    public static string StorageSkuName => Config.Require("storage-sku-name");

    public static string FileUploadFolderName => Config.Require("file-upload-folder-name");
}