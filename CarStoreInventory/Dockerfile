FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY CarStoreInventory/CarStoreInventory.csproj CarStoreInventory/
COPY CarStoreShared/CarStoreShared.csproj CarStoreShared/
RUN dotnet restore "CarStoreInventory/CarStoreInventory.csproj"
COPY . .
WORKDIR "/src/CarStoreInventory"
RUN dotnet build "CarStoreInventory.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarStoreInventory.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarStoreInventory.dll"]
