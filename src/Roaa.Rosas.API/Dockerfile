  
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Roaa.Rosas.API/Roaa.Rosas.API.csproj", "src/Roaa.Rosas.API/"]
COPY ["src/Roaa.Rosas.Application/Roaa.Rosas.Application.csproj", "src/Roaa.Rosas.Application/"]
COPY ["src/Roaa.Rosas.Authorization/Roaa.Rosas.Authorization.csproj", "src/Roaa.Rosas.Authorization/"]
COPY ["src/Roaa.Rosas.Common/Roaa.Rosas.Common.csproj", "src/Roaa.Rosas.Common/"]
COPY ["src/Roaa.Rosas.Domain/Roaa.Rosas.Domain.csproj", "src/Roaa.Rosas.Domain/"]
COPY ["src/Roaa.Rosas.Infrastructure/Roaa.Rosas.Infrastructure.csproj", "src/Roaa.Rosas.Infrastructure/"]
RUN dotnet restore "src/Roaa.Rosas.API/Roaa.Rosas.API.csproj"
COPY . .
WORKDIR "/src/src/Roaa.Rosas.API"
RUN dotnet build "Roaa.Rosas.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Roaa.Rosas.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Roaa.Rosas.API.dll"]