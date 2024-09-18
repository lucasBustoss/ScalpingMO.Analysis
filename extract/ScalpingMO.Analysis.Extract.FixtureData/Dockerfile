# Base image with .NET runtime
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
    --no-install-recommends && rm -rf /var/lib/apt/lists/*

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copiar o appsettings.Docker.json manualmente, se necessário
COPY ["extract/ScalpingMO.Analysis.Extract.FixtureData/appsettings.Docker.json", "extract/ScalpingMO.Analysis.Extract.FixtureData/"]
COPY ["extract/ScalpingMO.Analysis.Extract.FixtureData/ScalpingMO.Analysis.Extract.FixtureData.csproj", "extract/ScalpingMO.Analysis.Extract.FixtureData/"]
RUN dotnet restore "./extract/ScalpingMO.Analysis.Extract.FixtureData/ScalpingMO.Analysis.Extract.FixtureData.csproj"
COPY . .
WORKDIR "/src/extract/ScalpingMO.Analysis.Extract.FixtureData"
RUN dotnet build "./ScalpingMO.Analysis.Extract.FixtureData.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScalpingMO.Analysis.Extract.FixtureData.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScalpingMO.Analysis.Extract.FixtureData.dll"]