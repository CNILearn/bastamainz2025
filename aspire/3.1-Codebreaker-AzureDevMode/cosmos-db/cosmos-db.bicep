@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource cosmos_db 'Microsoft.DocumentDB/databaseAccounts@2024-08-15' = {
  name: take('cosmosdb-${uniqueString(resourceGroup().id)}', 44)
  location: location
  properties: {
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    disableLocalAuth: true
  }
  kind: 'GlobalDocumentDB'
  tags: {
    'aspire-resource-name': 'cosmos-db'
  }
}

resource codebreaker_games 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-08-15' = {
  name: 'codebreaker-games'
  location: location
  properties: {
    resource: {
      id: 'codebreaker-games'
    }
  }
  parent: cosmos_db
}

resource GamesV3 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-08-15' = {
  name: 'GamesV3'
  location: location
  properties: {
    resource: {
      id: 'GamesV3'
      partitionKey: {
        paths: [
          '/PartitionKey'
        ]
        kind: 'Hash'
      }
    }
  }
  parent: codebreaker_games
}

output connectionString string = cosmos_db.properties.documentEndpoint

output name string = cosmos_db.name