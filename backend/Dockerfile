# ASP.NET Core on Railway — Docker build (Railpack does not support .NET yet).
# https://docs.railway.app/guides/aspnet-core
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY NeighborhoodIntel.Api.csproj .
RUN dotnet restore NeighborhoodIntel.Api.csproj

COPY . .
RUN dotnet publish NeighborhoodIntel.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Railway sets PORT at runtime; Program.cs also binds to PORT.
CMD ["sh", "-c", "exec dotnet NeighborhoodIntel.Api.dll --urls http://0.0.0.0:${PORT}"]
