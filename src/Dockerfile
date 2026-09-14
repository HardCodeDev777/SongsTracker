# Build
FROM dhi.io/dotnet:10-sdk AS build
WORKDIR /src
COPY ./*.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime
FROM dhi.io/aspnetcore:10
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SongsTracker.dll"]