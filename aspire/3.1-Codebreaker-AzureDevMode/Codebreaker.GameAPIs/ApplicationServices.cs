namespace Codebreaker.GameAPIs;

public static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        static void ConfigureSqlServer(IHostApplicationBuilder builder)
        {
            string connectionString = builder.Configuration.GetConnectionString("codebreaker-db") ?? throw new InvalidOperationException("Connection string 'codebreaker-db' not found.");
            builder.Services.AddDbContext<IGamesRepository, GamesSqlServerContext>(
                options => options.UseSqlServer(connectionString));

            builder.EnrichSqlServerDbContext<GamesSqlServerContext>(options =>
            {
            });
        }

        static void ConfigureCosmos(IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<IGamesRepository, DataContextProxy<GamesCosmosContext>>();

            builder.AddCosmosDbContext<GamesCosmosContext>("codebreaker-games");
        }

        static void ConfigureInMemory(IHostApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IGamesRepository, GamesMemoryRepository>();
        }

        string? dataStore = builder.Configuration.GetValue<string>("DataStore");
        switch (dataStore)
        {
            case "Cosmos":
                ConfigureCosmos(builder);
                break;
            case "SqlServer":
                ConfigureSqlServer(builder);
                break;
            default:
                ConfigureInMemory(builder);
                break;
        }

        builder.Services.AddScoped<IGamesService, GamesService>();
    }

    public static async Task CreateOrUpdateDatabaseAsync(this WebApplication app)
    {
        var dataStore = app.Configuration["DataStore"] ?? "InMemory";
        if (dataStore == "SqlServer")
        {
            try
            {
                using var scope = app.Services.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                if (repo is GamesSqlServerContext context)
                {
                    await context.Database.MigrateAsync();
                    app.Logger.LogInformation("SQL Server database updated");
                }
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Error updating database");
                throw;
            }
        }
    }
}