using LionFire.Data;
using LionFire.Data.Connections;
using System;
using System.Linq;

namespace LionFire.CouchDB;


public class CouchDBConnectionOptions : ConnectionOptions<CouchDBConnectionOptions>, IHasConnectionString, IHasConnectionUri
{

    //public string Key
    //{
    //    get => GetKey(showPassword: !InsecurePassword);
    //    set => throw new NotImplementedException();
    //}
    //protected string GetKey(bool showPassword)
    //    => $"couch://{Username}{(showPassword ? ":" + Password : "")}@{Host}{(Port == default ? "" : ":" + Port)}{(Database != null ? "/" + Database : "")}";
    //public bool InsecurePassword { get; set; }

    public string DatabaseUrl => $"{EffectiveScheme}://{Username}:{Password}@{Host}{(Port == default ? "" : ":" + Port)}{(Database != null ? "/" + Database : "")}";
    public string WebUrl => $"{EffectiveScheme}://{Username}:{Password}@{Host}{(Port == default ? "" : ":" + Port)}";

    #region Properties

    public string EffectiveScheme => Scheme ?? "https";
    public string Scheme { get; set; }

    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }

    #endregion

    #region Construction

    public CouchDBConnectionOptions(string connectionString) { ConnectionString = connectionString; }
    public CouchDBConnectionOptions(Uri connectionUri) { ConnectionUri = connectionUri; }
    public CouchDBConnectionOptions(string host, string username, string password, string database = null)
    {
        Scheme = IsLocalhost(host) ? "http" : "https";

        if (host.Contains(":"))
        {
            var split = host.Split(':');
            if (split.Length != 2) throw new ArgumentException($"If {nameof(host)} contains port, it must only contain one ':'");
            Host = split[0];
            Port = Int32.Parse(split[1]);
        }
        Username = username;
        Password = password;
        Database = database;
    }

    public CouchDBConnectionOptions(string host, int port, string username, string password, string database = null)
    {
        if (host.Contains(":"))
        {
            throw new ArgumentException($"If {nameof(port)} is specified, host must not contain ':'");
        }
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        Database = database;
    }
    #endregion

    #region Uri

    public Uri ConnectionUri
    {
        get => new Uri(ConnectionString);
        set
        {
            Scheme = value.Scheme;
            Host = value.Host;
            Port = value.Port;
            var userInfo = value.UserInfo;
            if (userInfo.Contains(":"))
            {
                var split = userInfo.Split(new char[] { ':' }, 2);
                Username = split[0];
                Password = split[1];
            }
            else
            {
                Username = userInfo;
            }
            Database = value.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
    }

    public string ConnectionString
    {
        get => $"{EffectiveScheme}://{Username}:{Password}@{Host}{(Port == default ? "" : ":" + Port)}{(Database != null ? "/" + Database : "")}";
        set => ConnectionUri = new Uri(value);
    }
    public string GetConnectionString(bool includePassword = false)
    {
        return $"{EffectiveScheme}://{Username}{(includePassword ? ":" + Password : "")}@{Host}{(Port == default ? "" : ":" + Port)}{(Database != null ? "/" + Database : "")}";
    }

    #endregion

    public static bool IsLocalhost(string host)
    {
        if (host == "localhost") return true;
        if (host == "127.0.0.1") return true;
        if (host == "::1") return true;
        if (host == "[::1]") return true;
        // TODO: better implementation of this
        return false;
    }

    public override string ToString() => $"{GetConnectionString(includePassword: false)}";

}
