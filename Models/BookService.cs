using Npgsql;
using StoreBooks.Dto;
namespace StoreBooks.Models;

public class BookService(IConfiguration configuration)
{
    // Implementation for book-related operations goes here
    public bool AddBook(BookDto book)
    {
        Book newBook = new Book
        {
            Id = Guid.NewGuid(),
            Title = book.Title,
            Author = book.Author,
            PriceInCents = book.PriceInCents,
            Stock = book.Stock,
            PublishedDate = book.PublishedDate,
            Genre = book.Genre,
            Description = book.Description ?? string.Empty,
            ISBN = book.ISBN,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Validate the new book object before adding it to the database
        var (isValid, errorMessage) = ValidateBook(newBook);
        if (!isValid)
        {
            throw new ArgumentException(errorMessage, nameof(book));
        }

        // Code to add a book to the database
        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "INSERT INTO books (Id, Title, Author, PriceInCents, Stock, PublishedDate, Genre, Description, ISBN, CreatedAt, UpdatedAt) " +
            "VALUES (@Id, @Title, @Author, @PriceInCents, @Stock, @PublishedDate, @Genre, @Description, @ISBN, @CreatedAt, @UpdatedAt)", connection);
        command.Parameters.AddWithValue("Id", newBook.Id);
        command.Parameters.AddWithValue("Title", newBook.Title);
        command.Parameters.AddWithValue("Author", newBook.Author);
        command.Parameters.AddWithValue("PriceInCents", newBook.PriceInCents);
        command.Parameters.AddWithValue("Stock", newBook.Stock);
        command.Parameters.AddWithValue("PublishedDate", newBook.PublishedDate);
        command.Parameters.AddWithValue("Genre", newBook.Genre);
        command.Parameters.AddWithValue("Description", newBook.Description ?? string.Empty);
        command.Parameters.AddWithValue("ISBN", newBook.ISBN);
        command.Parameters.AddWithValue("CreatedAt", newBook.CreatedAt);
        command.Parameters.AddWithValue("UpdatedAt", newBook.UpdatedAt);

        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;

    }

    public bool UpdateBook(Guid bookId, BookDto book)
    {
        // get other values
        var existingBook = GetBookById(bookId)
            ?? throw new ArgumentException("Book not found.", nameof(bookId));

        // Create an updated book object
        var updatedBook = new Book
        {
            Id = bookId,
            Title = book.Title,
            Author = book.Author,
            PriceInCents = book.PriceInCents,
            Stock = book.Stock,
            PublishedDate = book.PublishedDate,
            Genre = book.Genre,
            Description = book.Description ?? string.Empty,
            ISBN = book.ISBN,
            CreatedAt = existingBook.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var (isValid, errorMessage) = ValidateBook(updatedBook);
        if (!isValid)
        {
            throw new ArgumentException(errorMessage, nameof(book));
        }

        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "UPDATE books SET Title = @Title, Author = @Author, PriceInCents = @PriceInCents, " +
            "Stock = @Stock, PublishedDate = @PublishedDate, Genre = @Genre, Description = @Description, " +
            "ISBN = @ISBN, UpdatedAt = @UpdatedAt WHERE Id = @Id",
            connection);
        command.Parameters.AddWithValue("Id", bookId);
        command.Parameters.AddWithValue("Title", updatedBook.Title);
        command.Parameters.AddWithValue("Author", updatedBook.Author);
        command.Parameters.AddWithValue("PriceInCents", updatedBook.PriceInCents);
        command.Parameters.AddWithValue("Stock", updatedBook.Stock);
        command.Parameters.AddWithValue("PublishedDate", updatedBook.PublishedDate);
        command.Parameters.AddWithValue("Genre", updatedBook.Genre);
        command.Parameters.AddWithValue("Description", updatedBook.Description);
        command.Parameters.AddWithValue("ISBN", updatedBook.ISBN);
        command.Parameters.AddWithValue("UpdatedAt", updatedBook.UpdatedAt);

        return command.ExecuteNonQuery() > 0;
    }

    public bool RemoveBook(Guid bookId)
    {
        try
        {
            string connectionString = configuration.GetConnectionString("database")
                ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");

            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(
                "DELETE FROM books WHERE Id = @Id",
                connection);
            command.Parameters.AddWithValue("Id", bookId);

            command.ExecuteNonQuery();

            return true;
        }
        catch
        {
            throw new Exception("Failed to remove book with Id: " + bookId);
        }
    }

    public Book? GetBookById(Guid bookId)
    {
        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "SELECT * FROM books WHERE Id = @Id",
            connection);
        command.Parameters.AddWithValue("Id", bookId);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Book
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Author = reader.GetString(reader.GetOrdinal("Author")),
                PriceInCents = reader.GetInt32(reader.GetOrdinal("PriceInCents")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                PublishedDate = reader.GetDateTime(reader.GetOrdinal("PublishedDate")),
                Genre = reader.GetString(reader.GetOrdinal("Genre")),
                Description = reader.GetString(reader.GetOrdinal("Description")) ?? string.Empty,
                ISBN = reader.GetString(reader.GetOrdinal("ISBN")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
        return null;
    }

    public Book? GetBookByISBN(string isbn)
    {
        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");

        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand(
            "SELECT * FROM books WHERE ISBN = @ISBN",
            connection);
        command.Parameters.AddWithValue("ISBN", isbn);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Book
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Author = reader.GetString(reader.GetOrdinal("Author")),
                PriceInCents = reader.GetInt32(reader.GetOrdinal("PriceInCents")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                PublishedDate = reader.GetDateTime(reader.GetOrdinal("PublishedDate")),
                Genre = reader.GetString(reader.GetOrdinal("Genre")),
                Description = reader.GetString(reader.GetOrdinal("Description")) ?? string.Empty,
                ISBN = reader.GetString(reader.GetOrdinal("ISBN")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
        return null;
    }

    public IEnumerable<Book> GetAllBooks()
    {
        // Code to retrieve all books from the database goes here
        string connectionString = configuration.GetConnectionString("database")
            ?? throw new InvalidOperationException("Connection string 'database' not found in appsettings.");
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        using var command = new NpgsqlCommand("SELECT * FROM books", connection);
        using var reader = command.ExecuteReader();
        var books = new List<Book>();
        while (reader.Read())
        {
            var book = new Book
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Author = reader.GetString(reader.GetOrdinal("Author")),
                PriceInCents = reader.GetInt32(reader.GetOrdinal("PriceInCents")),
                Stock = reader.GetInt32(reader.GetOrdinal("Stock")),
                PublishedDate = reader.GetDateTime(reader.GetOrdinal("PublishedDate")),
                Genre = reader.GetString(reader.GetOrdinal("Genre")),
                Description = reader.GetString(reader.GetOrdinal("Description")) ?? string.Empty,
                ISBN = reader.GetString(reader.GetOrdinal("ISBN")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
            books.Add(book);
        }
        return books;

    }

    // Validate incoming book data before performing database operations
    private (bool IsValid, string ErrorMessage) ValidateBook(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Title))
        {
            return (IsValid: false, ErrorMessage: "Book title cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(book.Author))
        {
            return (false, "Book author cannot be empty.");
        }
        if (book.PriceInCents < 0)
        {
            return (false, "Book price cannot be negative.");
        }
        if (book.Stock < 0)
        {
            return (false, "Book stock cannot be negative.");
        }
        if (string.IsNullOrWhiteSpace(book.ISBN))
        {
            return (false, "Book ISBN cannot be empty.");
        }
        var bookWithISBN = GetBookByISBN(book.ISBN);
        if (bookWithISBN != null && bookWithISBN.Id != book.Id)
        {
            return (false, "Book with the same ISBN already exists.");
        }
        return (true, string.Empty);
    }
}