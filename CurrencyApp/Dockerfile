# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
RUN apk add --no-cache tzdata
WORKDIR /src
COPY ["CurrencyApp.csproj", "./"]
RUN dotnet restore "./CurrencyApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "CurrencyApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "CurrencyApp.csproj" -c Release -o /app/publish

# Use the base image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Install tzdata in the final stage to include time zone data
RUN apk add --no-cache tzdata

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CurrencyApp.dll"]

# docker run -d -p 8000:8080 --name currencyapp currencyapp:latest
