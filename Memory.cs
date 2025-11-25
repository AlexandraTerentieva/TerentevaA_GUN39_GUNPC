namespace HomeWork
{
    // Структура Interval - ТЗ 2
    public struct Interval
    {
        public float Min { get; }
        public float Max { get; }

        private static Random random = new Random();

        public Interval(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                (minValue, maxValue) = (maxValue, minValue);
                Console.WriteLine("Incorrect input data. Values swapped.");
            }

            if (minValue < 0)
            {
                minValue = 0;
                Console.WriteLine("Incorrect input data. Negative value changed to 0.");
            }

            if (maxValue < 0)
            {
                maxValue = 0;
                Console.WriteLine("Incorrect input data. Negative value changed to 0.");
            }

            if (minValue == maxValue)
            {
                maxValue += 10;
                Console.WriteLine("Incorrect input data. Max value increased by 10.");
            }

            Min = minValue;
            Max = maxValue;
        }

        public float Get()
        {
            return (float)(random.NextDouble() * (Max - Min) + Min);
        }
    }

    // Структура Room - ТЗ 2
    public struct Room
    {
        public Unit Unit;
        public Weapon Weapon;

        public Room(Unit unit, Weapon weapon)
        {
            Unit = unit;
            Weapon = weapon;
        }
    }

    // Класс Dungeon - ТЗ 2
    public class Dungeon
    {
        private Room[] rooms;

        public Dungeon()
        {
            rooms = new Room[]
            {
                new Room(new Unit("Warrior", 0, 10), new Weapon("Sword", 5, 15)),
                new Room(new Unit("Mage", 0, 8), new Weapon("Staff", 3, 12)),
                new Room(new Unit("Archer", 0, 12), new Weapon("Bow", 4, 18))
            };
        }

        public void ShowRooms()
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                var room = rooms[i];
                Console.WriteLine("Unit of room" + room.Unit);
                Console.WriteLine("Weapon of room" + room.Weapon);
                Console.WriteLine("---");
            }
        }
    }

    // Класс Unit - ТЗ 1 + доработки ТЗ 2
    public class Unit
    {
        public string Name { get; }
        public float Health => _health;
        public Interval Damage { get; } // ТЗ 2: заменен на Interval
        public float Armor { get; }

        private float _health;

        // ТЗ 1: Конструкторы
        public Unit() : this("Unknown Unit")
        {
        }

        public Unit(string name) : this(name, 0, 5) // ТЗ 2: вызов нового конструктора
        {
        }

        // ТЗ 2: Новый конструктор с параметрами урона
        public Unit(string name, int minDamage, int maxDamage)
        {
            Name = name;
            Damage = new Interval(minDamage, maxDamage); // ТЗ 2: Interval
            Armor = 0.6f;
            _health = 100f;
        }

        public float GetRealHealth()
        {
            return Health * (1f + Armor);
        }

        public bool SetDamage(float value)
        {
            _health -= value * Armor;
            return _health <= 0f;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    // Класс Weapon - ТЗ 1 + доработки ТЗ 2
    public class Weapon
    {
        public string Name { get; }
        public Interval Damage { get; private set; } // ТЗ 2: заменен на Interval
        public float Durability { get; }

        public Weapon(string name)
        {
            Name = name;
            Durability = 1f;
            Damage = new Interval(1, 10); // ТЗ 2: Interval
        }

        public Weapon(string name, int minDamage, int maxDamage) : this(name)
        {
            SetDamageParams(minDamage, maxDamage);
        }

        public void SetDamageParams(int minDamage, int maxDamage)
        {
            // ТЗ 1: Сообщения из оригинального задания
            if (minDamage > maxDamage)
            {
                (minDamage, maxDamage) = (maxDamage, minDamage);
                Console.WriteLine($"Incorrect input data for weapon '{Name}'. Values swapped.");
            }

            if (minDamage < 1)
            {
                minDamage = 1;
                Console.WriteLine($"Forced minimum value for weapon '{Name}'.");
            }

            if (maxDamage <= 1)
            {
                maxDamage = 10;
            }

            Damage = new Interval(minDamage, maxDamage); // ТЗ 2: Interval
        }

        public int GetDamage()
        {
            // ТЗ 1: среднее арифметическое между MinDamage и MaxDamage
            return ((int)Damage.Min + (int)Damage.Max) / 2;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // ТЗ 1: Тестирование оригинальных классов
            Unit unit1 = new Unit();
            Unit unit2 = new Unit("Warrior");

            Console.WriteLine($"Unit 1: {unit1.Name}, Health: {unit1.Health}, Damage: {unit1.Damage}, Armor: {unit1.Armor}");
            Console.WriteLine($"Unit 2: {unit2.Name}, Health: {unit2.Health}, Damage: {unit2.Damage}, Armor: {unit2.Armor}");
            Console.WriteLine($"Unit 2 Real Health: {unit2.GetRealHealth()}");

            bool isDead = unit2.SetDamage(50f);
            Console.WriteLine($"After taking 50 damage: Health = {unit2.Health}, Is Dead: {isDead}");

            Weapon sword = new Weapon("Sword", 5, 15);
            Weapon axe = new Weapon("Axe", 10, 5);
            Weapon brokenWeapon = new Weapon("Broken", -5, 1);

            Console.WriteLine($"Weapon: {sword.Name}, Damage: {sword.GetDamage()}, Range: {sword.Damage.Min}-{sword.Damage.Max}");
            Console.WriteLine($"Weapon: {axe.Name}, Damage: {axe.GetDamage()}, Range: {axe.Damage.Min}-{axe.Damage.Max}");
            Console.WriteLine($"Weapon: {brokenWeapon.Name}, Damage: {brokenWeapon.GetDamage()}, Range: {brokenWeapon.Damage.Min}-{brokenWeapon.Damage.Max}");

            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("ТЗ 2: Dungeon with Rooms");
            Console.WriteLine(new string('=', 40));

            // ТЗ 2: создание Dungeon и вызов ShowRooms
            Dungeon dungeon = new Dungeon();
            dungeon.ShowRooms();

            // Тестирование Interval
            Console.WriteLine("\nTesting Interval:");
            Interval interval = new Interval(5, 15);
            Console.WriteLine($"Interval: {interval.Min}-{interval.Max}");
            Console.WriteLine($"Random value: {interval.Get()}");
        }
    }
}