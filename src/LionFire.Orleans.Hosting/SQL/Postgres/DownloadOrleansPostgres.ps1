Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Shared/PostgreSQL-Main.sql" -OutFile "PostgreSQL-Main.sql"
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Clustering.AdoNet/PostgreSQL-Clustering.sql" -OutFile "PostgreSQL-Clustering.sql"
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Persistence.AdoNet/PostgreSQL-Persistence.sql" -OutFile "PostgreSQL-Persistence.sql"
Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Reminders.AdoNet/PostgreSQL-Reminders.sql" -OutFile "PostgreSQL-Reminders.sql"
#Invoke-WebRequest -Uri "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Reminders.AdoNet/PostgreSQL-Streaming.sql" -OutFile "PostgreSQL-Streaming.sql"
