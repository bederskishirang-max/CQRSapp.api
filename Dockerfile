FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["CQRSapp.api/CQRSapp.api.csproj", "CQRSapp.api/"]
COPY ["CQRSapp.Application/CQRSapp.Application.csproj", "CQRSapp.Application/"]
COPY ["CQRSapp.Domain/CQRSapp.Domain.csproj", "CQRSapp.Domain/"]
COPY ["CQRSapp.Infrastructure/CQRSapp.Infrastructure.csproj", "CQRSapp.Infrastructure/"]

RUN dotnet restore "CQRSapp.api/CQRSapp.api.csproj"

COPY . .

WORKDIR "/src/CQRSapp.api"
RUN dotnet build "CQRSapp.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CQRSapp.api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 80 443

ENTRYPOINT ["dotnet", "CQRSapp.api.dll"]
