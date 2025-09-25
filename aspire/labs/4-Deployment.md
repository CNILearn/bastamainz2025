# Lab 4: Deploying the application

We'll use different ways to deploy the application.

## Use the Azure Developer CLI (azd) to deploy the application to Azure

Run `azd init` to initialize the project. Specify an environment name, e.g. `basta2025-testing`.

Run `azd up` to deploy the application to Azure. This will create all the resources needed, including an Azure Cosmos DB account.

When prompted, enter the location, resource group name, and subscription ID for the Azure Cosmos DB account. Check the log output to see if something fails.

Run the application, and play some games. Check the log output, metrics, and distributed traces in the .NET Aspire dashboard within the Azure Container Apps environment.

## Use `aspire publish`, and use Docker Compose to run the application locally

Add the NuGet package `Aspire.Hosting.Docker` (preview) to the AppHost project.
Add the `"HostEnvironment": "Docker"` setting to the appsettings.json file in the AppHost project:

```json
  "DataStore": "InMemory",
  "HostEnvironment": "Docker"
```

> You can also create multiple environments, and deploy the Cosmos database to Azure, while running the services in Docker containers locally (using `WithComputeEnvironment`).

Configure the Azure App Service environment in the AppHost project:

```csharp
else if (hostEnvironment == "AzureAppService")
{
    builder.AddAzureAppServiceEnvironment("codebreaker");
}
```

Use `aspire publish` to create the Bicep scripts for the selected environment:

```bash
aspire publish -o publishappservice
```

Check the bicep files created in the `publishappservice` folder.

Use the Azure CLI to deploy the resources to Azure:

```bash
az deployment sub create --location <your-location> --template-file ./main.bicep --parameters resourceGroupName=<your-resource-group> location=<your-location> principalId=<your-principal-id>    
```

To get your principal ID, run:

```bash
az ad signed-in-user show --query id -o tsv
```
