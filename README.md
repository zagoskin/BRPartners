# BRPartners "VendorManagement"

O sistema simula de forma simplificada a necessidade de uma empresa que trabalha com outras empresas externas "Vendors" de poder administrar contratos que eles tem com cada uma delas. Não foram implementadas todas as operações de CRUD dos modelos de dominio para agilizar a demo. Apenas existem os necessários para poder visualizar um fluxo básico.

# Estrutura

- [VendorManagement](#serviço-principal-vendormanagement)
- [Mailer](#serviço-mailer)
- [Shared](#shared)
- [Solution Items](#solution-items)
  - [Docker](#sobre-o-docker-compose)
- [Executando localmente](#executando-localmente)
  - [Teste de fluxos](#teste-de-fluxos)

## Serviço principal VendorManagement

A solução é um Web API desenvolvido com .NET 6, usando Clean Architecture dividindo as diferentes partes:

- API
  - Definições de Controllers com as diferentes operações
  - Mappings de modelos internos a modelos a ser expostos aos consumidores
  - Configurações particulares da presentação
  - Mappings de erros
- Application
  - Contém os diferentes casos de uso da aplicação
    - ListVendors
    - GetVendorById
    - GetContractById
    - ListContracts
    - CreateContract
    - FakeUserService (simulando um serviço que obtém o usuário logado, mas retorna sempre o mesmo usuário)
  - Definições de abstrações a ser implementadas
- Domain
  - Contém modelos base e abstrações para definição e identificação de objetos
  - Definições de modelos concretos de dominio
    - Eles encapsulam a logica de criação usando Factory static methods
  - Cada aggregate esta definido numa pasta, e nela esta tudo o que refer especificamente a ele, tendo definições de entidades da qual o aggregate é dono e value objects especificos
- Infrastructure
  - Definição de mecanismo de persistencia
    - EF Core in-memory (foi usado SQL Server só para simular criação de migrations)
    - Configurations para cada aggregate (para simular migrations)
    - Interceptors para mecanismo de OutboxPattern (para garantir entrega ou log de erros de propagação de eventos)
    - Configuração interna do outbox
    - Contém handlers **test** apenas para poder visualizar os outbox events, sendo algo que não existiria na prática
    - Implementação de UnitOfWork
    - Data seed para Vendors
  - Definição de mecanismo de integração
    - Mapeamento de domain events a integration events
    - Serviço de publicação de integration events usando um exchange de RabbitMQ
    - BackgroundService que publica constantemente domain events aos MediatR handlers
    - BackgroundService que publica, usando o serviço prévio, os integration events ao RabbitMQ
    - As configurações de nomes foram implementadas com Options Pattern

Cada projeto tem um arquivo que expõe o metodo necessário para usar dependency injection, sendo que cada projeto é responsável pelo cadastro dos serviços necessários que ele implementa. No caso do projeto de Infrastructure também foi implementado um método de "setup" para o seed do banco de dados.

### Projeto de unit tests

O VendorManagement conta com um projeto de Unit Tests na pasta "tests" interna dele. O projeto usa xUnit, FluentAssertions e NSubstitute (para mock de dependências externas)

Ele possui a seguinte estrutura pastas, e em cada uma delas existe um factory para facilitar a criação de objetos:

- Contracts
  - Aggregate (domain model tests)
  - Commands (create contract tests)
  - Queries (getbyid e list tests)
- Vendors
  - Commands (create contract tests)
  - Queries (getbyid e list tests)

## Serviço mailer

Uma Minimal API que simula um microserviço que continuamente escuta numa fila de RabbitMQ que esta suscrita ao Exchange onde o VendorManagement publica os seus eventos. O objetivo desta API é o envio de emails, notificando da criação de contratos. Os emails não são realmente enviados, senão salvos num server de emails usando um container de **papercut**.

A estrutura respeita o mesmo padrão de Clean Architecture, sendo simplificado em apenas pastas.

- Endpoints
  - Facilita um endpoint para visualizar os emails que foram processados
- Application
  - Integrations
    - Handler de evento de contrato criado, que salva um objeto de outbox
  - Mails
    - Commands
      - SendEmailCommand
    - Queries
      - ListEmailsQuery
- Domain
  - Definição do modelo OutboxEmail para garantir envio ou log de erros de emails
- Infrastructure
  - Mecanismo de persistence com MongoDB
  - BackgroundServices
    - Consumer que obtém do RabbitMQ os integration events e propaga eles pelo serviço
    - EmailSender que constantemente verifica se existem emails a ser enviados, dispara o comando e atualiza a DB
  - Services
    - Serviço de outbox (representando um "repository")
    - Serviço de persistence na collection de MongoDB

## Shared

Simula um package comum onde seriam publicados os "contratos" (requests, responses, messages) de cada projeto, para manter uma interface uniforme. Também define algumas abstrações a ser aproveitadas pelos consumidores

## Solution Items

Os arquivos dentro da "pasta" Solution Items existem físicamente na raíz da solução. Contém 4 arquivos:

- README.md
- examples.md
- .editorconfig (regras básicas e conventions para programação)
- docker-compose.yml

### Sobre o docker-compose

Os serviços de **RabbitMQ**, **MongoDB** e **Papercut** são images buildadas em conjunto usando a funcionalidade do docker-compose. As images do RabbitMQ e Papercut possuim um port para entrar num website e poder visualizar os dados que estão sendo criados.

# Executando localmente

## Criando os containers

Para executar a solução localmente, primeiro é necessário ter instalado [docker](https://docs.docker.com/engine/install/) e [docker-compose](https://docs.docker.com/compose/install/), depois, se deve posicionar com um terminal na pasta onde fica o arquivo `docker-compose.yml` e rodar o comando

> docker-compose up

Pode ser usado o argumento `--detach` para começar os containers em background.

## VendorManagement

Posicionado na pasta que contém o projeto de API `BRP.VendorManagement.API.csproj` usar o comando
(_VendorManagement\src\BRP.VendorManagement.API\BRP.VendorManagement.API.csproj_)

> dotnet run

Também desde Visual Studio é possivel apertar clic direito no projeto

> Debug -> Start Without Debugging

Um browser deveria ser aberto, ou pode ser acessado manualmente seguindo o link

- https://localhost:7144/swagger/index.html

## Mailer

Posicionado na pasta que contém o projeto de API `BRP.Mailer.API.csproj` usar o comando
(_"C:\Users\toto\_\_jjqufpv\source\labs\BRPartners\Mailer\src\BRP.Mailer.API\BRP.Mailer.API.csproj"_)

> dotnet run

Também desde Visual Studio é possivel apertar clic direito no projeto

> Debug -> Start Without Debugging

Um browser deveria ser aberto, ou pode ser acessado manualmente seguindo o link

- https://localhost:7015/swagger/index.html

## Teste de fluxos

Ir a [exemplos](./examples.md)
