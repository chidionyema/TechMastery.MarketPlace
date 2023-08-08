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

# Start RabbitMQ, PostgreSQL, and Azure Blob Emulator in Docker containers
docker-compose up -d

# Wait for services to start (adjust the sleep duration as needed)
sleep 10

# Restore NuGet packages and build your microservice
dotnet restore src/TechMastery.MarketPlace.Api
dotnet build src/TechMastery.MarketPlace.Api

# Run your microservice
dotnet run --project src/TechMastery.MarketPlace.Api
