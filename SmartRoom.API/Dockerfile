# 1. Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copy csproj dulu supaya layer cache Docker lebih efisien
# (Sesuaikan path jika Dockerfile ada di root folder backend)
COPY ["SmartRoom.API.csproj", "./"]
RUN dotnet restore "SmartRoom.API.csproj"

# 3. Copy seluruh file sisanya
COPY . .

# 4. Build aplikasi
RUN dotnet build "SmartRoom.API.csproj" -c Release -o /app/build

# 5. Publish aplikasi
FROM build AS publish
RUN dotnet publish "SmartRoom.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 7. Konfigurasi Port & Environment
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "SmartRoom.API.dll"]