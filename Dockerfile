FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY backend/Portfolio.Domain/Portfolio.Domain.csproj backend/Portfolio.Domain/
COPY backend/Portfolio.Application/Portfolio.Application.csproj backend/Portfolio.Application/
COPY backend/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj backend/Portfolio.Infrastructure/
COPY backend/Portfolio.Api/Portfolio.Api.csproj backend/Portfolio.Api/
RUN dotnet restore backend/Portfolio.Api/Portfolio.Api.csproj
COPY backend/ backend/
RUN dotnet publish backend/Portfolio.Api/Portfolio.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
