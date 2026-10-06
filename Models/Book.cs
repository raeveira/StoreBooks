using static System.Runtime.InteropServices.JavaScript.JSType;
namespace StoreBooks.Models;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required int PriceInCents { get; set; }
    public required int Stock { get; set; }
    public required DateTime PublishedDate { get; set; }
    public required string Genre { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string ISBN { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime UpdatedAt { get; set; }
}