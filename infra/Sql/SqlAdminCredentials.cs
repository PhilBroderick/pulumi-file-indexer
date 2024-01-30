using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Sql;

public record SqlAdminCredentials(Input<string> SqlAdminUsername, Input<string> SqlAdminPassword);