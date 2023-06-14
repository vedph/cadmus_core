using Fusi.DbManager.MySql;
using Fusi.DbManager;
using System;

namespace Cadmus.Index.Ef.MySql.Test;

/// <summary>
/// Fixture class used to drop the test database before tests are run.
/// This is required as far as we have different schemas for the same database
/// to keep compatibility with legacy MySql tests, which assume differently
/// named fields.
/// </summary>
public class MySqlFixture : IDisposable
{
    private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql;";
    private const string DB_NAME = "cadmus-index-test";
    static private readonly string CS = string.Format(CST, DB_NAME);

    private bool _disposed;

    public void DropDatabase()
    {
        IDbManager dbManager = new MySqlDbManager(CS);
        dbManager.RemoveDatabase(CS);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed) _disposed = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
