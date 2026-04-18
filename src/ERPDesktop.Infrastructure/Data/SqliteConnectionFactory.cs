using System.Data;
using ERPDesktop.Application.Abstractions;
using ERPDesktop.Shared.Paths;
using Microsoft.Data.Sqlite;

namespace ERPDesktop.Infrastructure.Data;

public sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqliteConnectionFactory(string? databaseFilePath = null)
    {
        var path = databaseFilePath ?? AppPaths.DatabaseFilePath;
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();
    }

    public IDbConnection CreateOpenConnection()
    {
        var c = new SqliteConnection(_connectionString);
        c.Open();
        using (var cmd = c.CreateCommand())
        {
            cmd.CommandText = "PRAGMA foreign_keys = ON;";
            cmd.ExecuteNonQuery();
        }

        return c;
    }
}
