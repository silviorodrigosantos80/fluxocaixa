# fluxocaixa
# FluxoCaixa – Arquitetura Escalável e Resiliente

## Objetivo

Desenvolver uma arquitetura de software escalável, resiliente e segura para:

- Serviço de controle de lançamentos (débitos e créditos)
- Serviço de saldo diário consolidado

O foco do projeto é demonstrar capacidade de tomada de decisão arquitetural, aplicação de boas práticas, domínio de requisitos não funcionais e desenho de solução desacoplada.

---

# Visão Arquitetural

A solução foi construída utilizando:

- Microservices
- Clean Architecture
- DDD (Domain-Driven Design)
- Event-Driven Architecture
- CQRS
- Outbox Pattern
- Idempotência
- Retry Exponencial
- Banco isolado por serviço
- Testes unitários

A comunicação entre serviços é assíncrona via mensageria.

---

# Stack Tecnológica

## Linguagem e Plataforma
- .NET 8
- C#

## Banco de Dados
- PostgreSQL (isolado por serviço)

## Mensageria
- RabbitMQ (self-hosted)

## Segurança
- Keycloak (OpenID Connect / OAuth2)
- JWT

## Testes
- xUnit
- Moq
- FluentAssertions

## Infraestrutura Local
- Docker Compose

---

# Princípios Aplicados

## SOLID
- SRP: Camadas bem definidas
- DIP: Uso de interfaces e abstrações
- OCP: Handlers extensíveis
- ISP: Interfaces específicas
- LSP: Implementações substituíveis

## Clean Architecture
- Domain sem dependência externa
- Application isolada
- Infrastructure plugável

## Event-Driven
- Comunicação assíncrona
- Desacoplamento entre serviços
- Eventual consistency

---

# Fluxo de Processamento

## Lançamentos
POST /lancamentos
↓
Aggregate dispara DomainEvent
↓
Outbox persiste evento
↓
Worker publica IntegrationEvent
↓
RabbitMQ


## Consolidado
RabbitMQ
↓
LancamentoCriadoConsumer
↓
MediatR
↓
ProcessarLancamentoIntegrationCommandHandler
↓
Atualiza SaldoDiario

# Requisitos Não Funcionais

## Resiliência

- Lançamentos não dependem do Consolidado
- Outbox garante publicação confiável
- Idempotência evita duplicação
- Retry exponencial trata falhas transitórias

Detalhes: `docs/resiliencia.md`


## Escalabilidade – 50 req/s

- Prefetch configurado
- Processamento concorrente controlado
- Worker escalável horizontalmente
- Banco isolado

Detalhes: `docs/escalabilidade.md`


# Segurança

- Autenticação via JWT
- Integração com Keycloak
- Role: comerciante
- Multi-tenant via claim `sub`

Detalhes: `docs/keycloak.md`


# Observabilidade

- Logs estruturados
- Tratamento global de exceções
- Preparado para integração com OpenTelemetry

Detalhes: `docs/observabilidade.md`


# Como Executar o Projeto

Este guia descreve como subir e testar a aplicação a partir da raiz do projeto (`FluxoCaixa`).

---

## Pré-requisitos

Certifique-se de possuir instalado:

- .NET 8 SDK
- Docker
- Docker Compose
- Git

Opcional (para inspeção):
- Postman ou Insomnia
- DBeaver ou similar (para banco)

---

# Como Executar o Projeto

Este guia descreve como subir, configurar e testar a aplicação a partir da raiz do projeto (`FluxoCaixa`).

---

## Pré-requisitos

Certifique-se de possuir instalado:

- .NET 8 SDK
- Docker
- Docker Compose
- Git

Opcional (para testes manuais e inspeção):
- Postman ou Insomnia
- DBeaver ou outro cliente SQL

---

# Subir Infraestrutura

A aplicação depende dos seguintes serviços:

- PostgreSQL (Lançamentos)
- PostgreSQL (Consolidado)
- RabbitMQ
- Keycloak

A partir da raiz do projeto execute:

```bash
docker compose up -d 

ou 

docker-compose -f docker/docker-compose.yml up -d --build
```

Verifique se os containers estão rodando:

```bash
docker ps
```

---

# Configurar Keycloak

Após subir os containers:

1. Acesse:  
   http://localhost:8080

2. Faça login como administrador  
   (credenciais definidas no `docker-compose.yml`).

3. Importe o realm disponível em:

```
docker/keycloak/realm-export.json
```

Isso criará automaticamente:

- Realm configurado
- Client da aplicação
- Role `comerciante`
- Configuração JWT

Detalhes: `docs/keycloak.md`
---

# Aplicar Migrations

A partir da raiz do projeto execute:

```bash
dotnet ef database update --project src/Services/Lancamentos/FluxoCaixa.Lancamentos.Infrastructure
dotnet ef database update --project src/Services/Consolidado/FluxoCaixa.Consolidado.Infrastructure
```
Após subir, acesse via navegador:

- Swagger Lançamentos
- Swagger Consolidado

---

# Executar Testes

Para rodar todos os testes:

```bash
dotnet test --no-build -v minimal
```

Executar apenas Integration Tests:

```bash
dotnet test tests/FluxoCaixa.IntegrationTests --no-build -v minimal
```

Executar apenas Unit Tests:

```bash
dotnet test tests/FluxoCaixa.UnitTests --no-build -v minimal
```

---

# Fluxo Esperado da Aplicação

1. Criar lançamento via API.
2. Evento salvo na tabela Outbox.
3. Worker publica evento no RabbitMQ.
4. Consolidado consome evento.
5. Saldo diário é atualizado.

---

# Observações Importantes

- O sistema opera sob **Eventual Consistency**.
- O Consolidado pode apresentar pequeno atraso na atualização do saldo.
- Caso o Worker esteja offline, os eventos permanecem na fila.
- A arquitetura é desacoplada e resiliente.
- Cada serviço possui banco de dados isolado.

---

# Endpoints e Serviços Úteis

RabbitMQ Management:
```
http://localhost:15672
```

Keycloak:
```
http://localhost:8080
```

Portainer:
```
http://localhost:9000
```

Lançamento API:
```
http://localhost:5001
```

Consolidado API:
```
http://localhost:5002
```
---

# Encerramento

Após seguir os passos acima:

- APIs estarão disponíveis via Swagger
- Mensageria estará ativa
- Segurança configurada
- Testes poderão ser executados
- Fluxo completo estará funcional


## Documentação Técnica

| Caminho                     | Descrição                                                                 |
|-----------------------------|---------------------------------------------------------------------------|
| `docs/architecture.md`     | Detalhamento completo da arquitetura adotada, padrões utilizados e trade-offs. |
| `docs/outbox.md`           | Explicação do Outbox Pattern e garantia de entrega confiável de eventos. |
| `docs/resiliencia.md`      | Estratégias de resiliência, retry, idempotência e consistência eventual. |
| `docs/escalabilidade.md`   | Abordagem para suportar 50 req/s, concorrência e escala horizontal. |
| `docs/keycloak.md`         | Configuração de autenticação, autorização e independência de cloud provider. |
| `docs/observabilidade.md`  | Estratégia de logs, monitoramento e evoluções futuras de observabilidade. |
