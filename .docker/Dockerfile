FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY scraper/AnnoucementScraper/AnnoucementScraper.sln ./
COPY scraper/AnnoucementScraper/AnnPostScraper/*.csproj ./AnnPostScraper/
COPY scraper/AnnoucementScraper/AnnoucementScraper/*.csproj ./AnnoucementScraper/

RUN dotnet restore

# copy and publish apps and libraries
COPY scraper/. .

WORKDIR /app/AnnoucementScraper
RUN dotnet build -c Release -o /app

# copy and publish app and libraries
WORKDIR /app/AnnPostScraper
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
WORKDIR /app
COPY --from=publish /app .

# Potential ENV variables/port
# ENV DEPLOY_METHOD=docker \
#     ROOT_URL=http://localhost:3000 \

# EXPOSE 3000

ENTRYPOINT ["dotnet", "AnnPostScraper.dll"]