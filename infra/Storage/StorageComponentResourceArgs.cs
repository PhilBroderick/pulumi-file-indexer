using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage.Inputs;

namespace ServerlessFileIndexerInfrastructure.Storage;

public record StorageComponentResourceArgs(SkuArgs Sku, ResourceGroup ResourceGroup, string FileUploadContainerName);