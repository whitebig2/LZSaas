FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8869

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apis/LZ.Product.API/LZ.Product.API.csproj", "Apis/LZ.Product.API/"]
RUN dotnet restore "./Apis/LZ.Product.API/LZ.Product.API.csproj"
COPY . .
WORKDIR "/src/Apis/LZ.Product.API"
RUN dotnet build "./LZ.Product.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LZ.Product.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LZ.Product.API.dll"]
ENV ASPNETCORE_HTTP_PORTS=8869
