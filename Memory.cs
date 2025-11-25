namespace HomeWork
{
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

    public struct Unit
    {
        public string Name { get; }
        public float Health => _health;
        public Interval Damage { get; }
        public float Armor { get; }

        private float _health;

        public Unit(string name) : this(name, 0, 5)
        {
        }

        public Unit(string name, int minDamage, int maxDamage)
        {
            Name = name;
            Damage = new Interval(minDamage, maxDamage);
            Armor = 0.6f;
            _health = 100f;
        }

        public float GetRealHealth()
        {
            return Health * (1f + Armor);
        }

        public bool SetDamage(float value)
        {
            float finalDamage = Math.Max(0, value - Armor);
            _health -= finalDamage;
            return _health <= 0f;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public struct Weapon
    {
        public string Name { get; }
        public Interval Damage { get; private set; }
        public float Durability { get; }

        public Weapon(string name)
        {
            Name = name;
            Durability = 1f;
            Damage = new Interval(1, 10);
        }

        public Weapon(string name, int minDamage, int maxDamage) : this(name)
        {
            SetDamageParams(minDamage, maxDamage);
        }

        public void SetDamageParams(int minDamage, int maxDamage)
        {
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

            Damage = new Interval(minDamage, maxDamage);
        }

        public int GetDamage()
        {
            return (int)Damage.Get();
        }

        public override string ToString()
        {
            return Name;
        }
    }

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
                Console.WriteLine($"Unit of room: {room.Unit}");
                Console.WriteLine($"Weapon of room: {room.Weapon}");
                Console.WriteLine("---");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing Dungeon:");
            Console.WriteLine(new string('=', 40));

            Dungeon dungeon = new Dungeon();
            dungeon.ShowRooms();

            Console.WriteLine("\nTesting Interval:");
            Interval interval = new Interval(5, 15);
            Console.WriteLine($"Interval: {interval.Min}-{interval.Max}");
            Console.WriteLine($"Random value: {interval.Get()}");

            Console.WriteLine("\nTesting Unit damage mechanics:");
            Unit testUnit = new Unit("Test Warrior", 5, 10);
            Console.WriteLine($"Initial health: {testUnit.Health}");
            bool isDead = testUnit.SetDamage(10f);
            Console.WriteLine($"After taking 10 damage: Health = {testUnit.Health}, Is Dead: {isDead}");

            Console.WriteLine("\nTesting Weapon damage:");
            Weapon testWeapon = new Weapon("Test Sword", 8, 12);
            Console.WriteLine($"Weapon: {testWeapon.Name}, Damage: {testWeapon.GetDamage()}");
        }
    }
}