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
RUN dotnet restore src/Services/Lancamentos/FluxoCaixa.Lancamentos.Api/FluxoCaixa.Lancamentos.Api.csproj

# Publicar
RUN dotnet publish src/Services/Lancamentos/FluxoCaixa.Lancamentos.Api/FluxoCaixa.Lancamentos.Api.csproj \
    -c Debug -o /app/publish

# -----------------------------
# RUNTIME STAGE
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5001

ENTRYPOINT ["dotnet", "FluxoCaixa.Lancamentos.Api.dll"]