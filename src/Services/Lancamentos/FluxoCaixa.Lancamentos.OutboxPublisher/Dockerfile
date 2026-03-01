# -----------------------------
# BUILD STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solução
COPY *.sln ./

# Copiar projetos
COPY src/BuildingBlocks ./src/BuildingBlocks
COPY src/Services ./src/Services

# Restaurar apenas o OutboxPublisher
RUN dotnet restore src/Services/Lancamentos/FluxoCaixa.Lancamentos.OutboxPublisher/FluxoCaixa.Lancamentos.OutboxPublisher.csproj

# Publicar
RUN dotnet publish src/Services/Lancamentos/FluxoCaixa.Lancamentos.OutboxPublisher/FluxoCaixa.Lancamentos.OutboxPublisher.csproj \
    -c Release -o /app/publish

# -----------------------------
# RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FluxoCaixa.Lancamentos.OutboxPublisher.dll"]