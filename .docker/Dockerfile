FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY scraper/AnnoucementScraper/AnnoucementScraper.sln ./
COPY scraper/AnnoucementScraper/AnnoucementScraper/*.csproj ./AnnoucementScraper/
WORKDIR /app/AnnoucementScraper
RUN dotnet restore

COPY scraper/AnnoucementScraper/AnnPostScraper/*.csproj ./AnnPostScraper/
WORKDIR /app/AnnPostScraper
RUN dotnet restore

# copy and publish apps and libraries
COPY scraper/AnnoucementScraper/AnnoucementScraper/. ./AnnoucementScraper/
WORKDIR /app/AnnoucementScraper
RUN dotnet publish -c Release -o out

# copy and publish app and libraries
COPY scraper/AnnoucementScraper/AnnPostScraper/. ./AnnPostScraper/
WORKDIR /app/AnnPostScraper
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/AnnPostScraper/out ./

ENTRYPOINT ["dotnet", "AnnPostScraper.dll"]