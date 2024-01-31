using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerlessFileIndexer.Data;

var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

if (connectionString is null)
    throw new ArgumentNullException(nameof(connectionString), "SQL connection string must be defined");

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddDbContext<FileIndexerDbContext>(
            options => options.UseSqlServer(connectionString));
    });

var hostBuilder = host.Build();

var context = hostBuilder.Services.GetRequiredService<FileIndexerDbContext>();
await context.Database.MigrateAsync();

hostBuilder.Run();
