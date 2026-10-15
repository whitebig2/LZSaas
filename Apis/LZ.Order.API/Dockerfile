#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8870

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apis/LZ.Order.API/LZ.Order.API.csproj", "Apis/LZ.Order.API/"]
RUN dotnet restore "./Apis/LZ.Order.API/LZ.Order.API.csproj"
COPY . .
WORKDIR "/src/Apis/LZ.Order.API"
RUN dotnet build "./LZ.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LZ.Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LZ.Order.API.dll"]
ENV ASPNETCORE_HTTP_PORTS=8870
