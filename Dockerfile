# 1. Usamos la imagen oficial de .NET 8 para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copiamos todas tus carpetas (DTOs, Data, Model, etc.)
COPY . .

# 3. Compilamos tu proyecto principal obligatoriamente
RUN dotnet publish Backend-Donaruma/Backend-Donaruma.csproj -c Release -o /app/publish

# 4. Preparamos el entorno de producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# 5. Le decimos a Railway por dónde escuchar
ENV ASPNETCORE_URLS=http://+:8080

# 6. ¡Encendemos la API!
ENTRYPOINT ["dotnet", "Backend-Donaruma.dll"]
