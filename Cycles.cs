namespace HomeWork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Task 1 - First 10 Fibonacci numbers:");
            int a = 0, b = 1;
            Console.Write(a + ", " + b);
            for (int i = 2; i < 10; i++)
            {
                int next = a + b;
                Console.Write(", " + next);
                a = b;
                b = next;
            }
            Console.WriteLine();
            Console.WriteLine("\nTask 2 - Even numbers from 2 to 20:");
            for (int i = 2; i <= 20; i += 2)
            {
                Console.Write(i + " ");
            }
            Console.WriteLine();
            Console.WriteLine("\nTask 3 - Multiplication table from 1 to 5:");
            for (int i = 1; i <= 5; i++)
            {
                for (int j = 1; j <= 5; j++)
                {
                    Console.Write($"{i} * {j} = {i * j}\t");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\nTask 4 - Password check:");
            string password = "qwerty";
            string input;
            do
            {
                Console.Write("Enter password: ");
                input = Console.ReadLine();
            } while (input != password);
            Console.WriteLine("Access granted!");
        }
    }
}