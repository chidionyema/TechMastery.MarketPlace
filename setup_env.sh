#!/bin/bash

set -e

# Check if Docker is installed
if ! command -v docker &>/dev/null; then
    echo "Docker is not installed. Please install Docker and try again."
    exit 1
fi

# Define the necessary environment variables
export ASPNETCORE_ENVIRONMENT=Development
export RabbitMqHost=localhost
export RabbitMqUsername=guest
export RabbitMqPassword=guest
export PsqlHost=localhost
export PsqlPort=5432
export PsqlUser=my_db_user
export PsqlPassword=my_db_password
export PsqlDatabase=my_db_name
export AzuriteAccount=myaccount
export AzuriteAccountKey=myaccountkey

# Start RabbitMQ, PostgreSQL, Azure Blob Emulator, and Elasticsearch in Docker containers
docker-compose up -d

# Wait for services to start (adjust the sleep duration as needed)
sleep 8

# Apply database migrations
dotnet ef database update --project src/TechMastery.MarketPlace.Persistence

# Restore NuGet packages and build your microservice
dotnet restore src/TechMastery.MarketPlace.Api
dotnet build src/TechMastery.MarketPlace.Api -c Release

# Run tests
#  dotnet test tests/TechMastery.MarketPlace.Application.UnitTests
#  dotnet test tests/TechMastery.MarketPlace.API.IntegrationTests
#  dotnet test tests/TechMastery.MarketPlace.Infrastructure.IntegrationTests


# Build the Docker image for the microservice
docker build -t techmastery/marketplace .

# Run your microservice in a Docker container
docker run -d -p 5000:80 --name marketplace techmastery/marketplace

sleep 3

# Check the health of the application
HEALTH_CHECK_URL="http://localhost/health"
HEALTH_CHECK_STATUS=$(curl -s -o /dev/null -w "%{http_code}" $HEALTH_CHECK_URL)

if [ "$HEALTH_CHECK_STATUS" -eq 200 ]; then
  echo "Application is healthy. Proceed with the setup."
else
  echo "Application is not healthy. Setup script aborted."
  exit 1
fi