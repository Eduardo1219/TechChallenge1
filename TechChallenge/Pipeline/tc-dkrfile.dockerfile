FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TechChallenge/TechChallenge.csproj", "TechChallenge/"]
COPY ["Infraestructure/Infraestructure.csproj", "Infraestructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]
RUN dotnet restore "TechChallenge/TechChallenge.csproj"
COPY . .
WORKDIR "/src/TechChallenge"
RUN dotnet build "TechChallenge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechChallenge.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechChallenge.dll"]