FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DotnetMonolith.sln", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["src/DotnetMonolith.Api/DotnetMonolith.Api.csproj", "src/DotnetMonolith.Api/"]
COPY ["tests/DotnetMonolith.Api.Tests/DotnetMonolith.Api.Tests.csproj", "tests/DotnetMonolith.Api.Tests/"]
RUN dotnet restore "DotnetMonolith.sln"

COPY . .
RUN dotnet publish "src/DotnetMonolith.Api/DotnetMonolith.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DotnetMonolith.Api.dll"]
