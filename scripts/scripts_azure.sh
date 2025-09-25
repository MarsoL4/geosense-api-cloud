#!/bin/bash
# GeoSense API - Azure CLI Deployment Script
# Execute todos os comandos abaixo para criar os recursos do App Service e Banco na Azure via CLI.
# Edite os nomes dos recursos conforme necessário. Execute este script após clonar o repositório.

# Variáveis do projeto
RG="geosense-rg"
LOCATION="brazilSouth"
SQL_SERVER="geosensesqlserver"
SQL_ADMIN="geosenseadmin"
SQL_PASSWORD="Geosense#2025"
SQL_DB="geosense-db"
APP_PLAN="geosense-plan"
APP_NAME="geosense-app"

# 1. Criar grupo de recursos
az group create --name $RG --location $LOCATION

# 2. Criar servidor SQL (Azure SQL)
az sql server create --name $SQL_SERVER --resource-group $RG --location $LOCATION --admin-user $SQL_ADMIN --admin-password $SQL_PASSWORD

# 3. Criar banco de dados SQL
az sql db create --resource-group $RG --server $SQL_SERVER --name $SQL_DB --service-objective S0

# 4. Liberar acesso do App Service ao SQL (firewall rule para Azure)
az sql server firewall-rule create --resource-group $RG --server $SQL_SERVER --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0

# 5. Criar App Service Plan (Windows)
az appservice plan create --name $APP_PLAN --resource-group $RG --location $LOCATION --sku B1

# 6. Criar Web App (.NET 8)
az webapp create --resource-group $RG --plan $APP_PLAN --name $APP_NAME --runtime "dotnet:8"

# 7. Configurar string de conexão no App Service
az webapp config connection-string set --resource-group $RG --name $APP_NAME --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:$SQL_SERVER.database.windows.net,1433;Initial Catalog=$SQL_DB;Persist Security Info=False;User ID=$SQL_ADMIN;Password=$SQL_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# 8. Publicar a API .NET (execute localmente este comando para gerar arquivos para deploy)
dotnet publish -c Release -o ./publish

# 9. Compactar arquivos publicados em ZIP (PowerShell/Windows)
# Se estiver no Windows PowerShell, use:
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip

# Se estiver no Linux/Mac, use:
# zip -r app.zip ./publish

# 10. Deploy do ZIP para o App Service
az webapp deployment source config-zip --resource-group $RG --name $APP_NAME --src ./app.zip

# 11. Acesse o Swagger da API publicada
echo "Acesse: https://$APP_NAME.azurewebsites.net/swagger"