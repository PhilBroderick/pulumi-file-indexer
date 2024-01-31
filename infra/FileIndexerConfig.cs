using Pulumi;

namespace ServerlessFileIndexerInfrastructure;

public static class FileIndexerConfig
{
    private static readonly Config Config = new("file-indexer");


    public const string PackageName = "philbroderick";
}