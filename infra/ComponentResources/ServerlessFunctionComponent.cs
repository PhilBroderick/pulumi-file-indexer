using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using System;
using System.Collections.Generic;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using ServerlessFileIndexerInfrastructure.Function;
using ServerlessFileIndexerInfrastructure.Sql;
using ServerlessFileIndexerInfrastructure.Storage;
using ServerlessFileIndexerInfrastructure.Tags;
using SkuArgs = Pulumi.AzureNative.Storage.Inputs.SkuArgs;

namespace ServerlessFileIndexerInfrastructure.ComponentResources;

internal class ServerlessFunctionComponent : ComponentResource
{
    private const string ComponentName = "ServerlessFileIndexer";

    public ServerlessFunctionComponent(string name, ServerlessFunctionComponentResourceArgs args)
        : base($"philbroderick:azure:{ComponentName}", name)
    {
        // Create resource group to logically contain everything
        var resourceGroup = BuildResourceGroup($"{name}-rsg");

        // Create storage account as we need connection string to pass to Function
        var storageCredentials = BuildStorageAccount($"{name}-storage-acc", resourceGroup, args.StorageSku);
        
        // Likewise with DB - needed for Function (this can be component resource)
        var sqlComponentResource = new SqlComponentResource("file-indexer-database", new()
        {
            ResourceGroup = resourceGroup,
            Sku = new()
            {
                Tier = SqlConfig.DatabaseTier,
                Name = SqlConfig.DatabaseSkuName
            }
        });

        // Create Function
        BuildFunction("file-indexer", 
            resourceGroup, 
            sqlComponentResource.DatabaseConnection,
            storageCredentials);
        
        // Ensure Pulumi is aware this resource has completed construction, providing any outputs
        RegisterOutputs(outputs: new Dictionary<string, object?> 
        {
            { StackOutputs.StorageUploadUrl, storageCredentials.FileUploadContainerName }
        });
    }

    private ResourceGroup BuildResourceGroup(string name)
    {
        return new ResourceGroup(name, new()
        {
            Tags =
            {
                { TagNames.Owner, ComponentName },
                { TagNames.Environment, TagValues.Environment },
                { TagNames.CreatedBy, TagValues.Pulumi }
            }
        }, new()
        {
            Parent = this
        });
    }

    private StorageCredentials BuildStorageAccount(string name, ResourceGroup resourceGroup, SkuArgs skuArgs)
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
            ContainerName = "file-uploads",
            ResourceGroupName = resourceGroup.Name,
        }, new()
        {
            Parent = this
        });

        var storageConnectionString = GetStorageConnectionString(resourceGroup.Name, storageAccount.Name);

        return new (storageConnectionString, fileUploadBlobContainer.Name);
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

    private void BuildFunction(string name, 
        ResourceGroup resourceGroup,
        DatabaseConnection databaseConnection,
        StorageCredentials storage
        )
    {
        var asp = new AppServicePlan($"{name}-app-service-plan", new()
        {
            ResourceGroupName = resourceGroup.Name,
            Kind = "Linux",
            // Needs to be set for Linux ASPs
            Reserved = true,
            Sku = new SkuDescriptionArgs
            {
                Name = FunctionConfig.SkuName,
                Tier = FunctionConfig.Tier
            }
        }, new()
        {
            Parent = this
        });  
        
        var function = new WebApp($"{name}-function", new WebAppArgs
        {
            Name = $"{name}-function",
            Kind = "FunctionApp",
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = asp.Id,
            SiteConfig = new SiteConfigArgs
            {
                AppSettings = new[]
                {
                    new NameValuePairArgs
                    {
                        Name = "AzureWebJobsStorage",
                        Value = storage.ConnectionString,
                    },
                    new NameValuePairArgs
                    {
                        Name = "ContainerName",
                        Value = storage.FileUploadContainerName,
                    },
                    new NameValuePairArgs
                    {
                        Name = "SqlConnectionString",
                        Value = databaseConnection.ConnectionString
                    }
                }
            },
        }, new()
        {
            Parent = this
        });
    }
}
