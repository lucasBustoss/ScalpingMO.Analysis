# Base image with .NET runtime and dependencies for Chrome/Chromium
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Install dependencies for Chrome/Chromium
RUN apt-get update && \
    apt-get install -y \
        wget \
        apt-transport-https \
        software-properties-common \
        chromium \
        chromium-driver \
        libnss3-dev \
        libxss1 \
        libappindicator1 \
        fonts-liberation \
        libappindicator3-1 \
        xdg-utils \
        --no-install-recommends && \
    rm -rf /var/lib/apt/lists/*

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

# Publicar o projeto
RUN dotnet publish "./ScalpingMO.Analysis.Extract.FixtureData.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Instalar o Playwright diretamente no Docker (na fase de build)
RUN dotnet tool install --global Microsoft.Playwright.CLI && \
    export PATH="$PATH:/root/.dotnet/tools" && \
    playwright install --with-deps chromium

# Etapa final para produção
FROM base AS final
WORKDIR /app

# Copiar a publicação do projeto da etapa anterior
COPY --from=build /app/publish .
COPY --from=build /root/.cache/ms-playwright /root/.cache/ms-playwright

# Definir o PATH para o Playwright CLI
ENV PATH="$PATH:/root/.dotnet/tools"

# Entrypoint da aplicação .NET
ENTRYPOINT ["dotnet", "ScalpingMO.Analysis.Extract.FixtureData.dll"]
