FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY scraper/cryptoAnalysisScraper/cryptoAnalysisScraper/*.csproj ./cryptoAnalysisScraper/
WORKDIR /app/cryptoAnalysisScraper
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY scraper/cryptoAnalysisScraper/cryptoAnalysisScraper/. ./cryptoAnalysisScraper/
WORKDIR /app/cryptoAnalysisScraper
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/cryptoAnalysisScraper/out ./

# Potential ENV variables/port
# ENV DEPLOY_METHOD=docker \
#     ROOT_URL=http://localhost:3000 \

# EXPOSE 3000

ENTRYPOINT ["dotnet", "cryptoAnalysisScraper.dll"]