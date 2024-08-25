FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /source
# Copy the source files
COPY ["EventTicketingSystem.API/", "EventTicketingSystem.API/"]
COPY ["EventTicketingSystem.Contract/", "EventTicketingSystem.Contract/"]
COPY ["EventTicketingSystem.DataAccess/", "EventTicketingSystem.DataAccess/"]

# Restore, Build and publish a release 
RUN dotnet restore "EventTicketingSystem.API/EventTicketingSystem.API.csproj"
RUN dotnet build "EventTicketingSystem.API/EventTicketingSystem.API.csproj" -c Release -o /app/build
RUN dotnet publish "EventTicketingSystem.API/EventTicketingSystem.API.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "EventTicketingSystem.API.dll"]
ENV ASPNETCORE_ENVIRONMENT test
ENV ASPNETCORE_URLS=http://+:5000
# ENV ASPNETCORE_URLS="http://+:5000;https://+:5001"
EXPOSE 5000
EXPOSE 5001

# Build: docker build -t event-ticketing-system-api:test -f Api.Azure.Dockerfile .
# Tag: docker tag event-ticketing-system-api:test eventticketingsystemcr.azurecr.io/event-ticketing-system-api:latest
# Auth: docker login eventticketingsystemcr.azurecr.io
# Deploy: docker push eventticketingsystemcr.azurecr.io/event-ticketing-system-api:latest
# Check: docker pull eventticketingsystemcr.azurecr.io/event-ticketing-system-api:latest
# Set env vars (Database, Messaging) in Azure Web App (event-ticketing-system-api)
# Env vars: DatabaseSettings__ConnectionString, MessagingSettings__ConnectionString