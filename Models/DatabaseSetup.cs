using Npgsql;

namespace StoreBooks.Models;

public class DatabaseSetup(IConfiguration configuration)
{
    public void InitializeDatabase()
    {
        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        // Convert Book.cs properties to SQL table creation statement
        const string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Books (
                Id UUID PRIMARY KEY,
                Title TEXT NOT NULL,
                Author TEXT NOT NULL,
                PriceInCents INT NOT NULL,
                Stock INT NOT NULL,
                PublishedDate TIMESTAMP NOT NULL,
                Genre TEXT NOT NULL,
                Description TEXT,
                ISBN TEXT NOT NULL,
                CreatedAt TIMESTAMP NOT NULL,
                UpdatedAt TIMESTAMP NOT NULL
            );";
        using var command = new NpgsqlCommand(createTableQuery, connection);
        command.ExecuteNonQuery();

        // Code to initialize the database goes here
        Console.WriteLine("Database initialized.");
    }

    public void PurgeDatabase()
    {
        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        // Drop everything in the database (currently only the Books table)
        const string dropTableQuery = "DROP TABLE IF EXISTS Books;";
        using var command = new NpgsqlCommand(dropTableQuery, connection);
        command.ExecuteNonQuery();

        Console.WriteLine("Database purged.");
    }
}