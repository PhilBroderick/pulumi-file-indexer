using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Sql;

public record DatabaseConnection(Output<string> ConnectionString);