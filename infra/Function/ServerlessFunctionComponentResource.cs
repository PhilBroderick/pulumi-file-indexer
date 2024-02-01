using Pulumi;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.OperationalInsights.Inputs;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using ServerlessFileIndexerInfrastructure.Sql;
using ServerlessFileIndexerInfrastructure.Storage;

namespace ServerlessFileIndexerInfrastructure.Function;

internal class ServerlessFunctionComponent : ComponentResource
{
    private const string ComponentName = "FileIndexerFunction";

    public ServerlessFunctionComponent(string name, ServerlessFunctionComponentResourceArgs args)
        : base($"{FileIndexerConfig.PackageName}:azure:{ComponentName}", name)
    {
        BuildFunction(name, 
            args.ResourceGroup, 
            args.DatabaseConnection,
            args.StorageCredentials);
        
        RegisterOutputs();
    }
    
    private void BuildFunction(string name, 
        ResourceGroup resourceGroup,
        DatabaseConnection databaseConnection,
        StorageCredentials storage
        )
    {
        var asp = new AppServicePlan($"{name}-app-service-plan", new()
        {
            ResourceGroupName = resourceGroup.Name,
            Kind = "Linux",
            // Needs to be set for Linux ASPs
            Reserved = true,
            Sku = new SkuDescriptionArgs
            {
                Name = FunctionConfig.SkuName,
                Tier = FunctionConfig.Tier
            }
        }, new()
        {
            Parent = this
        });  
        
        // App Insights needs a LA workspace
        var workspace = new Workspace("la-workspace", new WorkspaceArgs
        {
            ResourceGroupName = resourceGroup.Name,
            RetentionInDays = 30,
            Sku = new WorkspaceSkuArgs
            {
                Name = WorkspaceSkuNameEnum.Free
            }
        });

        var appInsights = new Component("app-insights", new ComponentArgs
        {
            ApplicationType = ApplicationType.Web,
            Kind = "web",
            ResourceGroupName = resourceGroup.Name,
            WorkspaceResourceId = workspace.Id
        });
        
        var function = new WebApp($"{name}-function", new WebAppArgs
        {
            Name = $"{name}-function",
            Kind = "FunctionApp",
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = asp.Id,
            SiteConfig = new SiteConfigArgs
            {
                AppSettings = new[]
                {
                    new NameValuePairArgs
                    {
                        Name = "AzureWebJobsStorage",
                        Value = storage.ConnectionString,
                    },
                    new NameValuePairArgs
                    {
                        Name = "ContainerName",
                        Value = storage.FileUploadContainerName,
                    },
                    new NameValuePairArgs
                    {
                        Name = "SqlConnectionString",
                        Value = databaseConnection.ConnectionString
                    },
                    new NameValuePairArgs
                    {
                        Name = "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        Value = Output.Format($"InstrumentationKey={appInsights.InstrumentationKey}"),
                    },
                }
            },
        }, new()
        {
            Parent = this
        });
    }
}
