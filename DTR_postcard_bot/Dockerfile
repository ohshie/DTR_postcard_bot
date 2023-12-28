FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DTR_postcard_bot/DTR_postcard_bot.csproj", "DTR_postcard_bot/"]
RUN dotnet restore "DTR_postcard_bot/DTR_postcard_bot.csproj"
COPY . .
WORKDIR "/src/DTR_postcard_bot"
RUN dotnet build "DTR_postcard_bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DTR_postcard_bot.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DTR_postcard_bot.dll"]
