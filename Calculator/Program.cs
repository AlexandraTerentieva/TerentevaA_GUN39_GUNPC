using System.Text;
namespace HomeWork
{
    public class StringOperations
{
    public static string ConcatenateStrings(string first, string second)
    {
        return first + second;
    }

    public static string GreetUser(string name, int age)
    {
        return $"Hello, {name}!\nYou are {age} years old.";
    }

    public static string AnalyzeString(string input)
    {
        int length = input.Length;
        string upper = input.ToUpper();
        string lower = input.ToLower();
        return $"Length: {length}\nUpper: {upper}\nLower: {lower}";
    }

    public static string GetFirstFiveCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input.Length <= 5 ? input : input.Substring(0, 5);
    }

    public static StringBuilder ConcatenateStringArray(string[] words)
    {
        StringBuilder sb = new StringBuilder();

        if (words == null || words.Length == 0)
            return sb;

        foreach (string word in words)
        {
            if (!string.IsNullOrEmpty(word))
            {
                if (sb.Length > 0)
                    sb.Append(" ");
                sb.Append(word);
            }
        }
        return sb;
    }

    public static string ReplaceWords(string inputString, string wordToReplace, string replacementWord)
    {
        if (string.IsNullOrEmpty(inputString) || string.IsNullOrEmpty(wordToReplace))
            return inputString;

        return inputString.Replace(wordToReplace, replacementWord ?? "");
    }

    public static void RunAllStringOperations()
    {
        Console.WriteLine("=== ТЕСТИРОВАНИЕ ВСЕХ МЕТОДОВ ===");

        Console.WriteLine("\n1. ConcatenateStrings:");
        string result1 = ConcatenateStrings("Hello", "World");
        Console.WriteLine($"   'Hello' + 'World' = '{result1}'");

        string result1_2 = ConcatenateStrings("C# ", "Programming");
        Console.WriteLine($"   'C# ' + 'Programming' = '{result1_2}'");

        Console.WriteLine("\n2. GreetUser:");
        string result2 = GreetUser("Alice", 30);
        Console.WriteLine($"   Name: Alice, Age: 30:\n{result2}");

        string result2_2 = GreetUser("Bob", 25);
        Console.WriteLine($"   Name: Bob, Age: 25:\n{result2_2}");

        Console.WriteLine("\n3. AnalyzeString:");
        string result3 = AnalyzeString("Hello World");
        Console.WriteLine($"   Input: 'Hello World'\n{result3}");

        string result3_2 = AnalyzeString("Test String");
        Console.WriteLine($"   Input: 'Test String'\n{result3_2}");

        Console.WriteLine("\n4. GetFirstFiveCharacters:");
        string result4 = GetFirstFiveCharacters("Programming");
        Console.WriteLine($"   'Programming' -> '{result4}'");

        string result4_2 = GetFirstFiveCharacters("Hi");
        Console.WriteLine($"   'Hi' -> '{result4_2}'");

        string result4_3 = GetFirstFiveCharacters("");
        Console.WriteLine($"   '' -> '{result4_3}'");

        Console.WriteLine("\n5. ConcatenateStringArray:");
        string[] words1 = { "This", "is", "a", "test" };
        StringBuilder result5 = ConcatenateStringArray(words1);
        Console.WriteLine($"   ['This', 'is', 'a', 'test'] -> '{result5}'");

        string[] words2 = { "C#", "is", "awesome" };
        StringBuilder result5_2 = ConcatenateStringArray(words2);
        Console.WriteLine($"   ['C#', 'is', 'awesome'] -> '{result5_2}'");

        Console.WriteLine("\n6. ReplaceWords:");
        string result6 = ReplaceWords("Hello world", "world", "universe");
        Console.WriteLine($"   'Hello world', 'world'->'universe' = '{result6}'");

        string result6_2 = ReplaceWords("cat dog cat", "cat", "bird");
        Console.WriteLine($"   'cat dog cat', 'cat'->'bird' = '{result6_2}'");

        string result6_3 = ReplaceWords("Hello world", "python", "C#");
        Console.WriteLine($"   'Hello world', 'python'->'C#' = '{result6_3}'");
    }
}
}