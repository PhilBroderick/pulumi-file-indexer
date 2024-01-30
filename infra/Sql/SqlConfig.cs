using Config = Pulumi.Config;

namespace ServerlessFileIndexerInfrastructure.Sql;

public static class SqlConfig
{
    private static readonly Config Config = new("file-indexer-sql");
    
    public static string SqlAdminUsername => Config.Require("sql-admin-username");
    
    public static string DatabaseTier => Config.Require("database-tier");
    public static string DatabaseSkuName => Config.Require("sku-name");
}