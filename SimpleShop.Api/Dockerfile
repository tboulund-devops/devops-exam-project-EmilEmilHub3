# -----------------------------
# Build stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (better Docker cache usage)
COPY SimpleShop.sln .
COPY SimpleShop.Api/SimpleShop.Api.csproj SimpleShop.Api/

# Restore dependencies
RUN dotnet restore SimpleShop.Api/SimpleShop.Api.csproj

# Copy the remaining source code
COPY SimpleShop.Api/ SimpleShop.Api/

# Publish the application
RUN dotnet publish SimpleShop.Api/SimpleShop.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# -----------------------------
# Runtime stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN useradd -m appuser

# Copy published app from build stage
COPY --from=build /app/publish .

# Switch to non-root user
USER appuser

# Configure ASP.NET port
ENV ASPNETCORE_URLS=http://+:8080

# Expose container port
EXPOSE 8080

# Start application
ENTRYPOINT ["dotnet", "SimpleShop.Api.dll"]