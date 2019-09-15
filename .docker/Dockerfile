FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy everything and build the project
COPY . ./
RUN dotnet restore scraper/AnnoucementScraper/AnnPostScraper/*.csproj
RUN dotnet publish scraper/AnnoucementScraper/AnnPostScraper/*.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/scraper/AnnoucementScraper/AnnPostScraper/out ./

ENTRYPOINT ["dotnet", "AnnPostScraper.dll"]