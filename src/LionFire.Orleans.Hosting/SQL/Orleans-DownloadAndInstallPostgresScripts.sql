
-- create database lf_trading;
-- CREATE USER lf_trading WITH ENCRYPTED PASSWORD 'temp123';
--  GRANT ALL PRIVILEGES ON DATABASE lf_trading TO lf_trading;
-- Switch to database, then:
-- GRANT ALL ON SCHEMA public TO lf_trading;
-- alter user lf_trading password '123';

-- DECLARE    db_name TEXT := COALESCE(current_setting('my.db.name', true), 'LF_Trading');

-- After creating the database, connect to it in a new query window or session
-- Note: You cannot connect to the new database within the same DO block or function
-- So, you'll need to manually connect to the new database in Azure Data Studio

-- Then, execute these commands in a new query session connected to the newly created database:
SET search_path TO public; -- Or whatever schema you want to use

\! curl -s -o /tmp/PostgreSQL-Main.sql "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Shared/PostgreSQL-Main.sql"
\i /tmp/PostgreSQL-Main.sql
\! del \tmp\PostgreSQL-Main.sql

\! curl -s -o /tmp/PostgreSQL-Clustering.sql "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Clustering.AdoNet/PostgreSQL-Clustering.sql"
\i /tmp/PostgreSQL-Clustering.sql
\! del \tmp\PostgreSQL-Clustering.sql

\! curl -s -o /tmp/PostgreSQL-Persistence.sql "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Persistence.AdoNet/PostgreSQL-Persistence.sql"
\i /tmp/PostgreSQL-Persistence.sql
\! del \tmp\PostgreSQL-Persistence.sql

\! curl -s -o /tmp/PostgreSQL-Reminders.sql "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Reminders.AdoNet/PostgreSQL-Reminders.sql"
\i /tmp/PostgreSQL-Reminders.sql
\! del \tmp\PostgreSQL-Reminders.sql

\! curl -s -o /tmp/PostgreSQL-Streaming.sql "https://raw.githubusercontent.com/dotnet/orleans/refs/heads/main/src/AdoNet/Orleans.Streaming.AdoNet/PostgreSQL-Streaming.sql"
\i /tmp/PostgreSQL-Streaming.sql
\! del \tmp\PostgreSQL-Streaming.sql

