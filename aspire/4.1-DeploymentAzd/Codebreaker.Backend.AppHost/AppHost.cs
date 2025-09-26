var builder = DistributedApplication.CreateBuilder(args);

var dataStore = builder.Configuration["DataStore"] ?? "InMemory";

var dataStoreParam = builder.AddParameter("DataStore", dataStore);

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("codebreaker-gameapis")
    .WithExternalHttpEndpoints()
    .WithEnvironment("DataStore", dataStoreParam);

builder.AddProject<Projects.CodeBreaker_Bot>("codebreaker-bot")
    .WithReference(gameApis)
    .WaitFor(gameApis)
    .WithExternalHttpEndpoints();

if (dataStore == "SqlServer")
{
    var sqlGames = builder.AddSqlServer("gamesdatabase")
        .WithDataVolume("sqlgamesdata-volume")
        .WithLifetime(ContainerLifetime.Session)
        .AddDatabase("codebreaker-db");

    gameApis.WithReference(sqlGames).WaitFor(sqlGames);
}
else if (dataStore == "Cosmos")
{
    //#pragma warning disable ASPIRECOSMOSDB001
    //    var cosmosServer = builder.AddAzureCosmosDB("cosmos-db")
    //        .RunAsPreviewEmulator(p =>
    //            p.WithDataExplorer()
    //            .WithDataVolume("sqlcosmosdata-volume")
    //            .WithLifetime(ContainerLifetime.Session));
    //#pragma warning restore ASPIRECOSMOSDB001

    var cosmosServer = builder.AddAzureCosmosDB("cosmos-db");

    var cosmosdb = cosmosServer
        .AddCosmosDatabase("codebreaker-games");

    cosmosdb.AddContainer("GamesV3", "/PartitionKey");

    gameApis.WithReference(cosmosdb).WaitFor(cosmosdb);
}

builder.Build().Run();
