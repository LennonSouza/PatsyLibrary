using Newtonsoft.Json.Linq;
using PatsyLibrary.Contracts.Services.Interfaces;
using PatsyLibrary.Models;

namespace PatsyLibrary.Contracts.Services;

public class BookService : IBookService
{
    public async Task<(Book Book, string Genre, string Publisher)> GoogleSearchBookISBNAsync(string isbn)
    {
        string apiKey = "AIzaSyCNDCh05W7RpAdaUZ3azYRCvtyNpcqQX8s";
        string url = $"https://www.googleapis.com/books/v1/volumes?q={isbn}&key={apiKey}";

        HttpClient httpClient = new();

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (null, null, null);

            string jsonResult = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(jsonResult);
            var volumeInfo = jsonObject["items"]?.FirstOrDefault()?["volumeInfo"];

            if (volumeInfo == null)
                return (null, null, null);

            var book = new Book(
                author: volumeInfo["authors"]?.FirstOrDefault()?.ToString() ?? "Autor desconhecido",
                tittle: volumeInfo["title"]?.ToString() ?? "Título desconhecido",
                publicationYear: volumeInfo["publishedDate"]?.ToString() ?? "Data desconhecida",
                sinopse: volumeInfo["description"]?.ToString() ?? "Sem sinopse disponível",
                isbn: isbn,
                language: volumeInfo["language"]?.ToString() ?? "Idioma desconhecido"
            );

            string genre = volumeInfo["categories"]?.FirstOrDefault()?.ToString() ?? null;
            string publisher = volumeInfo["publisher"]?.ToString() ?? null;

            return (book, genre, publisher);
        }
        catch (Exception)
        {
            return (null, null, null);
        }
    }

    public async Task<(Book Book, string Publisher)> OpenLibrarySearchBookISBNAsync(string isbn)
    {
        string url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&jscmd=data&format=json";

        HttpClient httpClient = new();

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (null, null);

            string jsonResult = await response.Content.ReadAsStringAsync();
            JObject jsonObject = JObject.Parse(jsonResult);

            var bookData = jsonObject[$"ISBN:{isbn}"];
            if (bookData == null)
                return (null, null);

            var book = new Book(
                author: bookData["authors"]?.FirstOrDefault()?["name"]?.ToString() ?? "Autor desconhecido",
                tittle: bookData["title"]?.ToString() ?? "Título desconhecido",
                sinopse: bookData["notes"]?.ToString() ?? "Sem sinopse disponível",
                isbn: isbn
            );

            string publisher = bookData["publishers"]?.FirstOrDefault()?["name"]?.ToString() ?? null;

            return (book, publisher);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao buscar livro na OpenLibrary API: {ex.Message}");
            return (null, null);
        }
    }
}