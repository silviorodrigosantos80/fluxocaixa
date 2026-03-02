# 📈 Escalabilidade – FluxoCaixa

A escalabilidade da solução foi projetada para atender ao requisito:

> Suportar 50 requisições por segundo com até 5% de perda.

A arquitetura foi desenhada para permitir crescimento horizontal e processamento paralelo controlado.

---

# Estratégia Arquitetural Adotada

A escalabilidade foi construída em três níveis:

1. Escalabilidade da API de Lançamentos
2. Escalabilidade do processamento assíncrono
3. Escalabilidade horizontal do Worker

---

# API de Lançamentos

A API de Lançamentos:

- Apenas persiste o lançamento
- Dispara DomainEvent
- Salva evento na Outbox
- Não depende do Consolidado

Como não há chamada síncrona externa, o throughput da API depende apenas de:

- Capacidade do PostgreSQL
- Pool de conexões
- Recursos da aplicação

Isso elimina gargalo externo imediato.

---

# Processamento Assíncrono via RabbitMQ

A integração entre serviços ocorre via RabbitMQ.

Isso permite:

- Buffer de requisições
- Processamento desacoplado
- Absorção de picos de carga

Se 50 requisições chegarem simultaneamente:

- Todas são persistidas
- Eventos entram na fila
- Processamento ocorre conforme capacidade do Worker

---

# Configuração de Concorrência

No Worker do Consolidado foi configurado:

```csharp
cfg.PrefetchCount = 100;
cfg.ConcurrentMessageLimit = 20;
```

## PrefetchCount

Define quantas mensagens o broker entrega antecipadamente ao consumer.

Impacto:

- Reduz latência
- Melhora throughput
- Evita overhead excessivo de round-trip

## ConcurrentMessageLimit

Define quantas mensagens podem ser processadas em paralelo.

Impacto:

- Controle de paralelismo
- Evita sobrecarga do banco
- Permite tuning fino de performance

---

# Retry Exponencial

Configurado via MassTransit:

```csharp
r.Exponential(
    retryLimit: 5,
    minInterval: 100ms,
    maxInterval: 5s,
    intervalDelta: 200ms);
```

Isso garante:

- Recuperação automática de falhas transitórias
- Redução de perda de mensagens
- Atendimento ao limite de 5% de falha permitido

---

# Banco Isolado por Serviço

Cada serviço possui banco próprio.

Benefícios:

- Evita lock distribuído
- Permite tuning individual
- Permite escalabilidade independente
- Elimina contenção entre serviços

---

# Comportamento sob Carga

Em cenário de 50 req/s:

1. API persiste rapidamente.
2. Eventos entram na fila.
3. Worker processa em paralelo.
4. Se houver pico:
   - Fila absorve.
   - Processamento ocorre gradualmente.
   - Sem indisponibilidade da API.

---

# Gargalos Possíveis

Mesmo com arquitetura escalável, alguns pontos devem ser observados:

- Limite de conexões do PostgreSQL
- I/O do disco
- CPU do Worker
- Saturação do RabbitMQ
- Tamanho do PrefetchCount mal configurado

Esses pontos podem ser ajustados sem alterar o domínio.

---

# Escala Horizontal

A arquitetura permite adicionar múltiplas instâncias do Worker.

Exemplo:

```
Worker 1
Worker 2
Worker 3
```

O RabbitMQ distribui mensagens automaticamente entre instâncias.

Benefícios:

- Escala linear
- Sem alteração no código
- Sem impacto no domínio

---

# Escalabilidade da Infraestrutura

Pode-se evoluir para:

- Kubernetes
- Auto-scaling baseado em CPU
- Auto-scaling baseado em tamanho da fila
- Replicação de banco
- Cluster RabbitMQ

---

# Evoluções Futuras

## Particionamento de Fila

Separar filas por:

- Tipo de evento
- Região
- Cliente

Reduz contenção e melhora paralelismo.

---

## Cache de Consulta

Adicionar Redis para:

- Cache de saldo diário
- Reduzir carga de leitura no banco

---

## Sharding de Banco

Caso volume cresça significativamente:

- Separar usuários por shard
- Balancear carga horizontalmente

---

## Métricas de Performance

Monitorar:

- Tempo médio de processamento
- Tamanho da fila
- Taxa de erro
- Throughput por segundo

Via Prometheus + Grafana.

---

# Trade-offs Arquiteturais

| Decisão | Benefício | Trade-off |
|----------|------------|-----------|
| Comunicação assíncrona | Absorve picos | Consistência eventual |
| Prefetch configurado | Alta performance | Precisa tuning |
| Worker concorrente | Paralelismo | Pode gerar contenção no banco |
| Banco isolado | Independência | Sem transação global |

---

# Conclusão

A solução foi desenhada para:

- Absorver picos de requisições
- Processar mensagens em paralelo
- Escalar horizontalmente
- Evitar gargalos síncronos
- Atender ao requisito de 50 req/s

A arquitetura permite crescimento progressivo sem necessidade de refatoração estrutural.