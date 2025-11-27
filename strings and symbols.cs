using System.Text;

namespace HomeWork
{
    internal class Program
    {
        public Program()
        {
        }

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
            if (words == null)
                return new StringBuilder();

            StringBuilder sb = new StringBuilder();
            foreach (string word in words)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    sb.Append(word);
                    sb.Append(" ");
                }
            }
            if (sb.Length > 0)
                sb.Length--;
            return sb;
        }

        public static string ReplaceWords(string inputString, string wordToReplace, string replacementWord)
        {
            if (string.IsNullOrEmpty(inputString) || string.IsNullOrEmpty(wordToReplace))
                return inputString;
            return inputString.Replace(wordToReplace, replacementWord);
        }

        private static void Main(string[] args)
        {
            string result1 = ConcatenateStrings("Hello", "World");
            Console.WriteLine(result1);

            string result2 = GreetUser("John", 25);
            Console.WriteLine(result2);

            string result3 = AnalyzeString("Test String");
            Console.WriteLine(result3);

            string result4 = GetFirstFiveCharacters("Programming");
            Console.WriteLine(result4);

            string[] words = { "This", "is", "a", "test" };
            StringBuilder result5 = ConcatenateStringArray(words);
            Console.WriteLine(result5);

            string result6 = ReplaceWords("Hello world", "world", "universe");
            Console.WriteLine(result6);
        }
    }
}