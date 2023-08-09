#!/bin/bash

set -e

# Check if Docker is installed
if ! command -v docker &>/dev/null; then
    echo "Docker is not installed. Please install Docker and try again."
    exit 1
fi

# Define the necessary environment variables for Local
export ASPNETCORE_ENVIRONMENT=Local
export RabbitMqHost=rabbitmq_host
export RabbitMqUsername=rabbitmq_username
export RabbitMqPassword=rabbitmq_password
export PsqlHost=postgresql_host
export PsqlPort=5432
export PsqlUser=db_user
export PsqlPassword=db_password
export PsqlDatabase=db_name
export AzuriteAccount=azurite_account
export AzuriteAccountKey=azurite_account_key

# Clean up existing containers (if any)
echo "Cleaning up existing Docker containers..."
docker-compose down --volumes --remove-orphans

# Check if the container exists
if docker ps -a --format '{{.Names}}' | grep -q "^marketplace$"; then
    # Stop the existing container
    docker stop marketplace

    # Remove the existing container
    docker rm marketplace
else
    echo "Container 'marketplace' does not exist."
fi

# Start RabbitMQ, PostgreSQL, Azure Blob Emulator, and Elasticsearch in Docker containers
echo "Starting Docker containers..."
docker-compose up -d

# Wait for services to start (adjust the sleep duration as needed)
sleep 10

# Apply database migrations
echo "Applying database migrations for persistence lib..."
dotnet ef database update --project src/TechMastery.MarketPlace.Persistence
echo "Applying database migrations for identity lib..."
dotnet ef database update --project src/TechMastery.MarketPlace.Identity

# Restore NuGet packages and build your microservice
echo "Restoring NuGet packages and building microservice..."
dotnet restore src/TechMastery.MarketPlace.Api
dotnet build src/TechMastery.MarketPlace.Api -c Release

# List of test project paths
test_projects=(
  "tests/TechMastery.MarketPlace.Application.UnitTests"
  "tests/TechMastery.MarketPlace.API.IntegrationTests"
  "tests/TechMastery.MarketPlace.Infrastructure.IntegrationTests"
)

# Function to run tests in parallel
run_tests_parallel() {
  for project in "$@"; do
    dotnet test "$project" --no-build --verbosity normal --filter FullyQualifiedName~YourTestNamespace || true
  done
}

# Run tests in parallel batches
num_parallel_batches=${#test_projects[@]}  # Number of parallel batches equal to the number of test projects
batch_size=$(((${#test_projects[@]} + num_parallel_batches - 1) / num_parallel_batches))
for ((i = 0; i < ${#test_projects[@]}; i += batch_size)); do
  batch=("${test_projects[@]:i:batch_size}")
  run_tests_parallel "${batch[@]}" &
done

# Wait for all test batches to finish
wait

# Continue with other commands
echo "Continuing with other commands..."

# Build the Docker image for the microservice
echo "Building Docker image for microservice..."
docker build -t techmastery/marketplace .

# Run your microservice in a Docker container
echo "Running microservice in a Docker container..."
docker run -d -p 5000:80 --name marketplace techmastery/marketplace

# Check the health of the application
HEALTH_CHECK_URL="http://localhost/health"
HEALTH_CHECK_STATUS=$(curl -s -o /dev/null -w "%{http_code}" $HEALTH_CHECK_URL)
if [ "$HEALTH_CHECK_STATUS" -eq 200 ]; then
    echo "Application is healthy."
else
    echo "Application is not healthy. Setup script aborted."
    exit 1
fi

# Continue with other commands
echo "Continuing with other commands..."
# Add your other commands here

# Exit with an appropriate exit code
exit 0
