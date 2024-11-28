using System;
using System.Data.SqlClient;

namespace LibraryManagementSystem
{
    class Program
    {
        // Update the connection string with your server details
        private static string connectionString = "Server=(localdb)\\Local;Database=LibraryDB;Trusted_Connection=True;";

        static void Main(string[] args)
        {
            InitializeDatabase();

            while (true)
            {
                Console.WriteLine("\nLibrary Management System");
                Console.WriteLine("1. Add Book");
                Console.WriteLine("2. View Books");
                Console.WriteLine("3. Search Book");
                Console.WriteLine("4. Update Book");
                Console.WriteLine("5. Delete Book");
                Console.WriteLine("6. Exit");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddBook();
                        break;
                    case "2":
                        ViewBooks();
                        break;
                    case "3":
                        SearchBook();
                        break;
                    case "4":
                        UpdateBook();
                        break;
                    case "5":
                        DeleteBook();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option, try again.");
                        break;
                }
            }
        }

        static void InitializeDatabase()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string createDatabaseQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LibraryDB')
                    BEGIN
                        CREATE DATABASE LibraryDB;
                    END";
                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Books')
                    BEGIN
                        CREATE TABLE Books (
                            Id INT PRIMARY KEY IDENTITY(1,1),
                            Title NVARCHAR(255) NOT NULL,
                            Author NVARCHAR(255) NOT NULL,
                            ISBN NVARCHAR(50) NOT NULL UNIQUE,
                            PublishedYear INT
                        );
                    END";

                // Create database if not exists
                SqlCommand command = new SqlCommand(createDatabaseQuery, connection);
                command.ExecuteNonQuery();

                // Reconnect to the newly created database
                connection.ChangeDatabase("LibraryDB");

                // Create table if not exists
                command = new SqlCommand(createTableQuery, connection);
                command.ExecuteNonQuery();
            }
        }

        static void AddBook()
        {
            Console.Write("Enter Title: ");
            string title = Console.ReadLine();
            Console.Write("Enter Author: ");
            string author = Console.ReadLine();
            Console.Write("Enter ISBN: ");
            string isbn = Console.ReadLine();
            Console.Write("Enter Published Year: ");
            int year = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO Books (Title, Author, ISBN, PublishedYear) VALUES (@Title, @Author, @ISBN, @Year)";
                SqlCommand command = new SqlCommand(insertQuery, connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@ISBN", isbn);
                command.Parameters.AddWithValue("@Year", year);
                command.ExecuteNonQuery();
                Console.WriteLine("Book added successfully!");
            }
        }

        static void ViewBooks()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM Books";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("\nBooks in the Library:");
                Console.WriteLine("ID\tTitle\tAuthor\tISBN\tPublished Year");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]}\t{reader["Title"]}\t{reader["Author"]}\t{reader["ISBN"]}\t{reader["PublishedYear"]}");
                }
            }
        }

        static void SearchBook()
        {
            Console.Write("Enter Title or Author or ISBN to search: ");
            string keyword = Console.ReadLine();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string searchQuery = "SELECT * FROM Books WHERE Title LIKE @Keyword OR Author LIKE @Keyword OR ISBN LIKE @Keyword";
                SqlCommand command = new SqlCommand(searchQuery, connection);
                command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                SqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("\nSearch Results:");
                Console.WriteLine("ID\tTitle\tAuthor\tISBN\tPublished Year");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]}\t{reader["Title"]}\t{reader["Author"]}\t{reader["ISBN"]}\t{reader["PublishedYear"]}");
                }
            }
        }

        static void UpdateBook()
        {
            Console.Write("Enter Book ID to update: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Enter new Title: ");
            string title = Console.ReadLine();
            Console.Write("Enter new Author: ");
            string author = Console.ReadLine();
            Console.Write("Enter new ISBN: ");
            string isbn = Console.ReadLine();
            Console.Write("Enter new Published Year: ");
            int year = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE Books SET Title = @Title, Author = @Author, ISBN = @ISBN, PublishedYear = @Year WHERE Id = @Id";
                SqlCommand command = new SqlCommand(updateQuery, connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@ISBN", isbn);
                command.Parameters.AddWithValue("@Year", year);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
                Console.WriteLine("Book updated successfully!");
            }
        }

        static void DeleteBook()
        {
            Console.Write("Enter Book ID to delete: ");
            int id = int.Parse(Console.ReadLine());

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM Books WHERE Id = @Id";
                SqlCommand command = new SqlCommand(deleteQuery, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
                Console.WriteLine("Book deleted successfully!");
            }
        }
    }
}
