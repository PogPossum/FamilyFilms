using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FamilyFilmsMgmtApp
{
    internal class Program
    {
        static string connString = @"Server=x.x.x.x,1433;Database=MovieDB;User Id=User;Password=Password1;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            bool running = true;

            while (running)
            {
                Console.Clear();
                Console.Write("\x1b[3J\x1b[H\x1b[2J");
                Console.Clear();
                Console.WriteLine("                FamilyFilms Management App");
                Console.WriteLine(" ");
                Console.WriteLine($"                  ---  MENU ---");
                Console.WriteLine("======================================================");
                Console.WriteLine($"SYSTEM TIME: {DateTime.Now:dd/MM/yyyy HH:mm} | STATUS: OPERATIONAL");
                Console.WriteLine("======================================================");
                Console.WriteLine(" [1] Show All Movies          [4] Add Movies in bulk");
                Console.WriteLine(" [2] Add Movies               [5] Delete Movies");
                Console.WriteLine(" [3] Movie Search");
                Console.WriteLine(" ");
                Console.WriteLine(" [0] Exit App");
                Console.WriteLine("======================================================");

                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowMovies();
                        break;
                    case "2":
                        AddNewMovie();
                        break;
                    case "3":
                        MovieSearch();
                        break;
                    case "4":
                        AddMoviesBulk();
                        break;
                    case "5":
                        DeleteMovies();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again..");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ShowMovies()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("              --- Show All Movies ---");
            Console.WriteLine("===========================================================================================================");
            Console.WriteLine(" Title                                             | Year | Location  | Category | Studio");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"SELECT Title, Release, Category, Location, Studio 
                                   FROM [FamilyFilms].[dbo].[Movies] 
                                   ORDER BY Title ASC;";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader["Title"].ToString().Trim();
                            string release = reader["Release"].ToString();
                            string location = reader["Location"].ToString().Trim();
                            string category = reader["Category"].ToString().Trim();
                            string studio = reader["Studio"].ToString().Trim();

                            Console.WriteLine($"{title.PadRight(50)} | {release} | {location.PadRight(10)} | {category.PadRight(15)}  | {studio}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("===========================================================================================================");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void AddNewMovie()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("                --- Add New Movie ---");
            Console.WriteLine("======================================================");
            Console.WriteLine(" Enter the movie details below:");
            Console.WriteLine("------------------------------------------------------");

            Console.Write("Title: ");
            string title = Console.ReadLine()?.Trim();

            Console.Write("Release Year (e.g., 1994): ");
            if (!int.TryParse(Console.ReadLine(), out int release))
            {
                Console.WriteLine("Invalid release year!");
                Console.ReadKey();
                return;
            }

            Console.Write("Category (e.g., Animated, Live-Action): ");
            string category = Console.ReadLine()?.Trim();

            Console.Write("Location: ");
            string location = Console.ReadLine()?.Trim();

            Console.Write("Studio: ");
            string studio = Console.ReadLine()?.Trim();

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string insertSql = @"INSERT INTO [FamilyFilms].[dbo].[Movies] (Title, Release, Location, Category, Studio) 
                                         VALUES (@Title, @Release, @Location, @Category, @Studio);";

                    using (SqlCommand cmd = new SqlCommand(insertSql, connection))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Release", release);
                        cmd.Parameters.AddWithValue("@Location", location);
                        cmd.Parameters.AddWithValue("@Category", category);
                        cmd.Parameters.AddWithValue("@Studio", studio);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("\nMovie added successfully!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDatabase Error: " + ex.Message);
                }
            }

            Console.WriteLine(" ");
            Console.WriteLine("======================================================");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void MovieSearch()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("                --- Movie Search ---");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine(" search Movies with only a part of the name needed");
            Console.WriteLine(" Example: 'Lion' for The Lion King");
            Console.WriteLine("----------------------------------------------------");

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    string sql = @"
                        SELECT Title, Release, Category, Location, Studio 
                        FROM [FamilyFilms].[dbo].[Movies]
                        where Title like '%' + @search + '%'
                        order by Release asc";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        Console.Write("Enter movie title to search: ");
                        string searchTerm = Console.ReadLine();
                        cmd.Parameters.AddWithValue("@search", searchTerm);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string title = reader["Title"].ToString().Trim();
                                int release = int.Parse(reader["Release"].ToString().Trim());
                                string location = reader["Location"].ToString().Trim();
                                string category = reader["Category"].ToString().Trim();
                                string studio = reader["Studio"].ToString().Trim();

                                Console.WriteLine($"{title.PadRight(50)} | {release} | {location.PadRight(10)} | {category.PadRight(15)}  | {studio}");
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
            }
            Console.WriteLine(" ");
            Console.WriteLine("============ ============ ============ ============");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void AddMoviesBulk()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.WriteLine("               --- Bulk Add New Movies ---");
            Console.WriteLine("=================================================================");
            Console.WriteLine(" --- Paste SQL-Style Values (Multiple Lines) ---");
            Console.WriteLine(" Format: ('Title', Release, 'Location', 'Category', 'Studio'),");
            Console.WriteLine(" Example: ('Bambi', 1942, 'Shelf A', 'Animated', 'Disney'),");
            Console.WriteLine(" Add movies and press ENTER on a blank line to finish:");
            Console.WriteLine("-----------------------------------------------------------------");

            StringBuilder sb = new StringBuilder();
            string line;

            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                sb.AppendLine(line);
            }

            string input = sb.ToString();

            // Regex matches: ('Title', Release, 'Location', 'Category', 'Studio')
            string pattern = @"\(\s*'(?<Title>.+?)'\s*,\s*(?<Release>\d+)\s*,\s*'(?<Location>.+?)'\s*,\s*'(?<Category>.+?)'\s*,\s*'(?<Studio>.+?)'\s*\)";
            MatchCollection matches = Regex.Matches(input, pattern, RegexOptions.Singleline);

            if (matches.Count == 0)
            {
                Console.WriteLine("\nNo valid entries found. Check your formatting!");
                FinishBulkPrompt();
                return;
            }

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();
                    int successCount = 0;

                    foreach (Match match in matches)
                    {
                        string title = match.Groups["Title"].Value.Trim();
                        int release = int.Parse(match.Groups["Release"].Value.Trim());
                        string location = match.Groups["Location"].Value.Trim();
                        string category = match.Groups["Category"].Value.Trim();
                        string studio = match.Groups["Studio"].Value.Trim();

                        string insertSql = @"INSERT INTO [FamilyFilms].[dbo].[Movies] (Title, Release, Location, Category, Studio) 
                                             VALUES (@Title, @Release, @Location, @Category, @Studio)";

                        using (SqlCommand insertCmd = new SqlCommand(insertSql, connection))
                        {
                            insertCmd.Parameters.AddWithValue("@Title", title);
                            insertCmd.Parameters.AddWithValue("@Release", release);
                            insertCmd.Parameters.AddWithValue("@Location", location);
                            insertCmd.Parameters.AddWithValue("@Category", category);
                            insertCmd.Parameters.AddWithValue("@Studio", studio);

                            insertCmd.ExecuteNonQuery();
                            successCount++;
                        }
                    }

                    Console.WriteLine($"\nProcessing Complete!");
                    Console.WriteLine($"Successfully added: {successCount} movies.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nDatabase Error: " + ex.Message);
                }
            }

            FinishBulkPrompt();
        }

        static void FinishBulkPrompt()
        {
            Console.WriteLine(" ");
            Console.WriteLine("======================================================");
            Console.WriteLine("Press any key to go back...");
            Console.ReadKey();
        }

        static void DeleteMovies()
        {
            Console.Clear();
            Console.Write("\x1b[3J\x1b[H\x1b[2J");
            Console.Clear();
            Console.WriteLine("           --- Delete Movies by Title ---");
            Console.WriteLine("======================================================");
            Console.WriteLine(" Enter titles separated by commas or new lines.");
            Console.WriteLine(" Example: 'Narnia', 'Bambi'");
            Console.WriteLine(" Press ENTER on a blank line to finish:");
            Console.WriteLine("------------------------------------------------------");

            StringBuilder sb = new StringBuilder();
            string line;
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                sb.AppendLine(line);
            }

            var titlesToDelete = sb.ToString()
                .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim().Trim('\''))
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            if (titlesToDelete.Count == 0) return;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                try
                {
                    connection.Open();

                    // --- STEP 1: PREVIEW MATCHES ---
                    Console.WriteLine("\nReviewing database for matches...");
                    List<string> foundMovies = new List<string>();

                    foreach (var title in titlesToDelete)
                    {
                        string checkSql = "SELECT Title, Release FROM [FamilyFilms].[dbo].[Movies] WHERE RTRIM(Title) = @T";
                        using (SqlCommand checkCmd = new SqlCommand(checkSql, connection))
                        {
                            checkCmd.Parameters.AddWithValue("@T", title);
                            using (SqlDataReader reader = checkCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    foundMovies.Add($"{reader["Title"].ToString().Trim()} ({reader["Release"]})");
                                }
                            }
                        }
                    }

                    if (foundMovies.Count == 0)
                    {
                        Console.WriteLine("No matching movies found in the database.");
                        FinishBulkPrompt();
                        return;
                    }

                    Console.WriteLine("\nTHE FOLLOWING ENTRIES WILL BE DELETED:");
                    foreach (var movie in foundMovies)
                    {
                        Console.WriteLine($"- {movie}");
                    }

                    // --- STEP 2: CONFIRMATION ---
                    Console.Write("\nAre you absolutely sure? (Type 'YES' to confirm): ");
                    string confirm = Console.ReadLine()?.ToUpper();

                    if (confirm == "YES")
                    {
                        int totalDeleted = 0;
                        foreach (var title in titlesToDelete)
                        {
                            string deleteSql = "DELETE FROM [FamilyFilms].[dbo].[Movies] WHERE RTRIM(Title) = @T";
                            using (SqlCommand delCmd = new SqlCommand(deleteSql, connection))
                            {
                                delCmd.Parameters.AddWithValue("@T", title);
                                totalDeleted += delCmd.ExecuteNonQuery();
                            }
                        }
                        Console.WriteLine($"\nSuccess! {totalDeleted} Movie(s) removed.");
                    }
                    else
                    {
                        Console.WriteLine("\nOperation cancelled. No data was harmed.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            FinishBulkPrompt();
        }
    }
}
