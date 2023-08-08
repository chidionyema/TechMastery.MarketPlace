# Use the official .NET SDK image with version 7
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy and restore project dependencies
COPY TechMastery.MarketPlace.Api.csproj .
RUN dotnet restore

# Copy the remaining source code
COPY . .

# Build the application with the 'Release' configuration
RUN dotnet build -c Release -o /app/out

# Build the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the 'build' stage to the 'runtime' stage
COPY --from=build /app/out ./

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "TechMastery.MarketPlace.Api.dll"]
