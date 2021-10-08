# Issue restoring the nuget packages was fixed from below
# Get-NetIPInterface -AddressFamily IPv4 | Sort-Object -Property InterfaceMetric -Descending
# Following line is to move a disconnected iterface with an alias of Ethernet to an
# Interfacemetric of 4 which was using a higher number that the enabled/connected interface
# Set-NetIPInterface -InterfaceAlias 'Ethernet' -InterfaceMetric 4
# https://improveandrepeat.com/2019/09/how-to-fix-network-errors-with-docker-and-windows-containers/

# docker build -t homebrewapi-image .
# docker run -d -p 5000:59607 --name myhomebrewapi homebrewapi-image
# docker-compose up

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "thehomebrewapi.dll"]