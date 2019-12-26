FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build

WORKDIR /app

COPY ./*.fsproj ./
RUN dotnet restore

COPY ./* ./
RUN dotnet publish -c Release --no-restore --output /app/pkg

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 as runtime

WORKDIR /app
COPY --from=build /app/pkg ./
ENTRYPOINT [ "blackhole" ]
