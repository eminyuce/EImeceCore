FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 35870
EXPOSE 44326

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["EImeceCore/EImeceCore.csproj", "EImeceCore/"]
RUN dotnet restore "EImeceCore/EImeceCore.csproj"
COPY . .
WORKDIR "/src/EImeceCore"
RUN dotnet build "EImeceCore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "EImeceCore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "EImeceCore.dll"]