using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace InventoryManagementSystem
{

    // b. Marker Interface for Inventory Entity
    public interface IInventoryEntity
    {
        int Id { get; }
    }



    // a. Immutable Inventory Record

    public record InventoryItem(
        int Id,
        string Name,
        int Quantity,
        DateTime DateAdded
    ) : IInventoryEntity;



    // c. Generic Inventory Logger

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log;
        private string _filePath;

        // Constructor
        public InventoryLogger(string filePath)
        {
            _log = new List<T>();
            _filePath = filePath;
        }


        public void Add(T item)
        {
            _log.Add(item);
        }



        public List<T> GetAll()
        {
            return new List<T>(_log);
        }



        // Save inventory data to file

        public void SaveToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    _log,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );

                // Use StreamWriter to write data
                using (StreamWriter writer = new StreamWriter(_filePath))
                {
                    writer.Write(json);
                }

                Console.WriteLine("Inventory data saved successfully.");
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "Error: You do not have permission to access the file."
                );
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine(
                    "Error: The directory was not found."
                );
            }
            catch (IOException ex)
            {
                Console.WriteLine(
                    "File error: " + ex.Message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unexpected error: " + ex.Message
                );
            }
        }

        public void LoadFromFile()
        {
            try
            {
                // Check whether the file exists
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine(
                        "File does not exist. Nothing to load."
                    );

                    return;
                }

                string json;

                // Use StreamReader to read data
                using (StreamReader reader = new StreamReader(_filePath))
                {
                    json = reader.ReadToEnd();
                }

                // Convert JSON back to List<T>
                List<T>? loadedItems =
                    JsonSerializer.Deserialize<List<T>>(json);

                if (loadedItems != null)
                {
                    _log = loadedItems;
                }

                Console.WriteLine(
                    "Inventory data loaded successfully."
                );
            }
            catch (JsonException)
            {
                Console.WriteLine(
                    "Error: The file contains invalid JSON data."
                );
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine(
                    "Error: You do not have permission to access the file."
                );
            }
            catch (IOException ex)
            {
                Console.WriteLine(
                    "File error: " + ex.Message
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Unexpected error: " + ex.Message
                );
            }
        }
    }



    // f. Integration Layer - InventoryApp

    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;


        public InventoryApp(string filePath)
        {
            _logger = new InventoryLogger<InventoryItem>(filePath);
        }




        public void SeedSampleData()
        {
            _logger.Add(
                new InventoryItem(
                    1,
                    "Laptop",
                    10,
                    DateTime.Now
                )
            );

            _logger.Add(
                new InventoryItem(
                    2,
                    "Keyboard",
                    25,
                    DateTime.Now
                )
            );

            _logger.Add(
                new InventoryItem(
                    3,
                    "Mouse",
                    30,
                    DateTime.Now
                )
            );

            _logger.Add(
                new InventoryItem(
                    4,
                    "Monitor",
                    15,
                    DateTime.Now
                )
            );

            _logger.Add(
                new InventoryItem(
                    5,
                    "Printer",
                    8,
                    DateTime.Now
                )
            );

            Console.WriteLine(
                "Sample inventory data added successfully."
            );
        }


        public void SaveData()
        {
            _logger.SaveToFile();
        }

        public void LoadData()
        {
            _logger.LoadFromFile();
        }



        public void PrintAllItems()
        {
            Console.WriteLine("\n===== INVENTORY ITEMS =====");

            List<InventoryItem> items = _logger.GetAll();

            if (items.Count == 0)
            {
                Console.WriteLine("No inventory items found.");
                return;
            }

            foreach (InventoryItem item in items)
            {
                Console.WriteLine($"ID: {item.Id}");
                Console.WriteLine($"Name: {item.Name}");
                Console.WriteLine($"Quantity: {item.Quantity}");
                Console.WriteLine(
                    $"Date Added: {item.DateAdded}"
                );

                Console.WriteLine("----------------------------");
            }
        }
    }

    // g. Main Application

    public class Program
    {
        public static void Main(string[] args)
        {
            string filePath = "inventory.json";


            Console.WriteLine("===== FIRST SESSION =====");

            InventoryApp app =
                new InventoryApp(filePath);

            app.SeedSampleData();

            app.SaveData();



            // SIMULATE CLEARING MEMORY
            Console.WriteLine(
                "\nClearing memory and starting a new session..."
            );

            Console.WriteLine("\n===== NEW SESSION =====");

            InventoryApp newApp =
                new InventoryApp(filePath);

            newApp.LoadData();


            // Print recovered data
            newApp.PrintAllItems();

            Console.WriteLine(
                "\nPress any key to exit..."
            );

            Console.ReadKey();
        }
    }
}