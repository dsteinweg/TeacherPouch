FROM mcr.microsoft.com/dotnet/aspnet:8.0.1 AS runtime
EXPOSE 5000
WORKDIR /app
COPY . /app/
ENV ASPNETCORE_URLs=http://+:5000
ENTRYPOINT ["dotnet", "TeacherPouch.dll"]
