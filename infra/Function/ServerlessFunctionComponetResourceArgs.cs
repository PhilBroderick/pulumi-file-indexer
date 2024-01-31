using Pulumi.AzureNative.Resources;
using ServerlessFileIndexerInfrastructure.Sql;
using ServerlessFileIndexerInfrastructure.Storage;

namespace ServerlessFileIndexerInfrastructure.Function;

public record ServerlessFunctionComponentResourceArgs(ResourceGroup ResourceGroup,
    DatabaseConnection DatabaseConnection, StorageCredentials StorageCredentials);