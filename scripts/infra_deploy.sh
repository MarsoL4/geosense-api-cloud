#!/bin/bash
set -e

# Configuráveis - troque se quiser outros nomes, mas mantenha únicos!
RG="geosense-rg"
LOCATION="brazilsouth"
PGSERVER="geosensepgserver"
PGADMIN="geosenseadmin"
PGPASSWORD="SenhaForte123!"
PGDB="geosense"
ACR="geosenseacr"
APPPLAN="geosense-plan"
WEBAPP="geosense-app-s4"
APIKEY="SEGREDO-GEOSENSE-123"

echo "== 1. Criando Resource Group =="
az group create --name "$RG" --location "$LOCATION"

echo "== 2. Criando Azure PostgreSQL Flexible Server =="
az postgres flexible-server create --resource-group "$RG" --name "$PGSERVER" --admin-user "$PGADMIN" --admin-password "$PGPASSWORD" --location "$LOCATION" --version 16 --public-access 0.0.0.0

echo "== 3. Criando database principal =="
az postgres flexible-server db create --resource-group "$RG" --server-name "$PGSERVER" --database-name "$PGDB"

echo "== 4. Criando Azure Container Registry (ACR) =="
az acr create --resource-group "$RG" --name "$ACR" --sku Basic

echo "== 5. Criando App Service Plan Linux =="
az appservice plan create --name "$APPPLAN" --resource-group "$RG" --is-linux --sku B1 --location "$LOCATION"

echo "== 6. Criando Web App for Containers com ACR =="
az webapp create --resource-group "$RG" --plan "$APPPLAN" --name "$WEBAPP" --deployment-container-image-name "$ACR.azurecr.io/geosense-api:latest"

echo "== 7. Configurando Application Settings (connection string/API key) =="
az webapp config appsettings set --resource-group "$RG" --name "$WEBAPP" --settings \
  "ConnectionStrings__DefaultConnection=Host=${PGSERVER}.postgres.database.azure.com;Port=5432;Database=${PGDB};Username=${PGADMIN};Password=${PGPASSWORD};Ssl Mode=Require;" \
  "ApiKey=$APIKEY"

echo "== 8. Habilitando Admin Access no ACR e configurando containers (para pull) =="
az acr update --name "$ACR" --admin-enabled true

ACRUSER=$(az acr credential show -n $ACR --query username -o tsv)
ACRPASS=$(az acr credential show -n $ACR --query passwords[0].value -o tsv)

az webapp config container set --resource-group "$RG" --name "$WEBAPP" \
  --docker-custom-image-name "$ACR.azurecr.io/geosense-api:latest" \
  --docker-registry-server-url "https://$ACR.azurecr.io" \
  --docker-registry-server-user "$ACRUSER" \
  --docker-registry-server-password "$ACRPASS"

echo "== Infraestrutura provisionada! Pronto para buildar e pushar a imagem da API =="