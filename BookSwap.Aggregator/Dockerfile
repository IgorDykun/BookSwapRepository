FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["BookSwap.Aggregator/BookSwap.Aggregator.csproj", "BookSwap.Aggregator/"]
COPY ["BookSwap.Infrastructure/BookSwap.Infrastructure.csproj", "BookSwap.Infrastructure/"]
COPY ["BookSwap.Domain/BookSwap.Domain.csproj", "BookSwap.Domain/"]
COPY ["BookSwap.GrpcContracts/BookSwap.GrpcContracts.csproj", "BookSwap.GrpcContracts/"]
COPY ["BookSwap.Application/BookSwap.Application.csproj", "BookSwap.Application/"]

RUN dotnet restore "BookSwap.Aggregator/BookSwap.Aggregator.csproj"

COPY . .
WORKDIR "/src/BookSwap.Aggregator"
RUN dotnet build "BookSwap.Aggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookSwap.Aggregator.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookSwap.Aggregator.dll"]