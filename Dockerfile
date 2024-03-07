FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 80
EXPOSE 443


COPY *.csproj ./
RUN dotnet restore /p:IsDockerBuild=true -v diag
COPY . .

RUN dotnet publish  -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BNB_challenge_backend.dll"]

