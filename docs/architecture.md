# Arquitetura da Solução – FluxoCaixa

## Visão Geral

A solução foi projetada com foco em:

- Escalabilidade
- Resiliência
- Alta disponibilidade
- Segurança
- Independência de vendor (cloud agnostic)
- Clareza de responsabilidades

A arquitetura combina:

- Microservices
- Clean Architecture
- DDD (Domain-Driven Design)
- Event-Driven Architecture
- CQRS
- Outbox Pattern

---

# Estrutura Arquitetural

## Separação por Serviço

A solução é composta por dois serviços principais:

- **Serviço de Lançamentos**
- **Serviço de Consolidado Diário**

Cada serviço possui:

- Domain
- Application
- Infrastructure
- API
- Worker (quando necessário)

Cada serviço possui banco de dados isolado.

---

# Clean Architecture

Cada serviço segue a estrutura:
Domain
Application
Infrastructure
API / Worker


### Domain
- Entidades
- Value Objects
- Regras de negócio
- Domain Events
- Interfaces de repositório

Não possui dependência externa.

---

### Application
- Commands
- Handlers (MediatR)
- Casos de uso
- Abstrações

Orquestra o domínio, mas não contém regras de infraestrutura.

---

### Infrastructure
- EF Core
- Repositórios concretos
- Mensageria
- Persistência
- Implementações técnicas

Depende do Domain e Application.

---

### API / Worker
- Camada de entrada
- Configuração de DI
- Exposição HTTP
- Configuração do broker
- Hospedagem

---

# Domain-Driven Design (DDD)

## Aggregate Root

`Lancamento` é modelado como Aggregate Root.

Toda modificação passa por ele.

---

## Value Object

`Valor` encapsula regras de validação monetária.

---

## Domain Events

Ao criar um lançamento:
LancamentoCriadoDomainEvent


É disparado dentro do Aggregate.

Isso permite desacoplamento interno.

---

# Event-Driven Architecture

A comunicação entre serviços é baseada em eventos.

## Fluxo Geral

É disparado dentro do Aggregate.

Isso permite desacoplamento interno.

---

Lançamentos
↓
DomainEvent
↓
Outbox
↓
RabbitMQ
↓
Consolidado


Não há chamadas HTTP entre serviços.

---

## Eventual Consistency

O sistema opera sob consistência eventual:

- O saldo pode ter pequeno atraso.
- Ganha-se alta disponibilidade.
- Não há transações distribuídas.

Trade-off consciente.

---

# Outbox Pattern

## Problema

Evitar inconsistência entre:

- Persistência do lançamento
- Publicação do evento

## Solução

- Evento salvo na mesma transação do Aggregate.
- Worker publica posteriormente.
- Garante entrega confiável.

---

# Segurança

## Autenticação

- JWT
- Keycloak (OpenID Connect)

## Autorização

- Role: comerciante
- Multi-tenant via claim `sub`

---

# Escalabilidade

## Estratégias aplicadas

- Mensageria assíncrona
- Prefetch configurado
- ConcurrentMessageLimit
- Banco isolado
- Worker escalável horizontalmente

## Escala Horizontal

O Worker pode ser replicado.

RabbitMQ distribui mensagens automaticamente.

---

# Resiliência

- Retry exponencial
- Idempotência via tabela `ProcessedEvents`
- Comunicação assíncrona
- Sem dependência direta entre serviços

Se Consolidado cair:

- Lançamentos continuam funcionando.
- Eventos permanecem na fila.
- Processamento ocorre posteriormente.

---

# CQRS

Separação de responsabilidades:

- Commands modificam estado.
- Queries consultam estado.

Handlers são isolados por caso de uso.

---

# Observabilidade (Preparado para)

- Logs estruturados
- Middleware de tratamento global
- Preparado para OpenTelemetry

---

# Independência de Cloud

A arquitetura foi desenhada para ser agnóstica de cloud provider.

## Escolhas

- RabbitMQ self-hosted
- PostgreSQL
- Keycloak
- Docker Compose

## Motivo

Evitar dependência de:

- Azure Service Bus
- Azure Entra ID
- AWS SQS
- AWS Cognito

A solução pode ser migrada para qualquer provedor sem impacto no domínio.

---

# Trade-offs Conscientes

| Decisão | Benefício | Trade-off |
|----------|------------|-----------|
| Event-Driven | Desacoplamento | Consistência eventual |
| Banco isolado | Independência | Sem transações globais |
| Outbox | Confiabilidade | Complexidade adicional |
| Mensageria | Escalabilidade | Operação do broker |

---

# Conclusão

A arquitetura foi desenhada com foco em:

- Separação clara de responsabilidades
- Independência tecnológica
- Escalabilidade horizontal
- Resiliência a falhas
- Consistência eventual controlada
- Aplicação de boas práticas de engenharia de software

A solução está preparada para evolução futura sem comprometer os princípios arquiteturais adotados.
