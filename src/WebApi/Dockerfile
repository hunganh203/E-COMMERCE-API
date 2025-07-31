#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 6699

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["../src/WebApi/WebApi.csproj", "src/WebApi/"]
COPY ["../src/Domain/Domain.csproj", "src/Domain/"]
COPY ["../src/EfCore.Persistence/EfCore.Persistence.csproj", "src/EfCore.Persistence/"]
COPY ["../src/Application/Application.csproj", "src/Application/"]
COPY ["../src/Cloud.Service/Cloud.Service.csproj", "src/Cloud.Service/"]
COPY ["../src/Shared/Shared.csproj", "src/Shared/"]
RUN dotnet restore "src/WebApi/WebApi.csproj"
COPY . .
WORKDIR "/src/src/WebApi"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]