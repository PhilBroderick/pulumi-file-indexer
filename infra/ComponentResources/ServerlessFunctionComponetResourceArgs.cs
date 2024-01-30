using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;

namespace ServerlessFileIndexerInfrastructure.ComponentResources;

public class ServerlessFunctionComponentResourceArgs
{
    public SkuArgs StorageSku { get; set; } = new () { Name = SkuName.Standard_LRS };
    
    public required string FileUploadFolderName { get; set; }
}