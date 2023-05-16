FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /
COPY . ./
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runenv
WORKDIR /
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "MoviesApi.dll"]
EXPOSE 80