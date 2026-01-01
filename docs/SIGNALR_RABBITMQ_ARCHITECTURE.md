# SignalR + RabbitMQ Real-Time Architecture

## Fluxo Completo de Submissão de Exercício

```
┌─────────┐                ┌──────────────┐                ┌────────────┐                ┌──────────────┐
│ Cliente │                │ CompetitionHub│                │  RabbitMQ  │                │ Worker       │
│ (React) │                │  (SignalR)    │                │  (MassT.)  │                │ (Consumer)   │
└────┬────┘                └──────┬───────┘                └─────┬──────┘                └──────┬───────┘
     │                             │                              │                              │
     │ 1. Connect()                │                              │                              │
     ├────────────────────────────►│                              │                              │
     │                             │                              │                              │
     │ 2. OnConnectionResponse     │                              │                              │
     │◄────────────────────────────┤                              │                              │
     │   (Competition details)     │                              │                              │
     │                             │                              │                              │
     │ 3. SendExerciseAttempt()    │                              │                              │
     ├────────────────────────────►│ 4. Validate                  │                              │
     │                             │   - Group blocked?           │                              │
     │                             │   - Already accepted?        │                              │
     │                             │   - Exercise in comp?        │                              │
     │                             │                              │                              │
     │                             │ 5. Publish ISubmitExerciseCmd│                              │
     │                             ├─────────────────────────────►│                              │
     │                             │                              │                              │
     │ 6. ReceiveExerciseAttemptQueued                            │                              │
     │◄────────────────────────────┤                              │                              │
     │   (correlationId, message)  │                              │                              │
     │                             │                              │ 7. Consume message           │
     │                             │                              ├─────────────────────────────►│
     │                             │                              │                              │ 8. Process
     │                             │                              │                              │    - Call Judge
     │                             │                              │                              │    - Create Attempt
     │                             │                              │                              │    - Update Ranking
     │                             │                              │                              │    - Create Log
     │                             │                              │                              │
     │                             │                              │ 9. Publish ISubmitExerciseRes│
     │                             │                              │◄─────────────────────────────┤
     │                             │                              │                              │
     │                             │ 10. Consume result           │                              │
     │                             │◄─────────────────────────────┤                              │
     │                             │  (via SubmitExerciseResult   │                              │
     │                             │   Consumer)                  │                              │
     │                             │                              │                              │
     │ 11. ReceiveExerciseAttemptResponse                         │                              │
     │◄────────────────────────────┤                              │                              │
     │   (attempt, ranking)        │                              │                              │
     │                             │                              │                              │
     │ 12. ReceiveRankingUpdate (ALL CLIENTS)                     │                              │
     │◄────────────────────────────┤                              │                              │
     │                             │                              │                              │
```

## Componentes

### 1. CompetitionHub (Falcon.Api/Hubs/CompetitionHub.cs)
**Métodos SignalR:**
- `OnConnectedAsync()` - Adiciona usuário aos grupos (Admins, Teachers, Students, Group:{id})
- `SendExerciseAttempt(competitionId, exerciseId, code, languageType)` - Valida e publica no RabbitMQ
- `GetCurrentCompetition()` - Retorna competição atual
- `SendRankingUpdate()` - Broadcasting de ranking
- `Ping()` - Healthcheck

**Eventos enviados ao cliente:**
- `OnConnectionResponse` - Dados iniciais da competição
- `ReceiveExerciseAttemptQueued` - Confirmação de enfileiramento
- `ReceiveExerciseAttemptResponse` - Resultado da submissão
- `ReceiveExerciseAttemptError` - Erro na submissão
- `ReceiveRankingUpdate` - Atualização de ranking (broadcast)
- `Pong` - Resposta ao ping

### 2. Mensagens RabbitMQ (Falcon.Core/Messages/)

**ISubmitExerciseCommand:**
```csharp
{
    Guid CorrelationId,
    string ConnectionId,
    Guid GroupId,
    Guid ExerciseId,
    Guid CompetitionId,
    string Code,
    LanguageType Language,
    DateTime SubmittedAt
}
```

**ISubmitExerciseResult:**
```csharp
{
    Guid CorrelationId,
    string ConnectionId,
    bool Success,
    string? ErrorMessage,
    Guid? AttemptId,
    bool Accepted,
    JudgeSubmissionResponse JudgeResponse,
    TimeSpan ExecutionTime,
    int RankOrder,
    double Points,
    double Penalty,
    int SolvedExercises
}
```

### 3. SubmitExerciseConsumer (Falcon.Worker/Consumers/)
Processa submissões assincronamente:
1. Carrega Exercise, Group, Competition
2. Chama IJudgeService.SubmitCodeAsync()
3. Cria GroupExerciseAttempt
4. Atualiza CompetitionRanking se aceito
5. Recalcula RankOrder
6. Cria Log
7. Publica ISubmitExerciseResult

### 4. SubmitExerciseResultConsumer (Falcon.Api/Consumers/)
Envia resultado de volta ao cliente:
1. Consome ISubmitExerciseResult do RabbitMQ
2. Usa IHubContext<CompetitionHub> para enviar ao cliente específico via ConnectionId
3. Envia `ReceiveExerciseAttemptResponse` ou `ReceiveExerciseAttemptError`
4. Faz broadcast de `ReceiveRankingUpdate` para todos os clientes

## Configuração

### Falcon.Api (Program.cs)
```csharp
// SignalR
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// MassTransit with Consumer
builder.Services.AddApiMassTransit(x =>
{
    x.AddConsumer<SubmitExerciseResultConsumer>();
});

// Endpoint
app.MapHub<CompetitionHub>("/hubs/competition");
```

### Falcon.Worker (Program.cs)
```csharp
services.AddInfrastructure(context.Configuration);

services.AddMassTransit(x =>
{
    x.AddConsumer<SubmitExerciseConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ConfigureEndpoints(ctx);
    });
});
```

## Execução

### 1. Iniciar RabbitMQ
```bash
docker-compose up -d
```

### 2. Iniciar Worker
```bash
dotnet run --project src/Falcon.Worker
```

### 3. Iniciar API
```bash
dotnet run --project src/Falcon.Api
```

### 4. Cliente conecta
```javascript
const connection = new HubConnectionBuilder()
    .withUrl("https://localhost:5001/hubs/competition", {
        accessTokenFactory: () => localStorage.getItem("token")
    })
    .withAutomaticReconnect()
    .build();

await connection.start();

// Receber dados iniciais
connection.on("OnConnectionResponse", (competition) => {
    console.log("Competition:", competition);
});

// Submeter exercício
await connection.invoke("SendExerciseAttempt", competitionId, exerciseId, code, 1);

// Receber confirmação
connection.on("ReceiveExerciseAttemptQueued", (data) => {
    console.log("Queued:", data.correlationId);
});

// Receber resultado
connection.on("ReceiveExerciseAttemptResponse", (data) => {
    console.log("Accepted:", data.attempt.accepted);
    console.log("Ranking:", data.ranking);
});

// Receber erros
connection.on("ReceiveExerciseAttemptError", (data) => {
    console.error("Error:", data.message);
});

// Receber atualizações de ranking (broadcast)
connection.on("ReceiveRankingUpdate", (ranking) => {
    console.log("Ranking updated:", ranking);
});
```

## Vantagens da Arquitetura

1. **Desacoplamento**: API não bloqueia aguardando avaliação do Judge
2. **Escalabilidade**: Múltiplas instâncias do Worker podem processar submissões
3. **Resilência**: Se Worker cair, mensagens ficam na fila
4. **Real-time**: Clientes recebem atualizações instantâneas via SignalR
5. **Broadcasting**: Atualização de ranking para todos os clientes simultaneamente
6. **Rastreabilidade**: CorrelationId permite rastrear cada submissão

## Próximos Passos

- [ ] Implementar Questions/Answers via SignalR
- [ ] Adicionar retry policies no MassTransit
- [ ] Implementar dead-letter queue para falhas
- [ ] Adicionar métricas e observabilidade
