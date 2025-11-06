FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Pcf.Preferences.WebHost/Pcf.Preferences.WebHost.csproj", "Pcf.Preferences.WebHost/"]
COPY ["Pcf.Preferences.Core/Pcf.Preferences.Core.csproj", "Pcf.Preferences.Core/"]
COPY ["Pcf.Preferences.DataAccess/Pcf.Preferences.DataAccess.csproj", "Pcf.Preferences.DataAccess/"]
RUN dotnet restore "Pcf.Preferences.WebHost/Pcf.Preferences.WebHost.csproj"
COPY . .
WORKDIR "/src/Pcf.Preferences.WebHost"
RUN dotnet build "Pcf.Preferences.WebHost.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pcf.Preferences.WebHost.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pcf.Preferences.WebHost.dll"]
