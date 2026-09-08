using System;
using System.Collections.Generic;


// 1. MARKER / INVENTORY INTERFACE
// ======================================================

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}


// 2. ELECTRONIC ITEM
// ======================================================

public class ElectronicItem : IInventoryItem
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; set; }
    public string Brand { get; private set; }
    public int WarrantyMonths { get; private set; }

    public ElectronicItem(
        int id,
        string name,
        int quantity,
        string brand,
        int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Quantity: {Quantity}, " +
               $"Brand: {Brand}, Warranty: {WarrantyMonths} months";
    }
}


// 3. GROCERY ITEM
// ======================================================

public class GroceryItem : IInventoryItem
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; private set; }

    public GroceryItem(
        int id,
        string name,
        int quantity,
        DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Quantity: {Quantity}, " +
               $"Expiry Date: {ExpiryDate:yyyy-MM-dd}";
    }
}


// 4. CUSTOM EXCEPTIONS
// ======================================================

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message)
        : base(message)
    {
    }
}


public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message)
        : base(message)
    {
    }
}


public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message)
        : base(message)
    {
    }
}


// 5. GENERIC INVENTORY REPOSITORY
// ======================================================

public class InventoryRepository<T> where T : IInventoryItem
{
    // Dictionary uses the item ID as the key
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    // Add Item
    // --------------------------------------------------

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException(
                $"An item with ID {item.Id} already exists.");
        }

        if (item.Quantity < 0)
        {
            throw new InvalidQuantityException(
                $"Quantity for '{item.Name}' cannot be negative.");
        }

        _items.Add(item.Id, item);
    }

    // Get Item By ID
    // --------------------------------------------------

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException(
                $"Item with ID {id} was not found.");
        }

        return _items[id];
    }


    // Remove Item
    // --------------------------------------------------

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException(
                $"Cannot remove item. ID {id} was not found.");
        }

        _items.Remove(id);
    }

    // Get All Items
    // --------------------------------------------------

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }


    // Update Quantity
    // --------------------------------------------------

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException(
                "Quantity cannot be negative.");
        }

        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException(
                $"Item with ID {id} was not found.");
        }

        _items[id].Quantity = newQuantity;
    }
}

// 6. WAREHOUSE MANAGER


public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronic;
    private InventoryRepository<GroceryItem> _groceries;



    public WareHouseManager()
    {
        _electronic = new InventoryRepository<ElectronicItem>();
        _groceries = new InventoryRepository<GroceryItem>();
    }


    public void SeedData()
    {
        try
        {
            _electronic.AddItem(
                new ElectronicItem(
                    101,
                    "Laptop",
                    10,
                    "Dell",
                    24
                ));

            _electronic.AddItem(
                new ElectronicItem(
                    102,
                    "Smartphone",
                    20,
                    "Samsung",
                    12
                ));

            _electronic.AddItem(
                new ElectronicItem(
                    103,
                    "Headphones",
                    15,
                    "Sony",
                    6
                ));


            _groceries.AddItem(
                new GroceryItem(
                    201,
                    "Milk",
                    30,
                    new DateTime(2026, 10, 15)
                ));

            _groceries.AddItem(
                new GroceryItem(
                    202,
                    "Bread",
                    25,
                    new DateTime(2026, 9, 20)
                ));

            _groceries.AddItem(
                new GroceryItem(
                    203,
                    "Rice",
                    50,
                    new DateTime(2027, 5, 10)
                ));

            Console.WriteLine("Seed data added successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while adding seed data: {ex.Message}");
        }
    }





    public void PrintAllItems<T>(InventoryRepository<T> repo)
        where T : IInventoryItem
    {
        List<T> items = repo.GetAllItems();

        foreach (T item in items)
        {
            Console.WriteLine(item);
        }
    }



    public void IncreaseStock<T>(
        InventoryRepository<T> repo,
        int id,
        int quantity)
        where T : IInventoryItem
    {
        try
        {
            T item = repo.GetItemById(id);

            if (quantity < 0)
            {
                throw new InvalidQuantityException(
                    "Increase quantity cannot be negative.");
            }

            int newQuantity = item.Quantity + quantity;

            repo.UpdateQuantity(id, newQuantity);

            Console.WriteLine(
                $"Stock increased successfully. " +
                $"New quantity: {newQuantity}");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Item Error: {ex.Message}");
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Quantity Error: {ex.Message}");
        }
    }



    public void RemoveItemById<T>(
        InventoryRepository<T> repo,
        int id)
        where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);

            Console.WriteLine(
                $"Item with ID {id} removed successfully.");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Remove Error: {ex.Message}");
        }
    }


    // Helper Methods
    // --------------------------------------------------

    public InventoryRepository<ElectronicItem> GetElectronicRepository()
    {
        return _electronic;
    }

    public InventoryRepository<GroceryItem> GetGroceryRepository()
    {
        return _groceries;
    }
}


// 7. MAIN APPLICATION
// ======================================================

public class Program
{
    public static void Main()
    {
        WareHouseManager manager = new WareHouseManager();


        // i. Instantiate WarehouseManager
        Console.WriteLine("     WAREHOUSE INVENTORY SYSTEM");
        Console.WriteLine("======================================\n");



        // ii. Seed Data
        manager.SeedData();


        // iii. Print Grocery Items
        Console.WriteLine("\n========== GROCERY ITEMS ==========");

        manager.PrintAllItems(
            manager.GetGroceryRepository()
        );


        // iv. Print Electronic Items

        Console.WriteLine("\n======= ELECTRONIC ITEMS =======");

        manager.PrintAllItems(
            manager.GetElectronicRepository()
        );


        // v. Test Duplicate Item

        Console.WriteLine("\n======= TEST DUPLICATE ITEM =======");

        try
        {
            GroceryItem duplicate = new GroceryItem(
                201,
                "Orange Juice",
                10,
                new DateTime(2026, 12, 1)
            );

            manager.GetGroceryRepository().AddItem(duplicate);
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Duplicate Error: {ex.Message}");
        }


        Console.WriteLine("\n===== TEST REMOVE NON-EXISTENT =====");

        manager.RemoveItemById(
            manager.GetGroceryRepository(),
            999
        );


        Console.WriteLine("\n====== TEST INVALID QUANTITY ======");

        try
        {
            manager.GetElectronicRepository()
                   .UpdateQuantity(101, -50);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Quantity Error: {ex.Message}");
        }


        Console.WriteLine("\n======= TEST INCREASE STOCK =======");

        manager.IncreaseStock(
            manager.GetElectronicRepository(),
            101,
            5
        );


        Console.WriteLine("\n====== FINAL ELECTRONIC INVENTORY ======");

        manager.PrintAllItems(
            manager.GetElectronicRepository()
        );

        Console.WriteLine("\n====== FINAL GROCERY INVENTORY ======");

        manager.PrintAllItems(
            manager.GetGroceryRepository()
        );


        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}