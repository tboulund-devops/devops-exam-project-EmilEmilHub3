FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY SimpleShop.sln .
COPY SimpleShop.Api/SimpleShop.Api.csproj SimpleShop.Api/

RUN dotnet restore SimpleShop.Api/SimpleShop.Api.csproj

COPY . .
RUN dotnet publish SimpleShop.Api/SimpleShop.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleShop.Api.dll"]