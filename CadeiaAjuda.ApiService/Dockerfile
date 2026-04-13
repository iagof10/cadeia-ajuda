FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CadeiaAjuda.ApiService/CadeiaAjuda.ApiService.csproj", "CadeiaAjuda.ApiService/"]
COPY ["CadeiaAjuda.ServiceDefaults/CadeiaAjuda.ServiceDefaults.csproj", "CadeiaAjuda.ServiceDefaults/"]
RUN dotnet restore "CadeiaAjuda.ApiService/CadeiaAjuda.ApiService.csproj"

COPY . .
WORKDIR "/src/CadeiaAjuda.ApiService"
RUN dotnet publish "CadeiaAjuda.ApiService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CadeiaAjuda.ApiService.dll"]
