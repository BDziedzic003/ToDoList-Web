FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ToDoList.Api/ToDoList.Api.csproj", "ToDoList.Api/"]
COPY ["ToDoList.Core/ToDoList.Core.csproj", "ToDoList.Core/"]

RUN dotnet restore "ToDoList.Api/ToDoList.Api.csproj"

COPY . .

WORKDIR "/src/ToDoList.Api"
RUN dotnet publish "ToDoList.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ToDoList.Api.dll"]