#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8865

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apis/LZ.Statistics.API/LZ.Statistics.API.csproj", "Apis/LZ.Statistics.API/"]
RUN dotnet restore "./Apis/LZ.Statistics.API/LZ.Statistics.API.csproj"
COPY . .
WORKDIR "/src/Apis/LZ.Statistics.API"
RUN dotnet build "./LZ.Statistics.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LZ.Statistics.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LZ.Statistics.API.dll"]
ENV ASPNETCORE_HTTP_PORTS=8865