using System;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

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
    // Saves user account with private key after successful registration
    public void SaveAccount(string username, string privateKey)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT OR REPLACE INTO Account (Username, PrivateKey)
            VALUES ($username, $privateKey);
        ";
        command.Parameters.AddWithValue("$username", username);
        command.Parameters.AddWithValue("$privateKey", privateKey);

        command.ExecuteNonQuery();
    }

    // Retrieves private key for the logged-in user
    public string GetPrivateKey(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = "SELECT PrivateKey FROM Account WHERE Username = $username;";
        command.Parameters.AddWithValue("$username", username);

        var result = command.ExecuteScalar();
        return result?.ToString();
    }

    // Saves a new message to the local database
    public void SaveMessage(string sender, string receiver, string content, bool isMine)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = @"
            INSERT INTO Messages (Sender, Receiver, Content, IsMine)
            VALUES ($sender, $receiver, $content, $isMine);
        ";
        command.Parameters.AddWithValue("$sender", sender);
        command.Parameters.AddWithValue("$receiver", receiver);
        command.Parameters.AddWithValue("$content", content);
        command.Parameters.AddWithValue("$isMine", isMine ? 1 : 0);

        command.ExecuteNonQuery();
    }

    // Retrieves chat history with a specific user
    public List<(string Sender, string Receiver, string Content, DateTime Timestamp, bool IsMine)> GetMessageHistory(string contactUsername)
    {
        var messages = new List<(string, string, string, DateTime, bool)>();

        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        var command = connection.CreateCommand();

        command.CommandText = @"
            SELECT Sender, Receiver, Content, Timestamp, IsMine 
            FROM Messages 
            WHERE Sender = $contact OR Receiver = $contact
            ORDER BY Timestamp ASC;
        ";
        command.Parameters.AddWithValue("$contact", contactUsername);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            messages.Add((
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetDateTime(3),
                reader.GetInt32(4) == 1
            ));
        }

        return messages;
    }
}

