#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MotorcycleRent.Api/MotorcycleRent.Api.csproj", "src/MotorcycleRent.Api/"]
COPY ["src/MotorcycleRent.Application/MotorcycleRent.Application.csproj", "src/MotorcycleRent.Application/"]
COPY ["src/MotorcycleRent.Core/MotorcycleRent.Core.csproj", "src/MotorcycleRent.Core/"]
COPY ["src/MotorcycleRent.Domain/MotorcycleRent.Domain.csproj", "src/MotorcycleRent.Domain/"]
COPY ["src/MotorcycleRent.IoC/MotorcycleRent.IoC.csproj", "src/MotorcycleRent.IoC/"]
COPY ["src/MotorcycleRent.Infrastructure/MotorcycleRent.Infrastructure.csproj", "src/MotorcycleRent.Infrastructure/"]
RUN dotnet restore "./src/MotorcycleRent.Api/MotorcycleRent.Api.csproj"
COPY . .
WORKDIR "/src/src/MotorcycleRent.Api"
RUN dotnet build "./MotorcycleRent.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MotorcycleRent.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MotorcycleRent.Api.dll"]