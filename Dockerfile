FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props .editorconfig ./
COPY src/Bookshelf.Domain/Bookshelf.Domain.csproj src/Bookshelf.Domain/
COPY src/Bookshelf.Application/Bookshelf.Application.csproj src/Bookshelf.Application/
COPY src/Bookshelf.Infrastructure/Bookshelf.Infrastructure.csproj src/Bookshelf.Infrastructure/
COPY src/Bookshelf.Web/Bookshelf.Web.csproj src/Bookshelf.Web/
RUN dotnet restore src/Bookshelf.Web/Bookshelf.Web.csproj

COPY src/ src/
RUN dotnet publish src/Bookshelf.Web/Bookshelf.Web.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p wwwroot/uploads/covers && chown -R $APP_UID wwwroot/uploads

USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Bookshelf.Web.dll"]
