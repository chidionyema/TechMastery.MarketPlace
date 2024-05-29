# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /app



# Copy the source code of your service and its direct dependencies
COPY TechMastery.UsermanagementService/ TechMastery.UsermanagementService/
COPY TechMastery.MarketPlace.Messaging/ TechMastery.MarketPlace.Messaging/  
COPY TechMastery.MarketPlace.Application/ TechMastery.MarketPlace.Application/  
COPY TechMastery.MarketPlace.Domain/ TechMastery.MarketPlace.Domain/  
COPY TechMastery.MarketPlace.Persistence/ TechMastery.MarketPlace.Persistence/  
COPY TechMastery.MarketPlace.Applcation.Contracts/ TechMastery.MarketPlace.Applcation.Contracts/  
COPY TechMastery.MarketPlace.Persistence.Contracts/ TechMastery.MarketPlace.Persistence.Contracts/  
COPY TechMastery.MarketPlace.Messaging.Contracts/ TechMastery.MarketPlace.Messaging.Contracts/  

# Restore the specific project to avoid restoring unnecessary projects
RUN dotnet restore TechMastery.UsermanagementService/TechMastery.UsermanagementService.csproj

# Publish the application with the 'Release' configuration
RUN dotnet publish TechMastery.UsermanagementService/TechMastery.UsermanagementService.csproj -c Release -o /out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime

WORKDIR /app

# Copy the built application from the 'build' stage to the 'runtime' stage
COPY --from=build /out .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "TechMastery.UsermanagementService.dll"]
