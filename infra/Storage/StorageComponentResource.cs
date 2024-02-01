using System;
using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using SkuArgs = Pulumi.AzureNative.Storage.Inputs.SkuArgs;

namespace ServerlessFileIndexerInfrastructure.Storage;

public class StorageComponentResource : ComponentResource
{
    private const string ComponentName = "FileIndexerStorage";
    public StorageCredentials StorageCredentials { get; init; }
    
    [Output]
    public Output<string> FileUploadContainerUrl { get; init; }

    public StorageComponentResource(string name, StorageComponentResourceArgs args)
        : base ($"{FileIndexerConfig.PackageName}:azure:{ComponentName}", name)
    {
        StorageCredentials = BuildStorageAccount(name, args.ResourceGroup, args.Sku, args.FileUploadContainerName);
        FileUploadContainerUrl = Output.Tuple(StorageCredentials.StorageAccountBaseAddress,
                StorageCredentials.FileUploadContainerName)
            .Apply(t => $"{t.Item1}{t.Item2}");

        RegisterOutputs();
    }
    
    private StorageCredentials BuildStorageAccount(string name, ResourceGroup resourceGroup, SkuArgs skuArgs,
        string fileUploadContainerName)
    {
        // Azure only allows lowercase + digits for storage accounts, between 3-24 characters      
        var normalisedStorageAccountName = name.Replace("-", string.Empty);
        if (normalisedStorageAccountName.Length is < 3 or > 24)
        {
            throw new ArgumentException($"Invalid name for Storage Account. Got {normalisedStorageAccountName} which is {normalisedStorageAccountName.Length} characters. Must be 12-24 characters");
        }

        var storageAccount = new StorageAccount(name, new()
        {
            AccountName = normalisedStorageAccountName,
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,

            AllowBlobPublicAccess = true,

            Kind = Kind.StorageV2,
            Sku = skuArgs
        }, new()
        {
            Parent = this
        });
        
        var fileUploadBlobContainer = new BlobContainer("file-upload-container", new()
        {
            AccountName = storageAccount.Name,
            ContainerName = fileUploadContainerName,
            ResourceGroupName = resourceGroup.Name,
            PublicAccess = PublicAccess.Blob
        }, new()
        {
            Parent = this
        });

        var storageConnectionString = GetStorageConnectionString(resourceGroup.Name, storageAccount.Name);

        return new (storageConnectionString, storageAccount.PrimaryEndpoints.Apply(p => p.Blob),fileUploadBlobContainer.Name);
    }
    
    private static Output<string> GetStorageConnectionString(Input<string> resourceGroupName, Input<string> accountName)
    {
        // Retrieve the primary storage account key.
        var storageAccountKeys = ListStorageAccountKeys.Invoke(new ListStorageAccountKeysInvokeArgs
        {
            ResourceGroupName = resourceGroupName,
            AccountName = accountName
        });

        return storageAccountKeys.Apply(keys =>
        {
            var primaryStorageKey = keys.Keys[0].Value;

            return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey}");
        });
    }
}