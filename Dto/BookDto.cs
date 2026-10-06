namespace StoreBooks.Dto;

public class BookDto
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int PriceInCents { get; set; }
    public int Stock { get; set; }
    public DateTime PublishedDate { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ISBN { get; set; } = string.Empty;
}
