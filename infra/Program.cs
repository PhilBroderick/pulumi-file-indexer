using Pulumi.AzureNative.Storage.Inputs;
using ServerlessFileIndexerInfrastructure;
using ServerlessFileIndexerInfrastructure.ComponentResources;

return await Pulumi.Deployment.RunAsync(() =>
{
    var fileIndexerComponentResource = new ServerlessFunctionComponent("file-indexer", new()
    {
        FileUploadFolderName = FileIndexerConfig.FileUploadFolderName,
        StorageSku = new SkuArgs
        {
            Name = FileIndexerConfig.StorageSkuName
        }
    });
});