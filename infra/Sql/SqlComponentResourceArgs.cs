using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Sql.Inputs;

namespace ServerlessFileIndexerInfrastructure.Sql;

public class SqlComponentResourceArgs
{
    public required SkuArgs Sku { get; set; }
    
    public required ResourceGroup ResourceGroup { get; set; }
}