FROM node:10 as css-build
WORKDIR /app
COPY ./js_src ./
RUN npm install
RUN npx tailwind build styles.css > final.css


FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./CertBag/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./CertBag ./
COPY --from=css-build /app/final.css ./wwwroot/css/styles.css
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:sdk

WORKDIR /app

RUN mkdir /db
ENV DATABASE_CONNECTION_STRING "Data Source=/db/certbag.db"

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CertBag.dll"]