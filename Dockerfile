# Stage 1: build (dùng SDK image nặng để biên dịch)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj trước để tận dụng layer cache
COPY src/ProductApi/ProductApi.csproj src/ProductApi/
RUN dotnet restore src/ProductApi/ProductApi.csproj

# Copy toàn bộ source rồi publish
COPY . .
RUN dotnet publish src/ProductApi/ProductApi.csproj -c Release -o /app/publish

# Stage 2: runtime (chỉ mang runtime nhẹ + file đã publish)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductApi.dll"]
