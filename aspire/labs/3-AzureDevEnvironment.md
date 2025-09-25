# Lab 3: Run Azure resources from your local development environment

This lab requires an Azure account. If you don't have one, create a free account before you begin.

## Configure the AppHost to use Azure Cosmos DB

Configure the AppHost to use Azure Cosmos DB running in Azure instead of using a Docker emulator.

```csharp
else if (dataStore == "Cosmos")
{
    // now use Azure Cosmos DB from the cloud while developing locally!
    var cosmosServer = builder.AddAzureCosmosDB("cosmos-db");

    var cosmosdb = cosmosServer
        .AddCosmosDatabase("codebreaker-games");

    cosmosdb.AddContainer("GamesV3", "/PartitionKey");

    gameApis.WithReference(cosmosdb).WaitFor(cosmosdb);
}
```

## Run the application with automatic deployment of Azure Cosmos DB

Create a resource group to be used for the Azure Cosmos DB account, e.g. `rg-localdev-basta2025`.

Check the subscription you are using (`az account show --query name -o tsv`).
Make sure it's the correct subscription (`az account set --subscription "your-subscription-id-or-name"`).
Have your Azure subscription Id ready (`az account show --query id -o tsv`). 

Start the application, and enter the location, resource group name, and subscription ID for the Azure Cosmos DB account when prompted. Check the log output to see if something fails.

Check the user secrets in the app-host project to see this configuration added by .NET Aspire. You might need to add a different account type to be used:

```json
 "Azure:CredentialSource": "AzureCli"
```

Wait until the Azure Cosmos DB account is created, and the application is started. Play some games. Check the log output, metrics, and distributed traces in the .NET Aspire dashboard.

Check the user secrets in the app-host project to see the Azure resources added by .NET Aspire. The same configuration is used when you re-run the application. You no longer need to wait for the deployment of the resources.

After the lab, you need to clean up the resources, e.g. by deleting the resource group you created.
