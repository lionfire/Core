using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace LionFire.Life.Model;

public class TimeTrackingDbDefaults
{
    public static string Dir => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public const string Filename = "TimeTracker.sqlite3.db";
    public static string Path => System.IO.Path.Join(Dir, "LionFire", "TimeTracker", TimeTrackingDbDefaults.Filename);

}

public class TimeTrackingContext : DbContext
{
    public DbSet<JournalEntry>? JournalEntries { get; set; }
    public DbSet<JournalTag>? Tags { get; set; }
    public DbSet<AccountingCode>? AccountingCodes { get; set; }

    public string DbPath { get; }

    public string ConnectionString => Configuration.GetConnectionString("sqlite") ?? $"Data Source={DbPath ?? TimeTrackingDbDefaults.Path}";

    public IConfiguration Configuration { get; }

    public TimeTrackingContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public TimeTrackingContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(ConnectionString);
}


public class TimeTrackingContextFactory : IDesignTimeDbContextFactory<TimeTrackingContext>
{
    public TimeTrackingContext CreateDbContext(string[] args)
    {
        var dir = Path.GetDirectoryName(TimeTrackingDbDefaults.Path);

        if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
        var optionsBuilder = new DbContextOptionsBuilder<TimeTrackingContext>();
        optionsBuilder.UseSqlite($"Data Source={TimeTrackingDbDefaults.Path}");

        return new TimeTrackingContext(optionsBuilder.Options);
    }
}