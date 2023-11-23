using static System.Console;
using PasswordGenerator;
using Meziantou.Framework;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Npgsql;
using Serilog;
using Microsoft.Extensions.Logging;
using Dapper;

[Command("db")]
public class DatabaseCommands : ConsoleAppBase
{
    public DatabaseCommands()
    {
        Log.Logger.Information("Test log");
        //WriteLine($"Working dir: {Environment.CurrentDirectory}");
        //WriteLine("Temp dir: " + Path.GetTempPath());
    }

    public string GeneratePassword()
    {
        var passwordGenerator = new Password().IncludeLowercase().IncludeUppercase().IncludeSpecial("[]{}^_=").LengthRequired(30);
        var password = passwordGenerator.Next();
        return password;
    }

    #region Init scripts

    static ScriptList PostgresInitScripts = new ScriptList(
            "https://raw.githubusercontent.com/dotnet/orleans/main/src/AdoNet/Shared/PostgreSQL-Main.sql",
            "https://raw.githubusercontent.com/dotnet/orleans/main/src/AdoNet/Orleans.Clustering.AdoNet/PostgreSQL-Clustering.sql",
            "https://raw.githubusercontent.com/dotnet/orleans/main/src/AdoNet/Orleans.Persistence.AdoNet/PostgreSQL-Persistence.sql",
            "https://raw.githubusercontent.com/dotnet/orleans/main/src/AdoNet/Orleans.Reminders.AdoNet/PostgreSQL-Reminders.sql"
        );

    public Dictionary<DatabaseTypes, ScriptList> InitScripts = new Dictionary<DatabaseTypes, ScriptList>
    {
        [DatabaseTypes.postgres] = PostgresInitScripts,
    };

    #endregion

    [Command("connection-string", "Display the connection string")]
    public string DisplayConnectionString(
        [Option("d")] string database,
        [Option("h")] string host = "localhost",
        [Option("u", "User for authentication")] string? user = null,
        [Option("p", "Password for authentication")] string? password = null
        )
    {
        user ??= database;

        var connectionString = $"Host={host};Database={database};Username={user};Password={password}";
        WriteLine($"Connection string: {connectionString}");
        return connectionString;
    }

    [Command("user-pw", "Set user password")]
    public async Task ChangeUserPassword([Option(0)] string user, [Option(1)] string password
        , [Option("u", "User for authentication")] string authUser = "postgres"
        , [Option("p", "Password for authentication")] string authPassword = "postgres"
        , [Option("h")] string host = "localhost")
    {
        await using var dataSource = NpgsqlDataSource.Create($"Host={host};Username={authUser};Password={authPassword}");
        var sql = $"""
                    ALTER ROLE {user} PASSWORD '{password}';
                    """;
        await using var command = dataSource.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }

    [Command("user-pw-reset", "Reset user password")]
    public async Task ResetRolePassword([Option(0)] string user
             , [Option("u", "User for authentication")] string authUser = "postgres"
        , [Option("p", "Password for authentication")] string password = "postgres"
        , [Option("h")] string host = "localhost")
    {
        await using var dataSource = NpgsqlDataSource.Create($"Host={host};Username={authUser};Password={password}");
        var newPassword = GeneratePassword();
        var sql = $"""
                    ALTER ROLE {user} PASSWORD {newPassword};
                    """;
        await using var command = dataSource.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
        WriteLine($"New password for {user}: {newPassword}");
    }

    [Command("dl", "Download database init scripts")]
    public async Task DownloadDatabaseInitScripts([Option("force", DefaultValue = "true")] bool force = false)
    {
        HttpClient? http = null;

        async Task<bool> DownloadIfMissing(string script)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Directory.CreateDirectory(AppConfig.DownloadDir);
            }
            else
            {
                Directory.CreateDirectory(AppConfig.DownloadDir, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
            }
            var localFilename = Path.Combine(AppConfig.DownloadDir, UriUtils.GetFilenameFromUri(script));

            if (force && File.Exists(localFilename)) { File.Delete(localFilename); }
            var exists = File.Exists(localFilename);
            WriteLine($"{localFilename} exists: {exists}");
            if (exists) return false;
            http ??= new HttpClient();
            var sw = ValueStopwatch.StartNew();
            var httpResult = await http.GetAsync(script);
            WriteLine($"Downloaded {localFilename} in {sw.GetElapsedTime()}ms");
            await File.WriteAllBytesAsync(localFilename, await httpResult.Content.ReadAsByteArrayAsync());
            return true;
        }

        var scripts = InitScripts[DatabaseTypes.postgres];

        await Task.WhenAll(scripts.Uris.Select(s => DownloadIfMissing(s)));
    }

    [Command("create", "Create the database and initialize with Orleans init scripts")]
    public void Create([Option(0, "database name")] string database
        , [Option("u", "User for authentication")] string user = "postgres"
        , [Option("p", "Password for authentication")] string password = "postgres"
        , [Option("h")] string host = "localhost"
        , [Option("user")] string? databaseUser = null
        )
    {
        Task.Run(async () =>
        {
            Task download = DownloadDatabaseInitScripts();

            databaseUser ??= database;
            WriteLine($"Database name: {database}");
            WriteLine($"Database user: {databaseUser}");
            var childDatabasePassword = GeneratePassword();
            WriteLine($"Database password: {childDatabasePassword}");

            {
                await using var dataSource = NpgsqlDataSource.Create($"Host={host};Username={user};Password={password}");
                var sql = $"""
                    CREATE DATABASE {database};
                    CREATE ROLE {databaseUser} LOGIN  PASSWORD '{childDatabasePassword}';
                    GRANT ALL PRIVILEGES ON DATABASE {database} TO {databaseUser};
                    GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO {databaseUser};
                    """;
                await using var command = dataSource.CreateCommand(sql);
                await command.ExecuteNonQueryAsync();
            }

            {
                await download;
                await using var dataSource = NpgsqlDataSource.Create($"Host={host};Username={user};Password={password};Database={database}");

                foreach (var scriptPath in InitScripts[DatabaseTypes.postgres].Files)
                {
                    if (!File.Exists(scriptPath)) throw new Exception("Missing " + scriptPath);

                    var sql = await File.ReadAllTextAsync(scriptPath);
                    await using var command = dataSource.CreateCommand(sql);
                    await command.ExecuteNonQueryAsync();
                }
            }

            WriteLine("Init scripts executed");

            DisplayConnectionString(database, host, databaseUser, childDatabasePassword);
            //await using var connection = await dataSource.OpenConnectionAsync();

            //await using var conn = new NpgsqlConnection(connString);
            //await conn.OpenAsync();

            //await using (var cmd = new NpgsqlCommand("INSERT INTO data (some_field) VALUES (@p)", conn))
            //{
            //    cmd.Parameters.AddWithValue("p", "Hello world");
            //    await cmd.ExecuteNonQueryAsync();
            //}

            //await using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
            //await using (var reader = await cmd.ExecuteReaderAsync())
            //{
            //    while (await reader.ReadAsync())
            //        Console.WriteLine(reader.GetString(0));
            //}

        }).Wait();

    }


    public static string GetConnectionString(string host, string user, string password, string? database = null)
    {
        if (database != null) return $"Host={host};Username={user};Password={password};Database={database}";
        return $"Host={host};Username={user};Password={password}";
    }
    public static NpgsqlDataSource GetDataSource(string host, string user, string password, string? database = null)
        => NpgsqlDataSource.Create(GetConnectionString(host, user, password, database));
    public NpgsqlDataSource GetDataSource()
        => NpgsqlDataSource.Create(ConnectionString ?? throw new ArgumentNullException(nameof(ConnectionString)));
    public static NpgsqlConnection GetConnection(string host, string user, string password, string? database = null)
        => new NpgsqlConnection(GetConnectionString(host, user, password, database));
    public NpgsqlConnection GetConnection()
         => new NpgsqlConnection(ConnectionString ?? throw new ArgumentNullException(nameof(ConnectionString)));

    public string? ConnectionString { get; set; }

    public async Task<bool> DatabaseExists(string database)
    {
        using var connection = GetConnection();
        connection.Open();
        var value = await connection.QueryAsync("SELECT 1 FROM pg_database WHERE datname = @database;", new { database });
        return value.Any();
    }

    public async Task<bool> RoleExists(string databaseUser)
    {
        using var connection = GetConnection();
        connection.Open();
        var value = await connection.QueryAsync("SELECT 1 FROM pg_roles WHERE rolname = @databaseUser;", new { databaseUser });
        return value.Any();
    }

    [Command("teardown", "Drop the database and the associated user")]
    public async Task Teardown([Option(0, "database name")] string database
        , [Option("u", "User for authentication")] string user = "postgres"
        , [Option("p", "Password for authentication")] string password = "postgres"
        , [Option("h")] string host = "localhost"
        , [Option("user")] string? databaseUser = null
        )
    {
        ConnectionString = GetConnectionString(host, user, password);

        databaseUser ??= database;

        bool databaseExists = await DatabaseExists(database);
        bool roleExists = await RoleExists(databaseUser);

        if (!databaseExists) Context.Logger.LogInformation($"Database does not exist: {database}");
        if (!roleExists) Context.Logger.LogInformation($"Role does not exist: {databaseUser}");
        if (!databaseExists && !roleExists) { Context.Logger.LogInformation($"Nothing to do.  Quitting."); Context.Terminate(); }


        WriteLine("This is a destructive operation.  Are you sure you wish to destroy these two things?");
        if(databaseExists) WriteLine($" - Database: {database}");
        if(roleExists) WriteLine($" - Database user: {databaseUser}");
        WriteLine("Type 'destroy' to continue");
        var read = Console.ReadLine();
        if (read == null) Context.Terminate();

        if (read != "destroy")
        {
            WriteLine("User did not choose to proceed.");
            Context.Terminate();
            return;
        }

        databaseUser ??= database;

        await using var dataSource = GetDataSource();
    
        if(databaseExists)
        {
            try
            {
                await using var command = dataSource.CreateCommand($"DROP DATABASE {database};");
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Context.Logger.LogError(ex, $"Failed to drop database: {database}");
            }
        }

        if (roleExists)
        {
            try
            {
                await using var command = dataSource.CreateCommand($"DROP ROLE {databaseUser};");
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Context.Logger.LogError(ex, $"Failed to drop role: {databaseUser}");
            }
        }
    }
}