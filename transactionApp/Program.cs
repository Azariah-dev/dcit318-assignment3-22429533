using System;
using System.Collections.Generic;

// A. Transaction Record

public record Transaction(
    int Id,
    DateTime Date,
    decimal Amount,
    string Category
);


// B. Transaction Processor Interface

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

// C. Concrete Transaction Processors


public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Bank Transfer: Processing GH₵{transaction.Amount:F2} " +
            $"for {transaction.Category}."
        );
    }
}


public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Mobile Money: Processing GH₵{transaction.Amount:F2} " +
            $"for {transaction.Category}."
        );
    }
}


public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine(
            $"Crypto Wallet: Processing GH₵{transaction.Amount:F2} " +
            $"for {transaction.Category}."
        );
    }
}

// D. General Account

public class Account
{
    public string AccountNumber { get; set; }

    public decimal Balance { get; protected set; }

    public Account(string accountNumber, decimal initialBalance)
    {
        AccountNumber = accountNumber;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
        Balance -= transaction.Amount;

        Console.WriteLine(
            $"Transaction applied. New balance: GH₵{Balance:F2}"
        );
    }
}

// E. Sealed SavingsAccount

public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountNumber, decimal initialBalance)
        : base(accountNumber, initialBalance)
    {
    }

    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount > Balance)
        {
            Console.WriteLine("Insufficient funds");
        }
        else
        {
            Balance -= transaction.Amount;

            Console.WriteLine(
                $"Transaction of GH₵{transaction.Amount:F2} applied."
            );

            Console.WriteLine(
                $"Updated balance: GH₵{Balance:F2}"
            );
        }
    }
}

// F. Finance Application

public class FinanceApp
{
    private List<Transaction> _transactions = new List<Transaction>();

    public void Run()
    {
        // 1. Create Savings Account
        SavingsAccount account =
            new SavingsAccount("ACC001", 1000m);
        Console.WriteLine("      FINANCE MANAGEMENT SYSTEM");
        Console.WriteLine("=================================");
        Console.WriteLine($"Account Number: {account.AccountNumber}");
        Console.WriteLine($"Initial Balance: GH₵{account.Balance:F2}");
        Console.WriteLine();

        // 2. Create three Transaction records

        Transaction transaction1 = new Transaction(
            1,
            DateTime.Now,
            150m,
            "Groceries"
        );

        Transaction transaction2 = new Transaction(
            2,
            DateTime.Now,
            200m,
            "Utilities"
        );

        Transaction transaction3 = new Transaction(
            3,
            DateTime.Now,
            100m,
            "Entertainment"
        );

        // 3 & 4. Process and Apply Transactions

        // Transaction 1 - Mobile Money
        ITransactionProcessor mobileMoney =
            new MobileMoneyProcessor();

        mobileMoney.Process(transaction1);
        account.ApplyTransaction(transaction1);

        Console.WriteLine();


        // Transaction 2 - Bank Transfer
        ITransactionProcessor bankTransfer =
            new BankTransferProcessor();

        bankTransfer.Process(transaction2);
        account.ApplyTransaction(transaction2);

        Console.WriteLine();


        // Transaction 3 - Crypto Wallet
        ITransactionProcessor cryptoWallet =
            new CryptoWalletProcessor();

        cryptoWallet.Process(transaction3);
        account.ApplyTransaction(transaction3);

        Console.WriteLine();


        // 5. Add transactions to the list
        _transactions.Add(transaction1);
        _transactions.Add(transaction2);
        _transactions.Add(transaction3);


        // Display transaction history
        Console.WriteLine("       TRANSACTION HISTORY");
        Console.WriteLine("=================================");

        foreach (Transaction transaction in _transactions)
        {
            Console.WriteLine(
                $"ID: {transaction.Id} | " +
                $"Date: {transaction.Date:g} | " +
                $"Amount: GH₵{transaction.Amount:F2} | " +
                $"Category: {transaction.Category}"
            );
        }

        Console.WriteLine();
        Console.WriteLine($"Final Balance: GH₵{account.Balance:F2}");
    }
}

// Main Application

public class Program
{
    public static void Main()
    {
        FinanceApp app = new FinanceApp();

        app.Run();
    }
}
