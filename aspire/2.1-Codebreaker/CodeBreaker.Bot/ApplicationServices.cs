namespace CodeBreaker.Bot;

internal static class ApplicationServices
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        // step 4 done: configure the HTTP client to use the Games API service
        builder.Services.AddHttpClient<GamesClient>(client =>
        {
            string apiUrl = builder.Configuration["GameApi"] ?? throw new InvalidOperationException("GameAPI address not found");
            client.BaseAddress = new Uri(apiUrl);
        });

        builder.Services.AddScoped<CodeBreakerTimer>();
        builder.Services.AddScoped<CodeBreakerGameRunner>();
    }
}
