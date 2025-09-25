# GeoSense API

GeoSense API é uma solução RESTful desenvolvida em .NET para o gerenciamento de motos, vagas e usuários em ambientes de pátio, manutenção e estacionamento. O projeto utiliza arquitetura em camadas, Entity Framework Core, Oracle como banco de dados e documentação completa via Swagger/OpenAPI.

---

## 👥 Integrantes

- **Enzo Giuseppe Marsola** – RM: 556310, Turma: 2TDSPK  
- **Rafael de Souza Pinto** – RM: 555130, Turma: 2TDSPY  
- **Luiz Paulo F. Fernandes** – RM: 555497, Turma: 2TDSPF

---

## 🏗 Justificativa do Domínio e Arquitetura

O domínio foi escolhido para atender à necessidade de controle eficiente do fluxo de motos em pátios de manutenção, oficinas ou estacionamentos. O sistema permite cadastro, alocação e histórico de motos, gestão de vagas e controle de usuários com diferentes permissões.

A arquitetura segue boas práticas REST, separação de responsabilidades (camadas Controller, Service, Repository), e utiliza recursos avançados como paginação, HATEOAS, DTOs e exemplos interativos no Swagger.

---

## 🚀 Instruções de Execução

1. **Configuração do Banco:**  
   Edite o arquivo `appsettings.json` com sua string de conexão Oracle.

2. **Execução da API:**  
   ```
   dotnet run --project GeoSense.API
   ```
   Acesse a documentação Swagger em:  
   `http://localhost:5194/swagger` ou `https://localhost:7150/swagger`

3. **Rodar Testes Automatizados:**  
   ```
   dotnet test
   ```

---

## 🔑 Principais Entidades

- **Moto:** Controle de motos cadastradas, informações de placa, chassi, modelo e vaga alocada.
- **Vaga:** Gerenciamento de vagas disponíveis em pátios, incluindo status e tipo.
- **Usuário:** Cadastro de usuários do sistema, com controle de papéis (administrador, mecânico) e autenticação.

---

## 📑 Endpoints e Exemplos de Uso

### 🛵 Moto

#### Listar Motos (Paginação + HATEOAS)
- **GET** `/api/moto?page=1&pageSize=10`
- **Resposta:**
    ```json
    {
      "items": [
        {
          "id": 1,
          "modelo": "Honda CG 160",
          "placa": "ABC1D23",
          "chassi": "9C2JC4110JR000001",
          "problemaIdentificado": "Motor com ruído excessivo",
          "vagaId": 1
        }
      ],
      "totalCount": 1,
      "page": 1,
      "pageSize": 10,
      "links": [
        { "rel": "self", "method": "GET", "href": "/api/moto?page=1&pageSize=10" }
      ]
    }
    ```

#### Buscar Moto por ID
- **GET** `/api/moto/{id}`

#### Criar Moto
- **POST** `/api/moto`
- **Payload de exemplo:**
    ```json
    {
      "modelo": "Honda CG 160",
      "placa": "ABC1D23",
      "chassi": "9C2JC4110JR000001",
      "problemaIdentificado": "Motor com ruído excessivo",
      "vagaId": 1
    }
    ```

#### Atualizar Moto
- **PUT** `/api/moto/{id}`
- **Payload igual ao POST**

#### Remover Moto
- **DELETE** `/api/moto/{id}`

---

### 🅿️ Vaga

#### Listar Vagas (Paginação + HATEOAS)
- **GET** `/api/vaga?page=1&pageSize=10`
- **Resposta:**
    ```json
    {
      "items": [
        {
          "numero": 101,
          "tipo": 0,
          "status": 0,
          "patioId": 1
        }
      ],
      "totalCount": 1,
      "page": 1,
      "pageSize": 10,
      "links": [
        { "rel": "self", "method": "GET", "href": "/api/vaga?page=1&pageSize=10" }
      ]
    }
    ```

#### Buscar Vaga por ID
- **GET** `/api/vaga/{id}`

#### Criar Vaga
- **POST** `/api/vaga`
- **Payload de exemplo:**
    ```json
    {
      "numero": 101,
      "tipo": 0,
      "status": 0,
      "patioId": 1
    }
    ```

#### Atualizar Vaga
- **PUT** `/api/vaga/{id}`
- **Payload igual ao POST**

#### Remover Vaga
- **DELETE** `/api/vaga/{id}`

---

### 👤 Usuário

#### Listar Usuários (Paginação + HATEOAS)
- **GET** `/api/usuario?page=1&pageSize=10`

#### Buscar Usuário por ID
- **GET** `/api/usuario/{id}`

#### Criar Usuário
- **POST** `/api/usuario`
- **Payload de exemplo:**
    ```json
    {
      "nome": "Rafael de Souza Pinto",
      "email": "rafael.pinto@exemplo.com",
      "senha": "12345678",
      "tipo": 0
    }
    ```

#### Atualizar Usuário
- **PUT** `/api/usuario/{id}`
- **Payload igual ao POST**

#### Remover Usuário
- **DELETE** `/api/usuario/{id}`

---

### 📊 Dashboard

#### Dados Agregados do Sistema
- **GET** `/api/dashboard`
- **Resposta:**
    ```json
    {
      "totalMotos": 10,
      "motosComProblema": 2,
      "vagasLivres": 5,
      "vagasOcupadas": 5,
      "totalVagas": 10
    }
    ```

---

## 🧩 Swagger/OpenAPI

- Todos os endpoints possuem descrição, parâmetros documentados, exemplos de payload (POST/PUT) e modelos de dados.
- Acesse `/swagger` para explorar e testar a API interativamente.

---

## 🧪 Comando para rodar os testes

```bash
dotnet test
```

---

## 🏆 Exemplos de Modelos de Dados

### MotoDTO

```json
{
  "modelo": "Honda CG 160",
  "placa": "ABC1D23",
  "chassi": "9C2JC4110JR000001",
  "problemaIdentificado": "Motor com ruído excessivo",
  "vagaId": 1
}
```

### VagaDTO

```json
{
  "numero": 101,
  "tipo": 0,
  "status": 0,
  "patioId": 1
}
```

### UsuarioDTO

```json
{
  "nome": "Rafael de Souza Pinto",
  "email": "rafael.pinto@exemplo.com",
  "senha": "12345678",
  "tipo": 0
}
```
