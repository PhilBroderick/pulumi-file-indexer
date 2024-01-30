using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Function;

public static class FunctionConfig
{
    private static readonly Config Config = new("file-indexer-function");
    
    public static string SkuName => Config.Require("sku-name");
    
    public static string Tier => Config.Require("tier");
}