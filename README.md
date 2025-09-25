# GeoSense API - Entrega DevOps Tools & Cloud Computing

## 👥 Integrantes

- **Enzo Giuseppe Marsola** – RM: 556310, Turma: 2TDSPK  
- **Rafael de Souza Pinto** – RM: 555130, Turma: 2TDSPY  
- **Luiz Paulo F. Fernandes** – RM: 555497, Turma: 2TDSPF

---

## 1️⃣ Descrição da Solução

GeoSense API é uma aplicação RESTful desenvolvida em .NET, destinada ao gerenciamento de motos, vagas, pátios e usuários em ambientes de manutenção ou estacionamento. A solução permite realizar operações completas de cadastro, consulta, atualização e remoção (CRUD) sobre as principais entidades do sistema, com integração total a um banco de dados na nuvem (Azure SQL) e publicação via App Service na Azure.

---

## 2️⃣ Benefícios para o Negócio

A GeoSense API resolve problemas de controle e rastreabilidade de ativos em pátios e oficinas, garantindo:
- Gestão centralizada e confiável de motos, vagas e usuários.
- Redução de erros e retrabalho com dados sempre disponíveis.
- Facilidade para consulta, alocação e monitoramento de status em tempo real.
- Otimização dos processos operacionais e tomada de decisão através de dados agregados (dashboard).

---

## 3️⃣ Banco de Dados em Nuvem

- **Tecnologia utilizada:** Azure SQL Database (PaaS)
- **Criação automática via Azure CLI**
- **Script DDL:** [script_bd.sql](GeoSense.API/db/script_bd.sql)
- **Não são utilizados bancos H2 ou Oracle FIAP.**

---

## 4️⃣ CRUD Completo

A aplicação implementa CRUD completo das entidades principais (Moto, Vaga, Patio, Usuário):
- **Inclusão:** POST
- **Consulta:** GET (listagem e por ID)
- **Alteração:** PUT
- **Exclusão:** DELETE

Todos os métodos são acessíveis via Swagger e testáveis em ambiente de nuvem.

---

## 5️⃣ Manipulação de Registros Reais

- Durante o vídeo de entrega, será demonstrada a inserção, atualização, consulta e exclusão de **pelo menos 2 registros reais** (exemplo: motos cadastradas) diretamente no banco de dados em nuvem via API.

---

## 6️⃣ Código-fonte no GitHub

Este repositório contém **todo o código-fonte** necessário para execução e testes da aplicação, incluindo:
- API principal (.NET)
- Testes automatizados
- Scripts de banco e deploy
- Arquivo de configuração (`appsettings.json`)

---

## 7️⃣ Passo a Passo para Deploy e Testes

### ⚡ Requisitos

- Azure CLI instalado e autenticado (`az login`)
- .NET SDK 8.0+
- Permissão para criar recursos no Azure

### 🚀 Deploy na Azure (App Service + SQL Database)

1. **Clone este repositório**
   ```bash
   git clone https://github.com/MarsoL4/geosense-api-cloud.git
   cd geosense-api-cloud
   ```

2. **Execute o script de criação dos recursos via Azure CLI**
   ```bash
   bash scripts/scripts_azure.sh
   ```
   > **Obs:** Execute localmente os comandos de publish e zip conforme o script.

3. **Acesse o Swagger da API publicada**
   ```
   https://geosense-app.azurewebsites.net/swagger
   ```
   - Teste todos os endpoints CRUD conforme exemplos abaixo.

### 📄 Scripts Azure CLI
- Todos os comandos de criação/configuração dos recursos estão em [`scripts/scripts_azure.sh`](scripts/scripts_azure.sh)

### 🗄️ Script DDL do Banco
- DDL completo disponível em [`GeoSense.API/db/script_bd.sql`](GeoSense.API/db/script_bd.sql)

---

## 8️⃣ Exemplos de Uso (JSON para testes)

### Moto (CRUD)
```json
POST /api/moto
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
POST /api/vaga
{
  "numero": 101,
  "tipo": 0,
  "status": 0,
  "patioId": 1
}
```

### Usuário (CRUD)
```json
POST /api/usuario
{
  "nome": "Rafael de Souza Pinto",
  "email": "rafael.pinto@exemplo.com",
  "senha": "12345678",
  "tipo": 0
}
```

---

## 🔗 Link do Vídeo
- **Vídeo Demonstrativo:** [Link do vídeo YouTube](https://youtube.com/)

---

## 🏗️ Arquitetura da Solução

- App Service (Azure) hospedando a API .NET
- Azure SQL Database (PaaS) como persistência
- Recursos criados e configurados **100% via Azure CLI**
- Testes CRUD demonstrando integração total entre API e Banco em nuvem

<details>
  <summary><b>Fluxo de Funcionamento</b></summary>

  1. Usuário acessa a API via Swagger ou HTTP.
  2. Realiza operações CRUD sobre motos, vagas, pátios e usuários.
  3. Dados trafegam pela API .NET hospedada no App Service.
  4. Persistência e consultas ocorrem diretamente no Azure SQL Database.
  5. Resultados apresentados em tempo real, inclusive dashboard agregado.

</details>

---

## 🧪 Testes Automatizados

Para rodar todos os testes unitários:
```bash
cd GeoSense.API.Tests
dotnet test
```

---

## 📑 Observações Finais

- Todos os recursos Azure criados via CLI (conforme [scripts/scripts_azure.sh](scripts/scripts_azure.sh))
- Script DDL entregue conforme padrão ([GeoSense.API/db/script_bd.sql](GeoSense.API/db/script_bd.sql))
