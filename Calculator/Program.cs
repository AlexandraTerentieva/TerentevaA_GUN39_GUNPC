namespace HomeWork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] fibonacci = new int[8] { 0, 1, 1, 2, 3, 5, 8, 13 };
            Console.WriteLine("Task 1 - Fibonacci numbers (first 8):");
            Console.WriteLine(string.Join(", ", fibonacci));
            string[] months = new string[12] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            Console.WriteLine("\nTask 2 - Months:");
            Console.WriteLine(string.Join(", ", months));
            int[,] matrix = new int[3, 3] { { 2, 3, 4 }, { 4, 9, 16 }, { 8, 27, 64 } };
            Console.WriteLine("\nTask 3 - Matrix 3x3:");
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
            double[][] jaggedArray = new double[3][];
            jaggedArray[0] = new double[5] { 1, 2, 3, 4, 5 };
            jaggedArray[1] = new double[2] { Math.E, Math.PI };
            jaggedArray[2] = new double[4] { Math.Log10(1), Math.Log10(10), Math.Log10(100), Math.Log10(1000) };
            Console.WriteLine("\nTask 4 - Jagged array:");
            for (int i = 0; i < jaggedArray.Length; i++)
            {
                Console.WriteLine($"Array {i + 1}: [{string.Join(", ", jaggedArray[i])}]");
            }
            int[] array = { 1, 2, 3, 4, 5 };
            int[] array2 = { 7, 8, 9, 10, 11, 12, 13 };
            Console.WriteLine("\nTask 5 - Copy first 3 elements:");
            Console.WriteLine("Before copy - array: " + string.Join(", ", array));
            Console.WriteLine("Before copy - array2: " + string.Join(", ", array2));
            var result = CopyArrays(array, array2, 3);
            Console.WriteLine("After copy - array2: " + string.Join(", ", result));
            string[] sample = { "", "" };
            Console.WriteLine("\nTask 6 - Resize array:");
            Console.WriteLine("Before resize - array: " + string.Join(", ", array));
            Console.WriteLine("Before resize - length: " + array.Length);
            ResizeArray(ref array, array.Length * 2);
            Console.WriteLine("After resize - array: " + string.Join(", ", array));
            Console.WriteLine("After resize - length: " + array.Length);
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
        static int[] CopyArrays(int[] source, int[] destination, int count)
        {
            Array.Copy(source, 0, destination, 0, count);
            return destination;
        }
        static void ResizeArray(ref int[] array, int newSize)
        {
            Array.Resize(ref array, newSize);
        }
    }
}