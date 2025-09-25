var builder = DistributedApplication.CreateBuilder(args);

var gameApis = builder.AddProject<Projects.Codebreaker_GameAPIs>("codebreaker-gameapis")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.CodeBreaker_Bot>("codebreaker-bot")
    .WithReference(gameApis)
    .WaitFor(gameApis)
    .WithExternalHttpEndpoints();

builder.Build().Run();
