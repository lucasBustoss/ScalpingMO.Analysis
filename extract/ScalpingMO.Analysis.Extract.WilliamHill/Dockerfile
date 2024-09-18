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

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["extract/ScalpingMO.Analysis.Extract.WilliamHill/ScalpingMO.Analysis.Extract.WilliamHill.csproj", "extract/ScalpingMO.Analysis.Extract.WilliamHill/"]
RUN dotnet restore "./extract/ScalpingMO.Analysis.Extract.WilliamHill/ScalpingMO.Analysis.Extract.WilliamHill.csproj"
COPY . .
WORKDIR "/src/extract/ScalpingMO.Analysis.Extract.WilliamHill"
RUN dotnet build "./ScalpingMO.Analysis.Extract.WilliamHill.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ScalpingMO.Analysis.Extract.WilliamHill.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set up ChromeDriver environment variables
ENV PATH="/usr/lib/chromium:/usr/local/bin:${PATH}"
ENV CHROME_BIN="/usr/lib/chromium/chromium"

ENTRYPOINT ["dotnet", "ScalpingMO.Analysis.Extract.WilliamHill.dll"]
