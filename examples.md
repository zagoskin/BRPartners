# Examples to try the API
  
Para rodar o exemplo, � necess�rio ter o docker e docker-compose instalados e rodar o comando `docker-compose up` na raiz do projeto.
Tamb�m � necess�rio rodar a VendorManagementAPI, e � recomendado ter os exemplos de microservi�os tamb�m para visualizar
a comunica��o entre eles.

## Create a valid contract

Representa um fluxo de cria��o normal de um contrato. Um 'domain event' � disparado e fica salvo no banco de dados.

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

� possivel visualizar o evento criado usando o endpoint **GET** [/outbox/domain-events](/outbox/domain-events) 
j� que o servi�o de background que processa eles tem um delay configurado manualmente.

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

A resposta � um erro de valida��o, seguindo o padr�o de erros ProblemDetails. No caso � usada uma extens�o,
com o objeto ValidationProblemDetails, que permite retornar erros de valida��o num formato de `Dictionary<string, string[]>`.

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

> Todos os erros s�o tratados e retornados no formato ProblemDetails, seguindo o padr�o da RFC 7807

## Invalid data

Existem duas camadas de valida��o. No pipeline do MediatR existe um validador que apenas filtra valores que 
s�o mais "obviamente" inv�lidos, como valores nulos ou vazios. J� no domain, foi escolhido o padr�o factory
para encapsular a cria��o de um contrato, e nesse ponto � feita a valida��o de regras de neg�cio.

#### Request

> Esse request apenas � filtrado no pipeline. N�o � validado o valor negativo de `estimatedValue` de prop�sito
para testar depois a valida��o dele no modelo de domain junto com a data.

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
      "'Title' no deber�a estar vac�o."
    ],
    "DeadLineUtc": [
      "DeadLineUtc is required"
    ]
  }
```

#### Request

> Esse request passa o pipeline. Mas � invalidado pelo modelo do domain.

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
Os erros de domain usam codigos de erro customizados, para que o cliente possa tratar de forma mais espec�fica se quiser.

### Followup

O unico domain event implementado � o `ContractCreatedDomainEvent` e o handler dele eventualmente transforma ele
numa vers�o de integra��o para ser consumido por outros servi�os. � publicado num exchange de RabbitMQ usando fanout, pelo que n�o sera consumido
por ningu�m se n�o estiver declarada uma fila que processe ele do exchange.