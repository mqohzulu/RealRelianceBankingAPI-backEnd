FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution file
COPY RealRelianceBankingAPI.sln .

# Copy all .csproj files to their respective directories
COPY RealRelianceBankingAPI/RealRelianceBankingAPI.csproj RealRelianceBankingAPI/
COPY RealRelianceBanking.Application/RealRelianceBanking.Application.csproj RealRelianceBanking.Application/
COPY RealRelianceBanking.Domain/RealRelianceBanking.Domain.csproj RealRelianceBanking.Domain/
COPY RealRelianceBanking.Infrastructure/RealRelianceBanking.Infrastructure.csproj RealRelianceBanking.Infrastructure/

# Restore NuGet packages
RUN dotnet restore

# Copy all source code
COPY . .

# Build and publish the API project
RUN dotnet publish RealRelianceBankingAPI/RealRelianceBankingAPI.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "RealRelianceBankingAPI.dll"]