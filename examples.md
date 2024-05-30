# Examples to try the API
  
Para rodar o exemplo, é necessário ter o docker e docker-compose instalados e rodar o comando `docker-compose up` na raiz do projeto.
Também é necessário rodar a VendorManagementAPI, e é recomendado ter os exemplos de microserviços também para visualizar
a comunicação entre eles.

## Create a valid contract

Representa um fluxo de criação normal de um contrato. Um 'domain event' é disparado e fica salvo no banco de dados.

#### Request
```json
// POST /contracts
{
    "vendorId": "992a4f6a-4010-44ec-83b2-3718bb9e6e58",
    "title": "My first contract",
    "deadLineUtc": "2030-05-30",
    "estimatedValue": 1000000
}
```

#### Response 201

```json
{
    "id": "00000000-0000-0000-0000-000000000000", // some guid
    "vendorId": "992a4f6a-4010-44ec-83b2-3718bb9e6e58",
    "title": "My first contract",
    "deadLineUtc": "2030-05-30",
    "estimatedValue": 1000000
}
```

### Followup

É possivel visualizar o evento criado usando o endpoint **GET** [/outbox/domain-events](/outbox/domain-events) 
já que o serviço de background que processa eles tem um delay configurado manualmente.

> Esse controller foi facilitado apenas para fins de fazer essa demo

## Create an invalid contract

### Vendor not found
---

#### Request
```json
{
    "vendorId": "992a1111-4010-44ec-83b2-3718bb9e6e58",
    "title": "My first contract",
    "deadLineUtc": "2030-05-30",
    "estimatedValue": 1000000
}
```

A resposta é um erro de validação, seguindo o padrão de erros ProblemDetails. No caso é usada uma extensão,
com o objeto ValidationProblemDetails, que permite retornar erros de validação num formato de `Dictionary<string, string[]>`.

#### Response
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-2b0408e099f9068b94336cef0ecfb9e0-2adf6dbdc1e4efc0-00",
  "errors": {
    "General.Validation": [
      "Vendor '992a1111-4010-44ec-83b2-3718bb9e6e58' is invalid"
    ]
  }
}
```

> Todos os erros são tratados e retornados no formato ProblemDetails, seguindo o padrão da RFC 7807

## Invalid data

Existem duas camadas de validação. No pipeline do MediatR existe um validador que apenas filtra valores que 
são mais "obviamente" inválidos, como valores nulos ou vazios. Já no domain, foi escolhido o padrão factory
para encapsular a criação de um contrato, e nesse ponto é feita a validação de regras de negócio.

#### Request

> Esse request apenas é filtrado no pipeline. Não é validado o valor negativo de `estimatedValue` de propósito
para testar depois a validação dele no modelo de domain junto com a data.

```json
{
    "vendorId": "992a4f6a-4010-44ec-83b2-3718bb9e6e58",
    "title": "",
    "deadLineUtc": "0001-01-01",
    "estimatedValue": -5
}
```

#### Response 400
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-35aeaa9c083089965b1cfd862500a56f-c5f8b2fef91521e7-00",
  "errors": {
    "Title": [
      "'Title' no debería estar vacío."
    ],
    "DeadLineUtc": [
      "DeadLineUtc is required"
    ]
  }
```

#### Request

> Esse request passa o pipeline. Mas é invalidado pelo modelo do domain.

```json
{
    "vendorId": "992a4f6a-4010-44ec-83b2-3718bb9e6e58",
    "title": "My first invalid contract",
    "deadLineUtc": "2010-01-01",
    "estimatedValue": -5
}
```

#### Response 400
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "traceId": "00-89717efed139ebf4c374a07c8a56e6ac-d8112a81e9ed46dc-00",
  "errors": {
    "Contract.DeadLineMustBeInTheFuture": [
      "DeadLine must be in the future"
    ],
    "Contract.EstimatedValueMustBeGreaterThanZero": [
      "EstimatedValue must be greater than 0"
    ]
  }
}
```
Os erros de domain usam codigos de erro customizados, para que o cliente possa tratar de forma mais específica se quiser.

### Followup

O unico domain event implementado é o `ContractCreatedDomainEvent` e o handler dele eventualmente transforma ele
numa versão de integração para ser consumido por outros serviços. É publicado num exchange de RabbitMQ usando fanout, pelo que não sera consumido
por ninguém se não estiver declarada uma fila que processe ele do exchange.