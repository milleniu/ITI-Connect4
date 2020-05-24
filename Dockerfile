# ------ BUILD SERVER
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-server

WORKDIR /src

COPY Server .

RUN dotnet restore
RUN dotnet publish -c release -o /app --no-self-contained --no-restore

# ------ BUILD FRONT
FROM node:current-alpine3.10 AS build-front

WORKDIR /src
COPY Front .

RUN npm install &&\
    npx elm make src/Main.elm &&\
    echo '<link rel="stylesheet" href="style.css">' >> index.html

# ------ FINAL IMAGE
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine

WORKDIR /src

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build-server /app ./
COPY --from=build-front /src/index.html ./wwwroot/
COPY --from=build-front /src/style.css ./wwwroot/

EXPOSE 8080

ENTRYPOINT ["./ITI.Connect4"]