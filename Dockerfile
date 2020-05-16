FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim 
WORKDIR /app
COPY out/ .
ENTRYPOINT ["dotnet", "K9Nano.Logging.Web.dll"]