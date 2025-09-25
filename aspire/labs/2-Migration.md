# Lab 2: Migrate the existing .NET application to .NET Aspire

This lab requires Docker Desktop (or Podman) installed and running.

## Use the existing Codebreaker application

Copy the application from `2.1-Codebreaker`, run it locally. Open the solution file. Either use the http files or the Swagger UI to test the application and run games. Check the log output in the console, and the database entries in the SQL Server database.

Check how the connection string is configured, and the configuration of the Games API via the Bot project.

## Update the application to .NET Aspire

Use Visual Studio to add .NET Aspire orchestration to the Games API and the Bot project.

Check this:

- Project references to the games API and the bot project in the AppHost project
- The app model with the AppHost
- The default services telemetry configuration and service discovery configuration
- Where the service discovery configuration is used in the Games API and Bot project
- The endpoints configured in the default services project, and how this is used in the Games API and Bot project

Run the application, and monitor the log, metrics, and distributed traces in the .NET Aspire dashboard.

## Update the Bot service to use service discovery

Create a dependency between the Bot project and the Games API project using the app model:

```csharp
var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("codebreaker-gameapis")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.CodeBreaker_Bot>("codebreaker-bot")
    .WithReference(gameApis)
    .WaitFor(gameApis)
    .WithExternalHttpEndpoints();
```

With the **Bot project**, remove the configuration of the link to the Games API, and add a service discovery name in `ApplicationServices.cs`:

```csharp
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHttpClient<GamesClient>(client =>
        {
            client.BaseAddress = new Uri("https+http://codebreaker-gameapis");
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
```

Check how the name is resolved with the HttpClient configuration.

## Add the database to the orchestration

### AppHost project

Add the NuGet packages `Aspire.Hosting.SqlServer`, and `Aspire.Hosting.Azure.CosmosDb` to the AppHost project.
Add the `DataStore` configuration to `appsettings.json`:

```json
  "DataStore": "SqlServer"
```

Read the configuration from the appsettings.json in `AppHost.cs`:

```csharp
var dataStore = builder.Configuration["DataStore"] ?? "InMemory";
var dataStoreParam = builder.AddParameter("DataStore", dataStore);
```

Pass the environment variable to the Games API project:

```csharp
var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("codebreaker-gameapis")
    .WithExternalHttpEndpoints()
    .WithEnvironment("DataStore", dataStoreParam);
```

Add the SQL Server database to the orchestration:

```csharp
if (dataStore == "SqlServer")
{
    var sqlGames = builder.AddSqlServer("gamesdatabase")
        .WithDataVolume("sqlgamesdata-volume")
        .WithLifetime(ContainerLifetime.Session)
        .AddDatabase("codebreaker-db");

    gameApis.WithReference(sqlGames).WaitFor(sqlGames);
}
```

Add the Azure Cosmos DB database to the orchestration:

```csharp
else if (dataStore == "Cosmos")
{
#pragma warning disable ASPIRECOSMOSDB001
    var cosmosServer = builder.AddAzureCosmosDB("cosmos-db")
        .RunAsPreviewEmulator(p =>
            p.WithDataExplorer()
            .WithDataVolume("sqlcosmosdata-volume")
            .WithLifetime(ContainerLifetime.Session));
#pragma warning restore ASPIRECOSMOSDB001

    var cosmosdb = cosmosServer
        .AddCosmosDatabase("codebreaker-games");

    cosmosdb.AddContainer("GamesV3", "/PartitionKey");

    gameApis.WithReference(cosmosdb).WaitFor(cosmosdb);
}
```

### Games API project

Add the NuGet packages `Aspire.Microsoft.EntityFrameworkCore.SqlServer`, and `Aspire.Microsoft.EntityFrameworkCore.Cosmos` to the Games API project.

Add the .NET Aspire integration configuration for SQL Server with EF Core:

```csharp
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            string connectionString = builder.Configuration.GetConnectionString("codebreaker-db") ?? throw new InvalidOperationException("Connection string 'codebreaker-db' not found.");
            builder.Services.AddDbContext<IGamesRepository, GamesSqlServerContext>(
                options => options.UseSqlServer(connectionString));

            builder.EnrichSqlServerDbContext<GamesSqlServerContext>(options =>
            {
            });
        }
```

Add the .NET Aspire integration configuration for Azure Cosmos DB with EF Core:

```csharp
        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();

            builder.AddCosmosDbContext<GamesCosmosContext>("codebreaker-games");
        }
```

Run the application with SQL Server and Azure Cosmos DB, and check the log output, metrics, and distributed traces in the .NET Aspire dashboard.
