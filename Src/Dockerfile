#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_Kestrel__Certificates__Default__Password=P@ssw0rd!
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/apigateway.pfx

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Src/APIGateway.csproj", "Src/"]
RUN dotnet restore "Src/APIGateway.csproj"
COPY . .
WORKDIR "/src/Src"
RUN dotnet dev-certs https -ep /app/https/apigateway.pfx -p P@ssw0rd!
RUN dotnet build "APIGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIGateway.csproj" -c Release -o /app/publish
RUN cp /app/https/apigateway.pfx /app/publish/apigateway.pfx

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "APIGateway.dll"]
