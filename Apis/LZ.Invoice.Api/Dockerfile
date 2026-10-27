# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8873

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Apis/LZ.Invoice.Api/LZ.Invoice.Api.csproj", "Apis/LZ.Invoice.Api/"]
RUN dotnet restore "./Apis/LZ.Invoice.Api/LZ.Invoice.Api.csproj"
COPY . .
WORKDIR "/src/Apis/LZ.Invoice.Api"
RUN dotnet build "./LZ.Invoice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LZ.Invoice.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LZ.Invoice.Api.dll"]
ENV ASPNETCORE_HTTP_PORTS=8873
