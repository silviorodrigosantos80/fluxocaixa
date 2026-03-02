# Resiliência – FluxoCaixa

A resiliência da solução foi projetada para garantir:

- Alta disponibilidade
- Tolerância a falhas
- Continuidade de operação
- Recuperação automática após indisponibilidade

A arquitetura foi desenhada para evitar acoplamento direto entre serviços e eliminar dependências síncronas críticas.

---

# Estratégias Aplicadas na Implementação

## Comunicação Assíncrona

A comunicação entre os serviços **Lançamentos** e **Consolidado** ocorre exclusivamente via mensageria (RabbitMQ).

Fluxo:

```
Lançamentos
   ↓
DomainEvent
   ↓
Outbox
   ↓
RabbitMQ
   ↓
Consolidado Worker
```

Não existe chamada HTTP entre os serviços.

### Benefícios

- Isolamento de falhas
- Eliminação de dependência síncrona
- Maior disponibilidade do sistema

---

## Outbox Pattern

Foi implementado o Outbox Pattern para garantir consistência entre:

- Persistência do lançamento
- Publicação do evento

### Como funciona

1. O Aggregate dispara um `DomainEvent`.
2. O evento é salvo na tabela `OutboxMessages` na mesma transação.
3. Um Worker publica o evento no RabbitMQ.
4. Após publicação bem-sucedida, o evento é marcado como processado.

Isso evita perda de mensagens caso o broker esteja indisponível.

---

## Retry Exponencial

No Worker do Consolidado foi configurado retry exponencial utilizando MassTransit.

Exemplo de configuração:

```csharp
cfg.UseMessageRetry(r =>
{
    r.Exponential(
        retryLimit: 5,
        minInterval: TimeSpan.FromMilliseconds(100),
        maxInterval: TimeSpan.FromSeconds(5),
        intervalDelta: TimeSpan.FromMilliseconds(200));
});
```

### Benefícios

- Recuperação automática de falhas transitórias
- Redução de perda de mensagens
- Maior robustez do processamento

---

## Idempotência

Para evitar processamento duplicado de eventos, foi criada a tabela:

```
ProcessedEvents
```

Antes de processar um evento:

- Verifica-se se o `EventId` já foi processado.
- Caso exista, o evento é ignorado.

Isso protege contra:

- Reentregas do broker
- Retry duplicado
- Reinício de containers

---

## Banco Isolado por Serviço

Cada serviço possui seu próprio banco de dados:

- PostgreSQL – Lançamentos
- PostgreSQL – Consolidado

Não existem transações distribuídas.

### Benefícios

- Independência de falhas
- Escalabilidade independente
- Desacoplamento de persistência

---

# Cenário de Falha – Consolidado Offline

Se o serviço de Consolidado estiver indisponível:

1. Lançamentos continuam funcionando normalmente.
2. Eventos permanecem na fila do RabbitMQ.
3. O Outbox continua registrando eventos.
4. Quando o Consolidado retorna:
   - Processa mensagens pendentes.
   - Atualiza saldo diário.
   - Mantém consistência eventual.

---

# Modelo de Consistência

A solução adota:

```
Eventual Consistency
```

### Trade-off consciente

- Pode haver pequeno atraso no saldo.
- Ganha-se alta disponibilidade e desacoplamento.

---

# Garantia do Requisito do Desafio

Requisito:

> O serviço de Lançamentos não pode ficar indisponível se o Consolidado cair.

Atendido porque:

- Não há dependência síncrona.
- Comunicação é assíncrona.
- Persistência é independente.
- Eventos não são perdidos.

---

# Evoluções Futuras

Embora a solução já seja resiliente, pode evoluir para nível enterprise:

---

## Dead Letter Queue (DLQ)

Mensagens que falharem após todos os retries podem ser enviadas para uma DLQ.

Benefícios:

- Não bloqueia a fila principal.
- Permite análise posterior.
- Evita loop infinito de falhas.

---

## Circuit Breaker

Pode-se implementar Circuit Breaker utilizando Polly.

Exemplo:

```csharp
services.AddHttpClient("external")
    .AddPolicyHandler(Policy
        .Handle<Exception>()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
```

Protege contra falhas externas prolongadas.

---

## Health Checks

Adicionar:

```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRabbitMQ(rabbitConnection);
```

Permite monitoramento via:

```
/health
```

---

## Monitoramento de Fila

Monitorar:

- Tamanho da fila
- Tempo médio de processamento
- Taxa de erro

Pode ser feito via Prometheus + Grafana.

---

## Replicação Horizontal

O Worker pode ser escalado horizontalmente:

- Múltiplas instâncias
- RabbitMQ distribui mensagens automaticamente
- Sem impacto no domínio

---

# Análise Arquitetural

| Estratégia | Impacto   |
|------------|---------- |
| Comunicação assíncrona | Alta disponibilidade |
| Outbox | Garantia de entrega |
| Retry exponencial | Recuperação automática |
| Idempotência | Consistência segura |
| Banco isolado | Desacoplamento total |

---

# Conclusão

A resiliência da solução foi construída com base em:

- Desacoplamento estrutural
- Tolerância a falhas
- Recuperação automática
- Consistência eventual controlada

A arquitetura atende aos requisitos do desafio e está preparada para evolução futura em ambientes produtivos distribuídos.