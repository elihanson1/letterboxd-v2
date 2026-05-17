// Movie rating app based on the actual app Letterboxd.
// V3: Now consumes a movies.json file on startup, iterates over the array
// of saved movie objects, and displays each one before entering the menu.

using System.Text.Json;

namespace Letterboxd
{
    class Movie
    {
        public string Title { get; set; } = "";
        public int Year { get; set; }
        public string Genre { get; set; } = "";
        public int Rating { get; set; }
        public string Review { get; set; } = "";

        public Movie() { }

        public Movie(string title, int year, string genre, int rating, string review)
        {
            Title = title;
            Year = year;
            Genre = genre;
            Rating = rating;
            Review = review;
        }

        public string GetStars()
        {
            int filled = (int)Math.Round(Rating / 2.0);
            int empty = 5 - filled;
            return new string('★', filled) + new string('☆', empty);
        }

        public void Display()
        {
            Console.WriteLine("\n======================================");
            Console.WriteLine($"  {Truncate(Title, 34)}");
            if (Year > 0)
                Console.WriteLine($"  {Year}");
            if (!string.IsNullOrWhiteSpace(Genre))
                Console.WriteLine($"  {Genre}");
            Console.WriteLine($"  {GetStars()}  ({Rating}/10)");
            if (!string.IsNullOrWhiteSpace(Review))
                Console.WriteLine($"  \"{Truncate(Review, 100)}\"");
            Console.WriteLine("======================================\n");
        }

        private static string Truncate(string s, int max) =>
            s.Length <= max ? s : s[..(max - 1)] + "…";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("               Letterboxd               ");
            Console.WriteLine("========================================\n");
            LoadSavedMovies();

            while (true)
            {
                Console.WriteLine(" 1 - Rate a movie");
                Console.WriteLine(" 0 - Quit\n");

                Console.Write("Choose (0-1): ");
                string? input = Console.ReadLine();

                Console.WriteLine();

                switch (input?.Trim())
                {
                    case "1": RateMovie(); break;
                    case "0": return;
                    default:
                        Console.WriteLine("Enter 0 or 1.\n");
                        break;
                }
            }
        }
        static void LoadSavedMovies()
        {
            string path = "movies.json";

            if (!File.Exists(path))
            {
                Console.WriteLine("No saved movies found.\n");
                return;
            }

            string json = File.ReadAllText(path);

            List<Movie>? movies = JsonSerializer.Deserialize<List<Movie>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (movies == null || movies.Count == 0)
            {
                Console.WriteLine("No saved movies found.\n");
                return;
            }

            Console.WriteLine($"--- Your Saved Movies ({movies.Count}) ---");

            foreach (Movie movie in movies)
            {
                movie.Display();
            }
        }

        static void RateMovie()
        {
            string title = Prompt("Movie title");
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Title can't be blank. Back to menu.\n");
                return;
            }

            int year = PromptYear();
            string genre = PromptGenre();
            int? rating = PromptRating();
            if (rating is null) return;

            Console.Write("Review (or Enter to skip): ");
            string review = Console.ReadLine()?.Trim() ?? "";

            Movie movie = new Movie(title, year, genre, rating.Value, review);
            movie.Display();
        }

        static string Prompt(string label)
        {
            Console.Write($"{label}: ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        static int PromptYear()
        {
            Console.Write("Year (or Enter to skip): ");
            string input = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(input)) return 0;
            if (int.TryParse(input, out int y) && y >= 1890 && y <= DateTime.Now.Year + 2)
                return y;
            Console.WriteLine("Invalid year — skipping.");
            return 0;
        }

        static string PromptGenre()
        {
            Console.Write("Genre (or Enter to skip): ");
            return Console.ReadLine()?.Trim() ?? "";
        }

        static int? PromptRating()
        {
            Console.Write("Rating (1-10): ");
            string input = Console.ReadLine()?.Trim() ?? "";

            if (int.TryParse(input, out int r) && r >= 1 && r <= 10)
                return r;

            if (string.IsNullOrWhiteSpace(input))
                Console.WriteLine("No rating entered. Back to menu.\n");
            else
                Console.WriteLine("Invalid input. Enter a whole number from 1 to 10. Back to menu.\n");

            return null;
        }
    }
}
