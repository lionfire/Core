using static System.Console;
using PasswordGenerator;
using Meziantou.Framework;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Npgsql;
using Serilog;
using Microsoft.Extensions.Logging;
using Dapper;
using ConsoleAppFramework;

[RegisterCommands("db")]
public class DatabaseCommands //: ConsoleAppBase
{
    public DatabaseCommands(ILogger<DatabaseCommands> logger)
    {
        Log.Logger.Information("Test log");
        Logger = logger;
        //WriteLine($"Working dir: {Environment.CurrentDirectory}");
        //WriteLine("Temp dir: " + Path.GetTempPath());
    }

    protected string GeneratePassword()
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

    /// <summary>
    /// Display the connection string 
    /// </summary>
    /// <param name="database"></param>
    /// <param name="host"></param>
    /// <param name="user">User for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <returns></returns>
    [Command("connection-string")]
    public void DisplayConnectionString(
        /*[Argument]*/ string database,
        /*[Argument] */string host = "localhost",
        /*[Argument("u")]*/ string? user = null,
        /*[Argument("p")] */string? password = null
        )
    {
        user ??= database;

        var connectionString = $"Host={host};Database={database};Username={user};Password={password}";
        WriteLine($"Connection string: {connectionString}");
        //return connectionString;
    }

    /// <summary>
    /// Set user password 
    /// </summary>
    /// <param name="user">User for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <param name="authUser"></param>
    /// <param name="authPassword"></param>
    /// <param name="host"></param>
    /// <returns></returns>
    [Command("user-pw")]
    public async Task ChangeUserPassword([Argument] string user, [Argument] string password
        , /*[Argument("u")] */string authUser = "postgres"
        , /*[Argument("p", "Password for authentication")] */string authPassword = "postgres"
        , /*[Argument("h")] */string host = "localhost")
    {
        await using var dataSource = NpgsqlDataSource.Create($"Host={host};Username={authUser};Password={authPassword}");
        var sql = $"""
                    ALTER ROLE {user} PASSWORD '{password}';
                    """;
        await using var command = dataSource.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Reset user password
    /// </summary>
    /// <param name="user"></param>
    /// <param name="authUser">User for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <param name="host"></param>
    /// <returns></returns>
    [Command("user-pw-reset")]
    public async Task ResetRolePassword([Argument] string user
             , /*[Argument("u", "User for authentication")]*/ string authUser = "postgres"
        , /*[Argument("p", "Password for authentication")] */string password = "postgres"
        , /*[Argument("h")] */string host = "localhost")
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

    /// <summary>
    /// Download database init scripts
    /// </summary>
    /// <param name="force"></param>
    /// <returns></returns>
    [Command("dl")]
    public async Task DownloadDatabaseInitScripts(/*[Argument("force", DefaultValue = "true")] */bool force = false) // TODO: Set default of force to true
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

    /// <summary>
    /// Create the database and initialize with Orleans init scripts
    /// </summary>
    /// <param name="database">database name</param>
    /// <param name="user">User for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <param name="host"></param>
    /// <param name="databaseUser"></param>
    /// <exception cref="Exception"></exception>
    [Command("create")]
    public void Create([Argument] string database
        , /*[Argument("u", "User for authentication")]*/ string user = "postgres"
        , /*[Argument("p", "Password for authentication")]*/ string password = "postgres"
        , /*[Argument("h")]*/ string host = "localhost"
        , /*[Argument("user")]*/ string? databaseUser = null
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
    protected NpgsqlDataSource GetDataSource()
        => NpgsqlDataSource.Create(ConnectionString ?? throw new ArgumentNullException(nameof(ConnectionString)));
    public static NpgsqlConnection GetConnection(string host, string user, string password, string? database = null)
        => new NpgsqlConnection(GetConnectionString(host, user, password, database));
    protected NpgsqlConnection GetConnection()
         => new NpgsqlConnection(ConnectionString ?? throw new ArgumentNullException(nameof(ConnectionString)));

    public string? ConnectionString { get; set; }
    public ILogger<DatabaseCommands> Logger { get; }

    protected async Task<bool> DatabaseExists(string database)
    {
        using var connection = GetConnection();
        connection.Open();
        var value = await connection.QueryAsync("SELECT 1 FROM pg_database WHERE datname = @database;", new { database });
        return value.Any();
    }

    protected async Task<bool> RoleExists(string databaseUser)
    {
        using var connection = GetConnection();
        connection.Open();
        var value = await connection.QueryAsync("SELECT 1 FROM pg_roles WHERE rolname = @databaseUser;", new { databaseUser });
        return value.Any();
    }

    /// <summary>
    /// Drop the database and the associated user
    /// </summary>
    /// <param name="database">database name</param>
    /// <param name="user">User for authentication</param>
    /// <param name="password">Password for authentication</param>
    /// <param name="host"></param>
    /// <param name="databaseUser"></param>
    /// <returns></returns>
    [Command("teardown")]
    public async Task Teardown(/*[Argument(0)] */string database
        , /*[Argument("u", "User for authentication")]*/ string user = "postgres"
        , /*[Argument("p", "Password for authentication")]*/ string password = "postgres"
        , /*[Argument("h")] */string host = "localhost"
        , /*[Argument("user")]*/ string? databaseUser = null
         
        )
    {
        ConnectionString = GetConnectionString(host, user, password);

        databaseUser ??= database;

        bool databaseExists = await DatabaseExists(database);
        bool roleExists = await RoleExists(databaseUser);

        if (!databaseExists) Logger.LogInformation($"Database does not exist: {database}");
        if (!roleExists) Logger.LogInformation($"Role does not exist: {databaseUser}");
        if (!databaseExists && !roleExists) { Logger.LogInformation($"Nothing to do.  Quitting."); return /*Context.Terminate()*/; }


        WriteLine("This is a destructive operation.  Are you sure you wish to destroy these two things?");
        if(databaseExists) WriteLine($" - Database: {database}");
        if(roleExists) WriteLine($" - Database user: {databaseUser}");
        WriteLine("Type 'destroy' to continue");
        var read = Console.ReadLine();
        if (read == null) { return; /* Context.Terminate();*/ }

        if (read != "destroy")
        {
            WriteLine("User did not choose to proceed.");
            //Context.Terminate();
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
                Logger.LogError(ex, $"Failed to drop database: {database}");
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
                Logger.LogError(ex, $"Failed to drop role: {databaseUser}");
            }
        }
    }
}