#!/bin/bash

# Configuration
BASE_URL="https://usermanagement-service.local:5001" # Change this to your application's URL
REGISTER_ENDPOINT="/Authentication/register" # Ensure leading slash
LOGIN_ENDPOINT="/Authentication/login" # Ensure leading slash

# User details for registration
EMAIL="user@example.com"
PASSWORD="YourStrongPassword@123"

# Function to wait for PostgreSQL to be ready (assuming it's already implemented)
wait_for_postgres

# Register user
echo "Registering user..."
register_response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X POST "$BASE_URL$REGISTER_ENDPOINT" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$EMAIL\", \"password\":\"$PASSWORD\"}")

echo "Registration response: $register_response"

# Check HTTP status code
http_status=$(echo "$register_response" | grep "HTTP_STATUS" | awk -F: '{print $2}')
if [ "$http_status" != "200" ]; then
    echo "Registration failed with status $http_status."
    exit 1
else
    echo "User registered successfully."
fi

# Login user
echo "Logging in..."
login_response=$(curl -s -w "\nHTTP_STATUS:%{http_code}" -X POST "$BASE_URL$LOGIN_ENDPOINT" \
    -H "Content-Type: application/json" \
    -d "{\"email\":\"$EMAIL\", \"password\":\"$PASSWORD\"}")

echo "Login response: $login_response"

# Extract token and check HTTP status
TOKEN=$(echo "$login_response" | grep -oP '"Token":"\K[^"]+')
http_status=$(echo "$login_response" | grep "HTTP_STATUS" | awk -F: '{print $2}')

if [ -n "$TOKEN" ] && [ "$http_status" == "200" ]; then
    echo "Login successful. Token: $TOKEN"
else
    echo "Login failed with status $http_status."
    exit 1
fi


