FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csprojs primeiro (cache de restore)
COPY CadeiaAjuda.ServiceDefaults/CadeiaAjuda.ServiceDefaults.csproj CadeiaAjuda.ServiceDefaults/
COPY CadeiaAjuda.Web/CadeiaAjuda.Web.csproj CadeiaAjuda.Web/
RUN dotnet restore CadeiaAjuda.Web/CadeiaAjuda.Web.csproj

# Copiar código e publicar
COPY CadeiaAjuda.ServiceDefaults/ CadeiaAjuda.ServiceDefaults/
COPY CadeiaAjuda.Web/ CadeiaAjuda.Web/
RUN dotnet publish CadeiaAjuda.Web/CadeiaAjuda.Web.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CadeiaAjuda.Web.dll"]
