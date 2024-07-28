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
ENV ASPNETCORE_ENVIRONMENT Development
ENV ASPNETCORE_URLS=http://+:5000
# ENV ASPNETCORE_URLS="http://+:5000;https://+:5001"
EXPOSE 5000
EXPOSE 5001
ENV MessagingSettings__ConnectionString="amqp://guest:guest@host.docker.internal:5672/"

# Build: docker build -t event-ticketing-system-api:dev -f Api.Dockerfile .
# Run: docker run --rm -d -p 5000:5000 -p 5001:5001 event-ticketing-system-api:dev