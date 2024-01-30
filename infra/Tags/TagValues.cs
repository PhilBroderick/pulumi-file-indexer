using Pulumi;

namespace ServerlessFileIndexerInfrastructure.Tags;
internal static class TagValues
{
    public static readonly string Environment = Deployment.Instance.StackName.ToLowerInvariant();
    public const string Pulumi = "Pulumi";
}
