# Base image with .NET runtime and dependencies for Chrome/Chromium
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Install dependencies for Chrome/Chromium and ChromeDriver
RUN apt-get update && apt-get install -y \
    chromium \
    chromium-driver \
    libnss3-dev \
    libxss1 \
    libappindicator1 \
    fonts-liberation \
    libappindicator3-1 \
    xdg-utils \
    powershell \
    --no-install-recommends && rm -rf /var/lib/apt/lists/*

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar o appsettings.Docker.json manualmente, se necessário
COPY ["extract/ScalpingMO.Analysis.Extract.FixtureData/appsettings.Docker.json", "extract/ScalpingMO.Analysis.Extract.FixtureData/"]
COPY ["extract/ScalpingMO.Analysis.Extract.FixtureData/ScalpingMO.Analysis.Extract.FixtureData.csproj", "extract/ScalpingMO.Analysis.Extract.FixtureData/"]

# Restaurar dependências
RUN dotnet restore "./extract/ScalpingMO.Analysis.Extract.FixtureData/ScalpingMO.Analysis.Extract.FixtureData.csproj"

# Copiar todo o código para o container
COPY . .

# Build do projeto
WORKDIR "/src/extract/ScalpingMO.Analysis.Extract.FixtureData"
RUN dotnet build "./ScalpingMO.Analysis.Extract.FixtureData.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta etapa é usada para publicar o projeto
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScalpingMO.Analysis.Extract.FixtureData.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final para produção
FROM base AS final
WORKDIR /app

# Copiar a publicação do projeto da etapa anterior
COPY --from=publish /app/publish .

# Instalar navegadores Playwright usando PowerShell
RUN pwsh -Command "pwsh -ExecutionPolicy Bypass -File bin/Debug/net8.0/playwright.ps1 install"

# Entrypoint da aplicação .NET
ENTRYPOINT ["dotnet", "ScalpingMO.Analysis.Extract.FixtureData.dll"]
