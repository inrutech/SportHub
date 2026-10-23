cd SportHub/bin/Debug/net10.0

dotnet SportHub.dll --environment "Staging" "--urls=http://*:11666"

dotnet publish SportHub -c Debug -o /Users/zero/Documents/Publish/Staging/SportHub

dotnet publish SportHub -c Release -o /Users/zero/Documents/Publish/Release/SportHub

dotnet publish SportHub -c Release -o /Users/zero/Documents/GitHub/SportHub_api

dotnet publish SportHub.Bot -c Release -o /Users/zero/Documents/GitHub/SportHub_bot_api -r linux-arm64

dotnet publish SportHub -c Release -o D:\代码\SportHub_api
dotnet publish SportHub.Bot -c Release -o D:\代码\SportHub_bot_api

dotnet tool update -g dotnet-ef

dotnet ef dbcontext scaffold "Server=localhost;Port=54321;User Id=geekee;Password=Geekee2018;Database=sporthub;Maximum Pool Size=500" Npgsql.EntityFrameworkCore.PostgreSQL  -o Model -c DBContext -p SportHub.Repository --startup-project SportHub -f --no-pluralize --no-onconfiguring --no-build
