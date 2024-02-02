using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;

namespace ServerlessFileIndexerInfrastructure.Sql;

public class SqlComponentResource : ComponentResource
{
    private const string ComponentName = "FileIndexerDatabase";
    
    public DatabaseConnection DatabaseConnection { get; init; }
    
    [Output]
    public Output<string> DbConnectionString { get; init; }

    public SqlComponentResource(string name, SqlComponentResourceArgs args)
        : base ($"{FileIndexerConfig.PackageName}:azure:{ComponentName}", name)
    {
        var sqlAdminCredentials = SqlSecrets.Credentials;
        
        DatabaseConnection = BuildDatabase(name, args.ResourceGroup, args.Sku, sqlAdminCredentials);

        DbConnectionString = DatabaseConnection.ConnectionString;
        
        RegisterOutputs();
    }
    
    private DatabaseConnection BuildDatabase(string name, 
        ResourceGroup resourceGroup, 
        SkuArgs skuArgs,
        SqlAdminCredentials sqlAdminCredentials)
    {
        var sqlServer = new Server("sql-server", new ServerArgs
        {
            ResourceGroupName = resourceGroup.Name,
            ServerName = name,
            PublicNetworkAccess = ServerNetworkAccessFlag.Enabled,
            AdministratorLogin = sqlAdminCredentials.SqlAdminUsername,
            AdministratorLoginPassword = sqlAdminCredentials.SqlAdminPassword
        }, new()
        {
            Parent = this
        });

        var fileIndexerDatabase = new Database($"{name}-db", new()
        {
            DatabaseName = $"{name}-db",
            ResourceGroupName = resourceGroup.Name,
            ServerName = sqlServer.Name,
            Sku = skuArgs
        }, new()
        {
            Parent = this
        });

        return new DatabaseConnection(SqlHelpers.ConnectionString(sqlServer.Name, fileIndexerDatabase.Name,
            sqlAdminCredentials.SqlAdminUsername, sqlAdminCredentials.SqlAdminPassword));
    }
}