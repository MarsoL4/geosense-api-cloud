# GeoSense API - DevOps Tools & Cloud Computing 🚀

## 👤 Integrantes

- **Enzo Giuseppe Marsola** – RM: 556310, Turma: 2TDSPK  
- **Rafael de Souza Pinto** – RM: 555130, Turma: 2TDSPY  
- **Luiz Paulo F. Fernandes** – RM: 555497, Turma: 2TDSPF

---

## 💡 Descrição da Solução

GeoSense API é uma aplicação RESTful em .NET 8 para gerenciamento de motos, vagas, pátios e usuários em ambientes de manutenção ou estacionamento. Permite operações completas de cadastro, consulta, atualização e remoção (CRUD) sobre as principais entidades do sistema, com integração total ao banco de dados em nuvem (Azure PostgreSQL Flexible Server) e publicação via contêiner Docker no Azure Web App, com CI/CD automatizado pelo Azure DevOps.

---

## 💼 Benefícios para o Negócio

A GeoSense API resolve problemas de controle e rastreabilidade de ativos em pátios e oficinas, oferecendo:
- Gestão centralizada e confiável de motos, vagas e usuários.
- Redução de erros e retrabalho, dados sempre disponíveis.
- Facilidade para consulta, alocação e monitoramento de status em tempo real.
- Otimização operacional e tomada de decisão por dados agregados (dashboard).

---

## 🗄️ Banco de Dados em Nuvem

- **Tecnologia:** Azure PostgreSQL Flexible Server (PaaS)
- **Provisionamento automático via script Bash + Azure CLI**
- **Migrations EF Core:** Primeiro deploy totalmente automatizado pela pipeline (`dotnet ef database update`)
- **Scripts de infraestrutura:** [`scripts/infra_deploy.sh`](scripts/infra_deploy.sh)

---

## 🛠️ Conteúdo do Repositório

Este repositório contém:
- [Código-fonte da API (.NET)](GeoSense.API)
- [Testes automatizados (`GeoSense.API.Tests`)](GeoSense.API.Tests)
- [Scripts de provisionamento e banco (`infra_deploy.sh`)](scripts/infra_deploy.sh)
- [Arquivos de configuração (`appsettings.json`)](GeoSense.API/appsettings.json)
- [Desenho de arquitetura da solução](arquitetura/diagrama-arquitetura.png)
- [Arquivo de CI/CD: `azure-pipelines.yml`](azure-pipelines.yml)

---

## ⚙️ Passo a Passo para Deploy e Testes

### Requisitos

- Azure CLI instalado e autenticado (`az login`)
- .NET SDK 8.0+
- Permissão para criar recursos na Azure

### 1. Clone o repositório

```bash
git clone https://github.com/MarsoL4/geosense-api-cloud.git
cd geosense-api-cloud
```

### 2. Provisionamento e Deploy Automatizado

#### Criação dos recursos na Azure

Execute o script para provisionar todos os recursos da nuvem (Resource Group, PostgreSQL Flexible Server, Container Registry, App Service Plan, Web App, variáveis seguras):

```bash
cd scripts
bash infra_deploy.sh
```

#### Variáveis importantes:
- **Usuário banco:** geosenseadmin
- **Senha:** SenhaForte123!
- **Banco:** geosense

#### Após o provisionamento:
- O pipeline CI/CD do Azure DevOps realiza build, testes automatizados, publish, migra o banco (EF Core migrations), faz build/push da imagem Docker no Azure Container Registry e faz deploy no Azure Web App (container).
- Secrets como string de conexão e API key são protegidos por Variable Groups no Azure DevOps.

### 3. Acesse o Swagger da API publicada

Exemplo (a URL do app service está definida em `geosense-app-s4`):

```
https://geosense-app-s4.azurewebsites.net/swagger
```

### 4. Exemplos de Uso (CRUD)

Veja as seções abaixo ou utilize o Swagger UI publicado.

---

## 📦 Exemplos de Uso (JSON para testes)

### Moto (CRUD)
```json
POST /api/v1/moto
{
  "modelo": "Honda CG 160",
  "placa": "ABC1D23",
  "chassi": "9C2JC4110JR000001",
  "problemaIdentificado": "Motor com ruído excessivo",
  "vagaId": 1
}
```

### Vaga (CRUD)
```json
POST /api/v1/vaga
{
  "numero": 101,
  "tipo": 0,
  "status": 0,
  "patioId": 1
}
```

### Usuário (CRUD)
```json
POST /api/v1/usuario
{
  "nome": "Rafael de Souza Pinto",
  "email": "rafael.pinto@exemplo.com",
  "senha": "12345678",
  "tipo": 0
}
```

### Pátio (CRUD)
```json
POST /api/v1/patio
{
  "nome": "Pátio Central"
}
```

### Dashboard (GET)
Resposta esperada:
```json
GET /api/v1/dashboard
{
  "totalMotos": 10,
  "motosComProblema": 2,
  "vagasLivres": 8,
  "vagasOcupadas": 2,
  "totalVagas": 10
}
```

---

## 📊 Testes Automatizados

Para rodar todos os testes unitários/integração localmente:
```bash
cd GeoSense.API.Tests
dotnet test
```
A pipeline do Azure DevOps executa esses testes automaticamente a cada push.

---

## 🤖 CI/CD: Azure DevOps Pipelines

- Build, testes, publicação e deploy automatizados a cada alteração em `main` ou `master`.
- Variáveis de ambiente (strings de conexão, API Key) protegidas por Variable Groups.
- Deploy via Docker no Azure Web App.
- Pipeline configurada em [`azure-pipelines.yml`](azure-pipelines.yml).

---

## 🏗️ Arquitetura da Solução

Abaixo está o desenho da arquitetura da solução, detalhando todos os recursos, fluxos e funcionamento após o deploy:

![Arquitetura GeoSense API](arquitetura/diagrama-arquitetura.png)

---
## 🎬 Link do Vídeo

- **Vídeo Demonstrativo:** (link será incluído após upload no Youtube)

---

## 🔒 Segurança e Boas Práticas

- Nenhuma credencial é exposta no código ou histórico de versões.
- Todas as configurações sensíveis (connection strings, API keys) ficam no Variable Group (DevOps) e AppSettings da Azure.
- Recomendado: crie variável de ambiente para "GeoSense-Api-Key" ao consumir a API.

---

## 🏁 Testando o CRUD online

- **Acesse o Swagger:**  
  https://geosense-app-s4.azurewebsites.net/swagger

- **No portal Azure**:  
  Acesse o banco de dados na nuvem e visualize as tabelas/crud em tempo real (veja o roteiro do vídeo para demonstração completa).

---

## 🧪 Testes Automatizados (Resumo)

- Testes unitários e de integração incluídos no projeto `GeoSense.API.Tests` rodando no pipeline CI.
- Relatório de testes disponível no Azure DevOps a cada execução.
