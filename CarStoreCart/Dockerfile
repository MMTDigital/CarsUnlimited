FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY CarStoreCart/CarStoreCart.csproj CarStoreCart/
RUN dotnet restore "CarStoreCart/CarStoreCart.csproj"
COPY . .
WORKDIR "/src/CarStoreCart"
RUN dotnet build "CarStoreCart.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CarStoreCart.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CarStoreCart.dll"]
