class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter first number:");
        if (!Int32.TryParse(Console.ReadLine(), out int a))
        {
            Console.WriteLine("Error!");
            return;
        }
        Console.WriteLine("Enter second number:");
        if (!Int32.TryParse(Console.ReadLine(), out int b))
        {
            Console.WriteLine("Error!");
            return;
        }
        Console.WriteLine("Enter operator (&, |, ^):");
        string s = Console.ReadLine();
        if (string.IsNullOrEmpty(s) || s.Length != 1 ||
            (s[0] != '&' && s[0] != '|' && s[0] != '^'))
        {
            Console.WriteLine("Wrong sign");
            return;
        }
        int result = 0;
        switch (s[0])
        {
            case '&':
                result = a & b;
                break;
            case '|':
                result = a | b;
                break;
            case '^':
                result = a ^ b;
                break;
        }
        Console.WriteLine($"Decimal: {result}");
        Console.WriteLine($"Binary: {Convert.ToString(result, 2)}");
        Console.WriteLine($"Hexadecimal: 0x{result:X}");
    }
}