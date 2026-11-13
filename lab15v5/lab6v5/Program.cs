using System;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== 1. СТВОРЕННЯ ОБ'ЄКТІВ ТА ВИКЛИК УНІКАЛЬНИХ МЕТОДІВ ===");
SavingsAccount savings = new SavingsAccount("SA-1001", 1000m, 0.05m);
CheckingAccount checking = new CheckingAccount("CA-2002", 500m, 200m);

savings.AddInterest();
checking.ProcessCheck();
Console.WriteLine();

Console.WriteLine("=== 2. ДЕМОНСТРАЦІЯ ПОЛІМОРФІЗМУ (override) ===");
BankAccount[] accounts = new BankAccount[]
{
    new BankAccount("BA-0000", 100m),
    savings,
    checking
};

foreach (var acc in accounts)
{
    acc.Deposit(500m);
}
Console.WriteLine();

Console.WriteLine("=== 3. ДЕМОНСТРАЦІЯ РІЗНИЦІ МІЖ override ТА new ===");
SavingsAccount directSavings = new SavingsAccount("SA-3003", 2000m, 0.03m);
BankAccount baseReference = directSavings;

Console.WriteLine("--- Перевизначений метод (override): ---");
directSavings.Deposit(100m);
baseReference.Deposit(100m);

Console.WriteLine("\n--- Прихований метод (new): ---");
Console.WriteLine($"Через посилання типу SavingsAccount: {directSavings.GetAccountType()}");
Console.WriteLine($"Через посилання типу BankAccount:    {baseReference.GetAccountType()}");


// ==========================================
// КЛАСИ
// ==========================================

public class BankAccount
{
    private string _accountNumber;
    private decimal _balance;

    public string AccountNumber
    {
        get => _accountNumber;
        set => _accountNumber = value;
    }

    public decimal Balance
    {
        get => _balance;
        protected set => _balance = value;
    }

    public BankAccount(string accountNumber, decimal initialBalance)
    {
        _accountNumber = accountNumber;
        _balance = initialBalance;
    }

    public virtual void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            Console.WriteLine($"[BankAccount] Внесено {amount:C}. Поточний баланс: {Balance:C}");
        }
    }

    public string GetAccountType()
    {
        return "Базовий банківський рахунок (BankAccount)";
    }
}

public class SavingsAccount : BankAccount
{
    public decimal InterestRate { get; set; }

    public SavingsAccount(string accountNumber, decimal initialBalance, decimal interestRate)
        : base(accountNumber, initialBalance)
    {
        InterestRate = interestRate;
    }

    public override void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            Console.WriteLine($"[SavingsAccount] Поповнення ощадного рахунку на {amount:C}. Баланс: {Balance:C}");
        }
    }

    public void AddInterest()
    {
        decimal interest = Balance * InterestRate;
        Balance += interest;
        Console.WriteLine($"[SavingsAccount] Нараховано відсотки: {interest:C}. Новий баланс: {Balance:C}");
    }

    public new string GetAccountType()
    {
        return "Ощадний рахунок (SavingsAccount)";
    }
}

public class CheckingAccount : BankAccount
{
    public decimal OverdraftLimit { get; set; }

    public CheckingAccount(string accountNumber, decimal initialBalance, decimal overdraftLimit)
        : base(accountNumber, initialBalance)
    {
        OverdraftLimit = overdraftLimit;
    }

    public override void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            Balance += amount;
            Console.WriteLine($"[CheckingAccount] Внесено {amount:C} на розрахунковий рахунок. Баланс: {Balance:C}");
        }
    }

    public void ProcessCheck()
    {
        Console.WriteLine($"[CheckingAccount] Обробка чек-операції для рахунку {AccountNumber}. Ліміт овердрафту: {OverdraftLimit:C}");
    }
}