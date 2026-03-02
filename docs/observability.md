# Observabilidade – FluxoCaixa

A observabilidade da solução foi projetada para permitir:

- Diagnóstico rápido de falhas
- Rastreabilidade de requisições
- Monitoramento de comportamento
- Preparação para ambientes distribuídos

A estratégia combina logging estruturado, tratamento centralizado de exceções e preparação para métricas e tracing distribuído.

---

# O Que Está Implementado

## Logging Estruturado

A aplicação utiliza o sistema de logging nativo do .NET, permitindo:

- Logs por nível (Information, Warning, Error)
- Logs categorizados por classe
- Logs contextualizados por request

Exemplo:

```csharp
_logger.LogInformation("Processando lançamento {LancamentoId} para usuário {UserId}", lancamentoId, userId);
```

Benefícios:

- Estrutura compatível com ferramentas de análise
- Fácil integração futura com ELK, Loki ou Application Insights
- Logs legíveis e filtráveis

---

## Middleware Global de Exceções

Foi implementado um middleware global para:

- Capturar exceções não tratadas
- Retornar respostas padronizadas
- Evitar vazamento de stacktrace
- Logar erros críticos

Fluxo:

```
Request
   ↓
Pipeline ASP.NET
   ↓
ExceptionHandlingMiddleware
   ↓
Log estruturado + resposta padronizada
```

Benefícios:

- Consistência nas respostas
- Segurança
- Melhor rastreabilidade

---

# Estratégia de Observabilidade (Arquitetural)

A solução está preparada para três pilares:

## Logs
Eventos de aplicação e erros.

## Métricas
Volume de requisições, tempo de processamento, falhas.

## Tracing Distribuído
Rastreamento de requisições entre serviços.

---

# Evolução com OpenTelemetry

OpenTelemetry pode ser integrado facilmente.

## Instalar Pacotes

```bash
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.Prometheus.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
```

---

## Configuração no Program.cs

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter();
    });
```

---

## Expor Endpoint de Métricas

```csharp
app.MapPrometheusScrapingEndpoint();
```

Isso cria endpoint:

```
http://localhost:5000/metrics
```

---

# Integração com Prometheus + Grafana

## Adicionar Prometheus no docker-compose

```yaml
prometheus:
  image: prom/prometheus
  ports:
    - "9090:9090"
  volumes:
    - ./docker/observability/prometheus.yml:/etc/prometheus/prometheus.yml
```

---

## Exemplo prometheus.yml

```yaml
global:
  scrape_interval: 5s

scrape_configs:
  - job_name: 'fluxocaixa-api'
    static_configs:
      - targets: ['host.docker.internal:5000']
```

---

## Adicionar Grafana

```yaml
grafana:
  image: grafana/grafana
  ports:
    - "3000:3000"
```

Acessar:

```
http://localhost:3000
```

Login padrão:

```
admin / admin
```

Adicionar Prometheus como Data Source.

---

# Tracing Distribuído

Para tracing distribuído completo, pode-se utilizar:

- OpenTelemetry + Jaeger
- OpenTelemetry + Tempo (Grafana)
- OpenTelemetry + Zipkin

Exemplo com Jaeger:

```yaml
jaeger:
  image: jaegertracing/all-in-one
  ports:
    - "16686:16686"
    - "4317:4317"
```

Acessar UI:

```
http://localhost:16686
```

---

# Benefícios Arquiteturais

- Observabilidade desacoplada do domínio
- Preparado para ambiente distribuído
- Escalável horizontalmente
- Compatível com padrões cloud-native
- Fácil integração com qualquer provider

---

# Trade-offs

| Decisão | Benefício | Complexidade |
|----------|-----------|-------------|
| Logging estruturado | Diagnóstico rápido | Baixa |
| Middleware global | Respostas padronizadas | Baixa |
| OpenTelemetry | Visibilidade completa | Média |
| Prometheus + Grafana | Monitoramento visual | Média |

---

# Conclusão

A solução já possui:

- Logging estruturado
- Tratamento centralizado de exceções

E está arquiteturalmente preparada para:

- Métricas detalhadas
- Tracing distribuído
- Monitoramento em tempo real
- Integração com ferramentas modernas de observabilidade

Essa abordagem garante maturidade operacional e preparação para ambientes produtivos.