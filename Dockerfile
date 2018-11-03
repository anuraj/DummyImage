FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
RUN apt-get update && apt-get install -y libc6-dev libgdiplus
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["DummyImage.csproj", "./"]
RUN dotnet restore "./DummyImage.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DummyImage.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DummyImage.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DummyImage.dll"]
