#!/bin/bash
set -eo pipefail

# Define configurations
CURRENT_DIR=$(pwd)
CERT_DIR="$CURRENT_DIR/var/app/certs"
POSTGRES_CONTAINER_NAME="myapp-postgres"
USER_SERVICE_CONTAINER_NAME="usermanagement-service"
USER_SERVICE_IMAGE_NAME="usermanagement-service-image"
NETWORK_NAME="mynetwork"
SECURE_SERVICE_IMAGE_NAME="secure-service-image"
SECURE_SERVICE_CONTAINER_NAME="secure-service"
SECURE_PROJECT_PATH="./src/TechMastery.SecureService"
DB_PORT="5432"
DB_NAME="myappdb"
DB_USER="postgres"
DB_PASSWORD="mysecretpassword"
PROJECT_PATH="./src/TechMastery.UsermanagementService"

# RabbitMQ Configuration
RABBITMQ_CONTAINER_NAME="rabbitmq"
RABBITMQ_DEFAULT_USER="guest"
RABBITMQ_DEFAULT_PASS="guest"

# Azure Blob Storage Emulator Configuration
AZURE_BLOB_EMULATOR_CONTAINER_NAME="azure-blob-emulator"
AZURITE_ACCOUNT="myaccount"
AZURITE_ACCOUNT_KEY="myaccountkey"

# Create or check network
create_or_check_network() {
    echo "Checking Docker network: $NETWORK_NAME"
    if ! docker network inspect $NETWORK_NAME &>/dev/null; then
        echo "Creating Docker network: $NETWORK_NAME"
        docker network create $NETWORK_NAME
    else
        echo "Network $NETWORK_NAME already exists."
    fi
}

# Start PostgreSQL container
start_postgres_container() {
    echo "Starting PostgreSQL container: $POSTGRES_CONTAINER_NAME"
    if docker ps -a | grep -q $POSTGRES_CONTAINER_NAME; then
        docker start $POSTGRES_CONTAINER_NAME
    else
        docker run --name $POSTGRES_CONTAINER_NAME -e POSTGRES_PASSWORD=$DB_PASSWORD \
            --network $NETWORK_NAME -p $DB_PORT:5432 -d postgres
    fi
}

# Wait for PostgreSQL to be ready
wait_for_postgres() {
    echo "Waiting for PostgreSQL to be ready..."
    until docker exec $POSTGRES_CONTAINER_NAME pg_isready -U $DB_USER; do
        sleep 2
    done
    echo "PostgreSQL is ready."
}

# Create database if not the default one
create_database() {
    if [ "$DB_NAME" != "postgres" ]; then
        echo "Creating database: $DB_NAME"
        docker exec $POSTGRES_CONTAINER_NAME psql -U $DB_USER -c "CREATE DATABASE $DB_NAME;"
    fi
}

# Apply EF migrations
apply_migrations() {
    echo "Applying EF migrations..."
    pushd $PROJECT_PATH > /dev/null || exit
    dotnet ef database update
    popd > /dev/null || exit
    echo "Migrations applied."
}

# Install mkcert and generate certificates
setup_certificates() {
    echo "Setting up certificates..."
    if ! command -v mkcert &> /dev/null; then
        echo "Installing mkcert..."
        brew install mkcert # For macOS
        # Add installation commands for other operating systems if needed
    fi
    mkcert -install
    mkcert -cert-file "$CERT_DIR/localhost.crt" -key-file "$CERT_DIR/localhost.key" localhost 127.0.0.1 ::1 usermanagement-service.local secure-service.local
    MKCERT_ROOT="$(mkcert -CAROOT)"
    cp "$MKCERT_ROOT/rootCA.pem" "$CERT_DIR/"
    echo "Certificates setup complete."
}

# Function to build and run Docker containers for user management and secure service
build_and_run_containers() {
    echo "Building and running Docker containers..."

    # Define the common parent directory as the build context
    BUILD_CONTEXT="$PROJECT_PATH/.."  # Assuming $PROJECT_PATH is the path to the User Service project

    # User Management Service
    echo "Building User Management Service container..."
    docker build -t $USER_SERVICE_IMAGE_NAME -f "$PROJECT_PATH/Dockerfile" "$BUILD_CONTEXT"
    docker run --name $USER_SERVICE_CONTAINER_NAME -d --network $NETWORK_NAME \
      -p 5000:5000 -p 5001:5001 \
      -e DB_HOST=$POSTGRES_CONTAINER_NAME \
      -e DB_PORT=$DB_PORT \
      -e DB_NAME=$DB_NAME \
      -e DB_USER=$DB_USER \
      -e ASPNETCORE_ENVIRONMENT=Development \
      -e DB_PASSWORD=$DB_PASSWORD \
      -v "$CERT_DIR:/var/app/certs" \
      $USER_SERVICE_IMAGE_NAME

    # Secure Service
    docker build -t $SECURE_SERVICE_IMAGE_NAME -f "$SECURE_PROJECT_PATH/Dockerfile" "$SECURE_PROJECT_PATH"
    docker run --name $SECURE_SERVICE_CONTAINER_NAME -d --network $NETWORK_NAME \
      -p 5002:5002 -p 5003:5003 \
      -e ASPNETCORE_ENVIRONMENT=Development \
      -v "$CERT_DIR:/var/app/certs" \
      $SECURE_SERVICE_IMAGE_NAME

    echo "Containers are set up and running."
}

# Import root CA into containers
import_certificates_into_containers() {
    echo "Importing certificates into containers..."
    docker exec $USER_SERVICE_CONTAINER_NAME /bin/sh -c 'cp /var/app/certs/rootCA.pem /usr/local/share/ca-certificates/ && update-ca-certificates'
    docker exec $SECURE_SERVICE_CONTAINER_NAME /bin/sh -c 'cp /var/app/certs/rootCA.pem /usr/local/share/ca-certificates/ && update-ca-certificates'
    echo "Certificate import complete."
}

# Perform health check
perform_health_check() {
    echo "Performing health check..."
    sleep 8 # Wait for containers to be fully ready
    health_response=$(curl -k -s "https://localhost:5001/health")
    echo "Health check response: $health_response"
    health_status=$(echo $health_response | jq -r '.status')
    if [ "$health_status" != "Healthy" ]; then
        echo "Application is not healthy. Status: $health_status"
    else
        echo "Application is healthy."
    fi
}

# Register a new user, login, and access secure data
register_login_and_access() {
    # Add your function logic here if needed
    echo "Placeholder for register, login, and access secure data logic."
}

# Start RabbitMQ, PostgreSQL, and Azure Blob Storage Emulator containers
start_rabbitmq_container() {
    echo "Starting RabbitMQ container..."
    docker run -d --hostname $RABBITMQ_CONTAINER_NAME --name $RABBITMQ_CONTAINER_NAME \
        -e RABBITMQ_DEFAULT_USER=$RABBITMQ_DEFAULT_USER \
        -e RABBITMQ_DEFAULT_PASS=$RABBITMQ_DEFAULT_PASS \
        --network $NETWORK_NAME \
        -p 5672:5672 -p 15672:15672 \
        rabbitmq:management
}

start_azure_blob_emulator_container() {
    echo "Starting Azure Blob Storage Emulator container..."
    docker run -d --name $AZURE_BLOB_EMULATOR_CONTAINER_NAME \
        -e AZURITE_ACCOUNT=$AZURITE_ACCOUNT \
        -e AZURITE_ACCOUNT_KEY=$AZURITE_ACCOUNT_KEY \
        --network $NETWORK_NAME \
        -p 10000:10000 -p 10001:10001 -p 10002:10002 \
        mcr.microsoft.com/azure-storage/azurite
}

# Main script execution
echo "Starting local development environment setup..."
create_or_check_network
start_postgres_container
wait_for_postgres
create_database
apply_migrations
setup_certificates
start_rabbitmq_container
start_azure_blob_emulator_container
build_and_run_containers
import_certificates_into_containers
perform_health_check
register_login_and_access
echo "Local development environment setup completed successfully."
