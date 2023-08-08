# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files and restore dependencies
COPY src .

# Publish the application with the 'Release' configuration
RUN dotnet publish TechMastery.MarketPlace.Api/TechMastery.MarketPlace.Api.csproj -c Release -o /app/out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the built application from the 'build' stage to the 'runtime' stage
COPY --from=build /app/out .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "TechMastery.MarketPlace.Api.dll"]
