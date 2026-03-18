# Dockerfile for CleanupFunction
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY CleanupJob.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# This is important for dotnet-isolated function
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV DOTNET_HOST_PATH=/azure-functions-host/Microsoft.Azure.Functions.Host.dll

ENTRYPOINT ["dotnet", "CleanupJob.dll"]