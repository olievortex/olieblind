#!/bin/sh
subscription_id=0b78c968-f580-4bef-a8a3-a51a51a912b1
resource_group=rg-olievortex-public
storage_name=stolieblinda3dd
ws_name=log-SCUS

echo Create app registration / service principal
app_id=$(az ad app create --display-name ap-olieblind --query appId --output tsv)
az ad sp create --id ${app_id}
sp_object=$(az ad sp list --display-name ap-olieblind --query "[0].id" --output tsv)

echo Create security group / Add service principal
az ad group create \
          --display-name group-olieblind
          --mail-nickname group-olieblind
az ad group member add \
          --group group-olieblind \
          --member-id ${sp_object}
group_id=$(az ad group list --display-name group-olieblind --query "[0].id" --output tsv)

echo Create resource group
az group create --name ${resource_group} \
        --location 'North Central US' \
        --subscription ${subscription_id}

echo Create storage account, container, rbac, and lifecycle management
az storage account create \
        --name ${storage_name} \
        --location 'South Central US' \
        --resource-group ${resource_group} \
        --subscription ${subscription_id} \
        --min-tls-version TLS1_2 \
        --access-tier Hot \
        --allow-shared-key-access false \
        --kind StorageV2 \
        --sku Standard_LRS

MSYS_NO_PATHCONV=1 az role assignment create --assignee ${group_id} \
        --role "Storage Blob Data Contributor" \
        --subscription ${subscription_id} \
        --scope /subscriptions/${subscription_id}/resourceGroups/${resource_group}/providers/Microsoft.Storage/storageAccounts/${storage_name}

az storage container create \
        --subscription ${subscription_id} \
        --account-name ${storage_name} \
        --name mysql-backups \
        --auth-mode login

az storage account management-policy create \
        --subscription ${subscription_id} \
        --account-name ${storage_name} \
        --resource-group ${resource_group} \
        --policy @lifecycle-mysql.json

echo Create workspace
az monitor log-analytics workspace create \
        --name ${ws_name} \
        --resource-group ${resource_group} \
        --location 'South Central US' \
        --subscription ${subscription_id}

echo Create Application Insights
MSYS_NO_PATHCONV=1 az monitor app-insights component create \
        --app appi-olieblind \
        --resource-group ${resource_group} \
        --location 'South Central US' \
        --workspace /subscriptions/${subscription_id}/resourceGroups/${resource_group}/providers/Microsoft.OperationalInsights/workspaces/${ws_name} \
        --subscription ${subscription_id}

echo Create Service Bus
az servicebus namespace create \
        --subscription ${subscription_id} \
        --location 'South Central US' \
        --resource-group ${resource_group} \
        --name sb-olieblind

az servicebus queue create \
        --subscription ${subscription_id} \
        --resource-group ${resource_group} \
        --namespace-name sb-olieblind \
        --name satellite_aws_posters

az servicebus queue create \
        --subscription ${subscription_id} \
        --resource-group ${resource_group} \
        --namespace-name sb-olieblind \
        --lock-duration PT5M \
        --name satellite_adhoc

MSYS_NO_PATHCONV=1 az role assignment create --assignee ${group_id} \
        --role "Azure Service Bus Data Owner" \
        --subscription ${subscription_id} \
        --scope /subscriptions/${subscription_id}/resourceGroups/${resource_group}/providers/Microsoft.ServiceBus/namespaces/sb-olieblind

echo Done!
