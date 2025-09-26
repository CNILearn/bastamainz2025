targetScope = 'subscription'

param resourceGroupName string

param location string

param principalId string

resource rg 'Microsoft.Resources/resourceGroups@2023-07-01' = {
  name: resourceGroupName
  location: location
}

module cosmos_db 'cosmos-db/cosmos-db.bicep' = {
  name: 'cosmos-db'
  scope: rg
  params: {
    location: location
  }
}

module cosmos_db_roles 'cosmos-db-roles/cosmos-db-roles.bicep' = {
  name: 'cosmos-db-roles'
  scope: rg
  params: {
    location: location
    cosmos_db_outputs_name: cosmos_db.outputs.name
    principalId: ''
  }
}