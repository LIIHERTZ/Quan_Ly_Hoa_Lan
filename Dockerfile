FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Presentation/QuanLyHoaLan.API/QuanLyHoaLan.API.csproj", "Presentation/QuanLyHoaLan.API/"]
COPY ["Core/QuanLyHoaLan.Application/QuanLyHoaLan.Application.csproj", "Core/QuanLyHoaLan.Application/"]
COPY ["Core/QuanLyHoaLan.Domain/QuanLyHoaLan.Domain.csproj", "Core/QuanLyHoaLan.Domain/"]
COPY ["Infrastructure/QuanLyHoaLan.Infrastructure/QuanLyHoaLan.Infrastructure.csproj", "Infrastructure/QuanLyHoaLan.Infrastructure/"]
RUN dotnet restore "Presentation/QuanLyHoaLan.API/QuanLyHoaLan.API.csproj"
COPY . .
WORKDIR "/src/Presentation/QuanLyHoaLan.API"
RUN dotnet build "QuanLyHoaLan.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuanLyHoaLan.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuanLyHoaLan.API.dll"]
