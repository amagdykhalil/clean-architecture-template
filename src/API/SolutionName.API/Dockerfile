# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/SolutionName.API/SolutionName.API.csproj", "src/API/SolutionName.API/"]
COPY ["src/Core/SolutionName.Application/SolutionName.Application.csproj", "src/Core/SolutionName.Application/"]
COPY ["src/Core/SolutionName.Domain/SolutionName.Domain.csproj", "src/Core/SolutionName.Domain/"]
COPY ["src/Infrastructure/SolutionName.Infrastructure/SolutionName.Infrastructure.csproj", "src/Infrastructure/SolutionName.Infrastructure/"]
COPY ["src/Infrastructure/SolutionName.Persistence/SolutionName.Persistence.csproj", "src/Infrastructure/SolutionName.Persistence/"]
RUN dotnet restore "./src/API/SolutionName.API/SolutionName.API.csproj"
COPY . .
WORKDIR "/src/src/API/SolutionName.API"
RUN dotnet build "./SolutionName.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SolutionName.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolutionName.API.dll"]