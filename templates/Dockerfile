# TEMPLATE. Copy to your repository root. Change three things:
#   1. the project path in the restore and publish lines;
#   2. the entry point assembly name on the last line;
#   3. the COPY list, if your build needs more than Directory.Build.props and src/.
#      Directory.Build.props is templates/Directory.Build.props, copied to the root.
# Keep the two stages and keep USER $APP_UID: the runtime image ships that
# non-root account, and running as root is the default nobody notices.
# Build stage: the SDK image restores from the committed lock files and publishes the host.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props ./
COPY src/ ./src/

# Locked mode fails the build if a package resolves to anything the lock files do not list.
RUN dotnet restore src/Helpdesk.Host/Helpdesk.Host.csproj --locked-mode

RUN dotnet publish src/Helpdesk.Host/Helpdesk.Host.csproj \
    --configuration Release \
    --no-restore \
    --output /app

# Runtime stage: no SDK, no source, no build output beyond what publish produced.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app ./

# APP_UID is the non-root account the dotnet base images ship with.
USER $APP_UID

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Helpdesk.Host.dll"]
