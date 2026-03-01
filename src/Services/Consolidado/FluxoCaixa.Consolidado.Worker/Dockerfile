# -----------------------------
# BUILD STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solução
COPY *.sln ./

# Copiar projetos (melhor cache)
COPY src/BuildingBlocks ./src/BuildingBlocks
COPY src/Services ./src/Services

# Restaurar apenas o Worker
RUN dotnet restore src/Services/Consolidado/FluxoCaixa.Consolidado.Worker/FluxoCaixa.Consolidado.Worker.csproj

# Publicar
RUN dotnet publish src/Services/Consolidado/FluxoCaixa.Consolidado.Worker/FluxoCaixa.Consolidado.Worker.csproj \
    -c Release -o /app/publish

# -----------------------------
# RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FluxoCaixa.Consolidado.Worker.dll"]