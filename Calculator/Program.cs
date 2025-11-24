namespace HomeWork
{
    public class Unit
    {
        public string Name { get; }
        public float Health => _health;
        public int Damage { get; }
        public float Armor { get; }

        private float _health;

        public Unit() : this("Unknown Unit")
        {
        }

        public Unit(string name)
        {
            Name = name;
            Damage = 5;
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
    }

    public class Weapon
    {
        public string Name { get; }
        public int MinDamage { get; private set; }
        public int MaxDamage { get; private set; }
        public float Durability { get; }

        public Weapon(string name)
        {
            Name = name;
            Durability = 1f;
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

            MinDamage = minDamage;
            MaxDamage = maxDamage;
        }

        public int GetDamage()
        {
            return (MinDamage + MaxDamage) / 2;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
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

            Console.WriteLine($"Weapon: {sword.Name}, Damage: {sword.GetDamage()}, Range: {sword.MinDamage}-{sword.MaxDamage}");
            Console.WriteLine($"Weapon: {axe.Name}, Damage: {axe.GetDamage()}, Range: {axe.MinDamage}-{axe.MaxDamage}");
            Console.WriteLine($"Weapon: {brokenWeapon.Name}, Damage: {brokenWeapon.GetDamage()}, Range: {brokenWeapon.MinDamage}-{brokenWeapon.MaxDamage}");
        }
    }
}


