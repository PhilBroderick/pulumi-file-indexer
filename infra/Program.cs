using System.Collections.Generic;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage.Inputs;
using ServerlessFileIndexerInfrastructure;
using ServerlessFileIndexerInfrastructure.Function;
using ServerlessFileIndexerInfrastructure.Sql;
using ServerlessFileIndexerInfrastructure.Storage;
using ServerlessFileIndexerInfrastructure.Tags;

return await Pulumi.Deployment.RunAsync(() =>
{
    const string resourcePrefix = "file-indexer";
    
    var resourceGroup = new ResourceGroup($"{resourcePrefix}-rsg", new()
    {
        Tags =
        {
            { TagNames.Owner, resourcePrefix },
            { TagNames.Environment, TagValues.Environment },
            { TagNames.CreatedBy, TagValues.Pulumi }
        }
    });

    var storageComponentResource = new StorageComponentResource($"{resourcePrefix}-storage-acc",
        new(new SkuArgs
        {
            Name = StorageConfig.StorageSkuName
        }, resourceGroup, StorageConfig.FileUploadFolderName));
    
    var sqlComponentResource = new SqlComponentResource($"{resourcePrefix}-database", new()
    {
        ResourceGroup = resourceGroup,
        Sku = new()
        {
            Tier = SqlConfig.DatabaseTier,
            Name = SqlConfig.DatabaseSkuName
        }
    });

    var fileIndexerComponentResource = new ServerlessFunctionComponent("file-indexer", new(resourceGroup,
        sqlComponentResource.DatabaseConnection, storageComponentResource.StorageCredentials));

    return new Dictionary<string, object?>
    {
        { StackOutputs.StorageUploadUrl, storageComponentResource.FileUploadContainerUrl },
        { StackOutputs.DbConnectionString, sqlComponentResource.DbConnectionString }
    };
});