using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Storage;

public record StorageCredentials(Input<string> ConnectionString, Input<string> StorageAccountBaseAddress, Input<string> FileUploadContainerName);