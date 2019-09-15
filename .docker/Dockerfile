FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY scraper/AnnoucementScraper/AnnPostScraper/*.csproj ./AnnPostScraper/
WORKDIR /app/AnnPostScraper
RUN dotnet restore

# copy and publish app and libraries
WORKDIR /app/
COPY scraper/AnnoucementScraper/AnnPostScraper/. ./AnnPostScraper/
WORKDIR /app/AnnPostScraper
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/runtime:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/AnnPostScraper/out ./

# Potential ENV variables/port
# ENV DEPLOY_METHOD=docker \
#     ROOT_URL=http://localhost:3000 \

# EXPOSE 3000

ENTRYPOINT ["dotnet", "AnnPostScraper.dll"]