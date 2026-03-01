# -----------------------------
# BUILD STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar apenas arquivos de solução primeiro (cache eficiente)
COPY *.sln ./

# Copiar projetos individualmente (melhor cache)
COPY src/BuildingBlocks ./src/BuildingBlocks
COPY src/Services ./src/Services

# Restaurar
RUN dotnet restore src/Services/Consolidado/FluxoCaixa.Consolidado.Api/FluxoCaixa.Consolidado.Api.csproj

# Publicar
RUN dotnet publish src/Services/Consolidado/FluxoCaixa.Consolidado.Api/FluxoCaixa.Consolidado.Api.csproj \
    -c Release -o /app/publish

# -----------------------------
# RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5002

ENTRYPOINT ["dotnet", "FluxoCaixa.Consolidado.Api.dll"]