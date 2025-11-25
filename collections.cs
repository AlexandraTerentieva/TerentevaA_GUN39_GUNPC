using System;
using System.Collections.Generic;

namespace HomeWork
{
    internal class Program
    {
        private class ListTask
        {
            public void TaskLoop()
            {
                Console.WriteLine("=== TASK 1: LIST OPERATIONS ===");
                Console.WriteLine("Enter '--exit' to quit");
                List<string> list = new List<string>();
                list.Add("Apple");
                list.Add("Banana");
                list.Add("Orange");
                Console.Write("Enter a new string to add to the list: ");
                string newString = Console.ReadLine();
                if (newString == "--exit") return;
                list.Add(newString);
                Console.WriteLine("Current list contents:");
                foreach (string item in list)
                {
                    Console.WriteLine(item);
                }
                Console.Write("Enter another string to insert in the middle: ");
                string middleString = Console.ReadLine();
                if (middleString == "--exit") return;
                int middleIndex = list.Count / 2;
                list.Insert(middleIndex, middleString);

                // Финальный вывод
                Console.WriteLine("Final list contents:");
                foreach (string item in list)
                {
                    Console.WriteLine(item);
                }

                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }
        private class DictionaryTask
        {
            public void TaskLoop()
            {
                Console.WriteLine("=== TASK 2: STUDENT GRADES DICTIONARY ===");
                Console.WriteLine("Enter '--exit' to quit");
                Dictionary<string, double> students = new Dictionary<string, double>();
                Console.Write("Enter student name: ");
                string name = Console.ReadLine();
                if (name == "--exit") return;

                Console.Write("Enter student grade (2-5): ");
                string gradeInput = Console.ReadLine();
                if (gradeInput == "--exit") return;
                if (double.TryParse(gradeInput, out double grade) && grade >= 2 && grade <= 5)
                {
                    students[name] = grade;
                    Console.WriteLine($"Student {name} added with grade {grade}");
                }
                else
                {
                    Console.WriteLine("Invalid grade! Must be between 2 and 5.");
                    return;
                }
                Console.Write("Enter student name to find grade: ");
                string searchName = Console.ReadLine();
                if (searchName == "--exit") return;
                if (students.ContainsKey(searchName))
                {
                    Console.WriteLine($"Student {searchName} has grade: {students[searchName]}");
                }
                else
                {
                    Console.WriteLine($"Student {searchName} does not exist in the dictionary.");
                }

                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }
        private class LinkedListTask
        {
            private class Node
            {
                public string Data { get; set; }
                public Node Previous { get; set; }
                public Node Next { get; set; }

                public Node(string data)
                {
                    Data = data;
                }
            }
            public void TaskLoop()
            {
                Console.WriteLine("=== TASK 3: DOUBLY LINKED LIST ===");
                Console.WriteLine("Enter '--exit' to quit");

                Node head = null;
                Node tail = null;
                int count = 0;
                Console.WriteLine("Create a list with 3 to 6 elements:");

                while (count < 6)
                {
                    Console.Write($"Enter element {count + 1}: ");
                    string input = Console.ReadLine();

                    if (input == "--exit") break;

                    if (!string.IsNullOrEmpty(input))
                    {
                        Node newNode = new Node(input);

                        if (head == null)
                        {
                            head = newNode;
                            tail = newNode;
                        }
                        else
                        {
                            tail.Next = newNode;
                            newNode.Previous = tail;
                            tail = newNode;
                        }
                        count++;
                    }

                    if (count >= 3 && count < 6)
                    {
                        Console.Write("Add more elements? (y/n): ");
                        if (Console.ReadLine().ToLower() != "y") break;
                    }
                }
                Console.WriteLine("List in forward order:");
                Node current = head;
                while (current != null)
                {
                    Console.Write($"{current.Data} ");
                    current = current.Next;
                }
                Console.WriteLine();
                Console.WriteLine("List in backward order:");
                current = tail;
                while (current != null)
                {
                    Console.Write($"{current.Data} ");
                    current = current.Previous;
                }
                Console.WriteLine();

                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Enter 1, 2 or 3 to select task:");

            if (int.TryParse(Console.ReadLine(), out int task))
            {
                switch (task)
                {
                    case 1:
                        new ListTask().TaskLoop();
                        break;
                    case 2:
                        new DictionaryTask().TaskLoop();
                        break;
                    case 3:
                        new LinkedListTask().TaskLoop();
                        break;
                    default:
                        Console.WriteLine("Invalid task number");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input");
            }
        }
    }
}