namespace PatsyLibrary.Helpers;

public class BookHelper
{
    public static Dictionary<string, string> Generos() => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "Fiction", "Ficção" },
        { "Non-fiction", "Não ficção" },
        { "Mystery", "Mistério" },
        { "Thriller", "Suspense" },
        { "Horror", "Terror" },
        { "Romance", "Romance" },
        { "Science Fiction", "Ficção científica" },
        { "Fantasy", "Fantasia" },
        { "Biography", "Biografia" },
        { "Autobiography", "Autobiografia" },
        { "Self-help", "Autoajuda" },
        { "History", "História" },
        { "Poetry", "Poesia" },
        { "Drama", "Drama" },
        { "Adventure", "Aventura" },
        { "Children's", "Infantil" },
        { "Young Adult", "Jovem adulto" },
        { "Graphic Novel", "Romance gráfico" },
        { "Memoir", "Memórias" },
        { "Philosophy", "Filosofia" },
        { "Psychology", "Psicologia" },
        { "Religion", "Religião" },
        { "Science", "Ciência" },
        { "True Crime", "Crime real" },
        { "Cookbook", "Livro de receitas" },
        { "Art", "Arte" },
        { "Travel", "Viagem" },
        { "Education", "Educação" },
        { "Health", "Saúde" },
        { "Business", "Negócios" },
        { "Political Science", "Ciência política" },
        { "Humor", "Humor" }
    };

    public static string TranslateGenre(string genre)
    {
        if (string.IsNullOrWhiteSpace(genre))
            return "Gênero desconhecido";

        return Generos().TryGetValue(genre, out string translatedGenre)
            ? translatedGenre
            : genre; // Se não encontrar no dicionário, mantém o original
    }
}