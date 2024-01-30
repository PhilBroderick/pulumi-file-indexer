using System;

namespace ServerlessFileIndexerInfrastructure.Sql;

public static class SqlSecrets
{
    public static SqlAdminCredentials Credentials => LazyCredentials.Value;

    private static readonly Lazy<SqlAdminCredentials> LazyCredentials = new(() =>
        new(SqlAdminUsername!, Password.Result));

    private static readonly string SqlAdminUsername = $"{SqlConfig.SqlAdminUsername}";

    /// <summary>
    /// create a random password for SQL Admin
    /// using Pulumi.Random ensures the result is treated as sensitive and not displayed in console output
    /// </summary>
    private static Pulumi.Random.RandomPassword Password => new("sql-admin-password",
        new Pulumi.Random.RandomPasswordArgs
        {
            Length = 16,
            Special = true
        });
}