#nullable enable
using Microsoft.Extensions.Configuration;
using Npgsql;
using Orleans;
using Orleans.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LionFire.Orleans_.SQL.Postgres;

public class InitOrleansPostgres : IStartupTask, ILifecycleParticipant<ISiloLifecycle>
{
    public ILogger<InitOrleansPostgres> Logger { get; }

    private string? ConnectionString;

    public void Participate(ISiloLifecycle lifecycle)
    {
        lifecycle.Subscribe("InitOrleansPostgres", 100, Execute, ct => Task.CompletedTask);
    }

    public InitOrleansPostgres(ILogger<InitOrleansPostgres> logger, IConfiguration configuration)
    {
        this.Logger = logger;

        ConnectionString = configuration["Postgres:ConnectionString"];
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        if (ConnectionString != null)
        {
            await InitializeTableAsync("OrleansQuery", "LionFire.Orleans_.SQL.Postgres.PostgreSQL-Main.sql");
            await InitializeTableAsync("OrleansStorage", "LionFire.Orleans_.SQL.Postgres.PostgreSQL-Persistence.sql");
            await InitializeTableAsync("OrleansMembershipTable", "LionFire.Orleans_.SQL.Postgres.PostgreSQL-Clustering.sql");
            await InitializeTableAsync("OrleansRemindersTable", "LionFire.Orleans_.SQL.Postgres.PostgreSQL-Reminders.sql");
        }

        ConnectionString = null;
    }
    
    //public IDisposable Subscribe(string observerName, int stage, ILifecycleObserver observer)
    //{
    //    // Subscribe to a specific stage
    //    if (stage == DefaultLifecycleStage.Init)
    //    {
    //        return LifecycleObservableExtensions.Subscribe(observer, stage, OnStart, OnStop);
    //    }
    //    return null;
    //}

    public async Task InitializeTableAsync(string tableName, string scriptResourceName, string? schemaName = null)
    {
        schemaName ??= "public";

        // Check if the table exists
        bool tableExists;
        await using (var conn = new NpgsqlConnection(ConnectionString))
        {
            await conn.OpenAsync();
            var query = "SELECT EXISTS (SELECT FROM pg_tables WHERE schemaname = @schemaName AND tablename = @tableName)";
            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("schemaName", schemaName);
                cmd.Parameters.AddWithValue("tableName", tableName.ToLower()); // PostgreSQL table names are case-insensitive
                tableExists = (bool)await cmd.ExecuteScalarAsync();
            }
        }

        // If the table exists, no need to run the script
        if (tableExists)
        {
            Logger.LogTrace("Table {SchemaName}.{TableName} already exists.", schemaName, tableName);
            return;
        }

        // Table is missing, execute the embedded script
        Logger.LogInformation("Table {SchemaName}.{TableName} not found. Executing script {ScriptResourceName}...", schemaName, tableName, scriptResourceName);

        // Read the embedded resource
        var assembly = Assembly.GetExecutingAssembly();
        string sqlScript;
        await using (var stream = assembly.GetManifestResourceStream(scriptResourceName))
        {
            if (stream == null)
            {
                Logger.LogError("Embedded resource {ScriptResourceName} not found.", scriptResourceName);
                throw new FileNotFoundException($"Embedded resource {scriptResourceName} not found.");
            }
            using var reader = new StreamReader(stream);
            sqlScript = await reader.ReadToEndAsync();
        }

        // Execute the script
        try
        {
            await using (var conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                await using (var cmd = new NpgsqlCommand(sqlScript, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            Logger.LogInformation("Script {ScriptResourceName} executed successfully for table {SchemaName}.{TableName}.", scriptResourceName, schemaName, tableName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Script {ScriptResourceName} failed for table {SchemaName}.{TableName}.", scriptResourceName, schemaName, tableName);
            throw;
        }
    }
}
