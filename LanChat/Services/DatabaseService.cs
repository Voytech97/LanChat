using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace LanChat.Services;
public class DatabaseService
{
    private readonly string _connectionString;
    public DatabaseService()
    {
        // creates folder on users disk in appdata/local/lanchat
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(folder, "LanChat");
        Directory.CreateDirectory(appFolder);

        var dbPath = Path.Combine(appFolder, "lanchat_client.db");
        _connectionString = $"Data Source={dbPath}";

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        //Account table: holds your login and private key
        //Message table: holds your messages history
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Account (
                Username TEXT PRIMARY KEY,
                PrivateKey TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Messages (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Sender TEXT NOT NULL,
                Receiver TEXT NOT NULL,
                Content TEXT NOT NULL,
                Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
                IsMine INTEGER NOT NULL
            );
        ";
        command.ExecuteNonQuery();

    }
}

