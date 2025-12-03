FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

EXPOSE 40
EXPOSE 80

ENV Build=Release

WORKDIR /src

# Copy solution and csproj files first
COPY HireDaily.sln .
COPY src/HireDaily.Api/HireDaily.Api.csproj src/HireDaily.Api/
COPY src/HireDaily.Application/HireDaily.Application.csproj src/HireDaily.Application/
COPY src/HireDaily.Infra/HireDaily.Infra.csproj src/HireDaily.Infra/
COPY src/HireDaily.Core/HireDaily.Core.csproj src/HireDaily.Core/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build + publish
RUN dotnet publish src/HireDaily.Api/HireDaily.Api.csproj -c $Build -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HireDaily.Api.dll"]
# CMD ["--urls", "http://0.0.0.0:80"]