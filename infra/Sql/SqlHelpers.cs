using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Sql;

public static class SqlHelpers
{
    private const string SqlConnectionStringTemplate =
        "Server=tcp:{server}.database.windows.net,1433;Initial Catalog={database};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    
    public static Output<string> ConnectionString(Input<string> server, Input<string> database,
        Input<string> username, Input<string> password) =>
        Output.Tuple(server, database, username, password).Apply(t =>
            Output.CreateSecret(SqlConnectionStringTemplate
                .Replace("{server}", t.Item1)
                .Replace("{database}", t.Item2)
                .Replace("{username}", t.Item3)
                .Replace("{password}", t.Item4)));
}