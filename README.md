# 📦 OrderProcessing

Este projeto simula um sistema backend assíncrono para processamento de pedidos utilizando RabbitMQ, seguindo boas práticas de **Clean Architecture**, **CQRS**, e injeção de dependência. O status dos pedidos é mantido **em memória**, sem banco de dados.

---

## 🎯 Objetivo

* Criar pedidos via API HTTP
* Publicar pedidos em uma fila RabbitMQ
* Consumir e processar pedidos com delay artificial de 2 segundos
* Consultar o status atual de um pedido (`Pendente` ou `Processado`)

---

## 🧩 Arquitetura

A aplicação segue os princípios da **Clean Architecture**, com separação clara entre domínio, aplicação, infraestrutura e apresentação (API). Cada camada depende apenas da anterior.

### 📐 Diagrama de Arquitetura

```OrderProcessing/
├───OrderProcessing.API/
├───OrderProcessing.Application/
│   ├───Commands/
│   └───Queries/
├───OrderProcessing.Domain/
├───OrderProcessing.Infrastructure/
└───OrderProcessing.Tests/
```

---

## 🔧 Tecnologias Utilizadas

* **.NET 8 / ASP.NET Core**
* **RabbitMQ 7+** com:

  * Retry (reentrega programada)
  * Dead Letter Queue (DLQ)
* **CQRS** com separação de comandos e queries
* **AutoMapper** para transformação entre DTOs e entidades
* **Serilog** para logging estruturado

---

## 🚀 Como Executar Localmente

### Requisitos

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/) para subir o RabbitMQ

### 1. Clone o repositório

```bash
git clone https://github.com/saulo-de-tarso/OrderProcessing.git
cd OrderProcessing
```

### 2. Rode o RabbitMQ com Docker

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

RabbitMQ estará em:

* UI: [http://localhost:15672](http://localhost:15672)
* Login: `guest` / `guest`
* Fila: `order_processing_queue`

### 3. Execute a aplicação

```bash
dotnet build
cd src/OrderProcessing.API
dotnet run
```

* A aplicação estará acessível em: [http://localhost:5098](http://localhost:5098)
---

## 🧪 Endpoints

### Criar Pedido

**Descrição:**  Cria um pedido para um cliente com uma lista de itens. Ambos campos são obrigatórios.  
**Retorna:**  Response header com Id da ordem criada. **Utilizar o Id gerado para testar o Endpoint de Consultar Pedido**.
```http
POST /api/Orders
Content-Type: application/json

{
  "clientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "items": [
    "10 bolovos"
  ]
}
```

### Consultar Pedido

**Descrição:**  Retorna o status atual do pedido: Pendente ou Processado.

```http
GET api/Orders/{id}
```

Resposta:

```json
{
  "id": "605f1bdb-b834-4a87-b27d-8cb36c62fc84",
  "status": "Processado"
}
```

---



## 🧠 Retry + DLQ

O consumidor RabbitMQ possui:

* ✅ **Retry**: mensagens reprocessadas após falha
* 🧨 **DLQ (Dead Letter Queue)**: mensagens que excedem 3 tentativas são redirecionadas para análise

Esses mecanismos aumentam a tolerância a falhas e previnem perda de dados.

---

## 📌 Boas Práticas Aplicadas

* Clean Architecture e separação de responsabilidades
* Dependency Injection com `IServiceCollection`
* `IOptions<T>` para configuração fortemente tipada
* Serilog estruturado com contexto
* CQRS com `CommandHandler` e `QueryHandler`
* Injeção de IMapper para conversão entre camadas

---


