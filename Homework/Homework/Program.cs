namespace RogueLikeGame
{
    public enum EquipmentSlot
    {
        Weapon,
        Armor,
        Helmet,
        RangeWeapon
    }

    public enum DifficultyLevel
    {
        Easy,
        Hard
    }

    public abstract class Item
    {
        public string Name { get; protected set; }

        public override bool Equals(object obj)
        {
            if (obj is Item other)
                return Name == other.Name;
            return false;
        }

        public override int GetHashCode() => Name?.GetHashCode() ?? 0;

        public virtual string GetInfo() => Name;
    }

    public abstract class EquipableItem : Item
    {
        public int Durability { get; protected set; }
        public int MaxDurability { get; protected set; }

        public void Repair(int amount)
        {
            Durability = Math.Min(Durability + amount, MaxDurability);
        }

        public void LoseDurability()
        {
            Durability = Math.Max(0, Durability - 1);
        }

        public override string GetInfo() => $"{Name} (Durability: {Durability}/{MaxDurability})";
    }

    public class Weapon : EquipableItem
    {
        public int Damage { get; private set; }

        public Weapon(string name = "Sword", int damage = 10, int maxDurability = 15)
        {
            Name = name;
            Damage = damage;
            Durability = maxDurability;
            MaxDurability = maxDurability;
        }

        public override string GetInfo() => $"{Name} (Damage: {Damage}, Durability: {Durability}/{MaxDurability})";
    }

    public class RangeWeapon : EquipableItem
    {
        public int Damage { get; private set; }
        public int Range { get; private set; }

        public RangeWeapon(string name = "Bow", int damage = 8, int range = 3, int maxDurability = 12)
        {
            Name = name;
            Damage = damage;
            Range = range;
            Durability = maxDurability;
            MaxDurability = maxDurability;
        }

        public override string GetInfo() => $"{Name} (Damage: {Damage}, Range: {Range}, Durability: {Durability}/{MaxDurability})";
    }

    public class Armor : EquipableItem
    {
        public int Defense { get; private set; }

        public Armor(string name = "Armor", int defense = 5, int maxDurability = 15)
        {
            Name = name;
            Defense = Math.Min(defense, 50);
            Durability = maxDurability;
            MaxDurability = maxDurability;
        }

        public override string GetInfo() => $"{Name} (Defense: {Defense}%, Durability: {Durability}/{MaxDurability})";
    }

    public class Helmet : EquipableItem
    {
        public int Defense { get; private set; }

        public Helmet(string name = "Helmet", int defense = 3, int maxDurability = 10)
        {
            Name = name;
            Defense = Math.Min(defense, 30);
            Durability = maxDurability;
            MaxDurability = maxDurability;
        }

        public override string GetInfo() => $"{Name} (Defense: {Defense}%, Durability: {Durability}/{MaxDurability})";
    }

    public abstract class EconomicItem : Item
    {
        public int Quantity { get; set; }
        public bool IsStackable { get; protected set; }

        public override string GetInfo() => $"{Name} x{Quantity}";
    }

    public class Gold : EconomicItem
    {
        public Gold(int quantity = 1)
        {
            Name = "Gold";
            Quantity = quantity;
            IsStackable = true;
        }
    }

    public abstract class UsableItem : EconomicItem
    {
        public abstract void Use(Player player);
    }

    public class HealthPotion : UsableItem
    {
        public HealthPotion()
        {
            Name = "Health Potion";
            Quantity = 1;
            IsStackable = false;
        }

        public override void Use(Player player)
        {
            player.Heal(7);
        }

        public override string GetInfo() => $"{Name} (Heals 7 HP)";
    }

    public class Whetstone : UsableItem
    {
        public Whetstone()
        {
            Name = "Whetstone";
            Quantity = 1;
            IsStackable = false;
        }

        public override void Use(Player player)
        {
            bool repaired = false;

            if (player.EquippedItems.ContainsKey(EquipmentSlot.Weapon) &&
                player.EquippedItems[EquipmentSlot.Weapon] is EquipableItem weapon)
            {
                weapon.Repair(4);
                repaired = true;
            }

            if (player.EquippedItems.ContainsKey(EquipmentSlot.Armor) &&
                player.EquippedItems[EquipmentSlot.Armor] is EquipableItem armor)
            {
                armor.Repair(4);
                repaired = true;
            }

            if (player.EquippedItems.ContainsKey(EquipmentSlot.Helmet) &&
                player.EquippedItems[EquipmentSlot.Helmet] is EquipableItem helmet)
            {
                helmet.Repair(4);
                repaired = true;
            }

            if (player.EquippedItems.ContainsKey(EquipmentSlot.RangeWeapon) &&
                player.EquippedItems[EquipmentSlot.RangeWeapon] is EquipableItem rangeWeapon)
            {
                rangeWeapon.Repair(4);
                repaired = true;
            }

            if (repaired)
            {
                Console.WriteLine("Whetstone used! All equipped items repaired by 4 durability.");
            }
        }

        public override string GetInfo() => $"{Name} (Repairs 4 durability for all equipped items)";
    }

    public abstract class Unit
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int BaseDamage { get; protected set; }

        public bool IsAlive => Health > 0;

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public void Heal(int amount)
        {
            Health = Math.Min(MaxHealth, Health + amount);
        }

        public virtual int CalculateDefense() => 0;
    }

    public class Goblin : Unit
    {
        public Goblin()
        {
            Name = "Goblin";
            MaxHealth = 18;
            Health = MaxHealth;
            BaseDamage = 2;
        }

        public override int CalculateDefense() => 0;
    }

    public class Orc : Unit
    {
        public Orc()
        {
            Name = "Orc";
            MaxHealth = 25;
            Health = MaxHealth;
            BaseDamage = 4;
        }

        public override int CalculateDefense() => 2;
    }

    public class Player : Unit
    {
        public Dictionary<EquipmentSlot, EquipableItem> EquippedItems { get; private set; }
        public List<Item> Inventory { get; private set; }
        private const int MaxInventorySize = 5;

        public Player(string name)
        {
            Name = name;
            MaxHealth = 30;
            Health = MaxHealth;
            BaseDamage = 6;
            EquippedItems = new Dictionary<EquipmentSlot, EquipableItem>();
            Inventory = new List<Item>();
        }

        public int CalculateDamage()
        {
            int damage = BaseDamage;

            if (EquippedItems.ContainsKey(EquipmentSlot.Weapon) && EquippedItems[EquipmentSlot.Weapon] is Weapon weapon)
            {
                damage += weapon.Damage;
            }
            else if (EquippedItems.ContainsKey(EquipmentSlot.RangeWeapon) && EquippedItems[EquipmentSlot.RangeWeapon] is RangeWeapon rangeWeapon)
            {
                damage += rangeWeapon.Damage;
            }

            return damage;
        }

        public override int CalculateDefense()
        {
            int defense = 0;

            if (EquippedItems.ContainsKey(EquipmentSlot.Armor) && EquippedItems[EquipmentSlot.Armor] is Armor armor)
            {
                defense += armor.Defense;
            }

            if (EquippedItems.ContainsKey(EquipmentSlot.Helmet) && EquippedItems[EquipmentSlot.Helmet] is Helmet helmet)
            {
                defense += helmet.Defense;
            }

            return Math.Min(defense, 80);
        }

        public bool AddToInventory(Item item)
        {
            if (Inventory.Count >= MaxInventorySize)
                return false;

            if (item is EconomicItem economicItem && economicItem.IsStackable)
            {
                var existingItem = Inventory.OfType<EconomicItem>()
                    .FirstOrDefault(i => i.Name == item.Name && i.IsStackable);

                if (existingItem != null)
                {
                    existingItem.Quantity += economicItem.Quantity;
                    return true;
                }
            }

            Inventory.Add(item);
            return true;
        }

        public void EquipItem(EquipableItem newItem)
        {
            EquipmentSlot? slot = GetSlotForItem(newItem);

            if (slot.HasValue)
            {
                if (EquippedItems.ContainsKey(slot.Value))
                {
                    var oldItem = EquippedItems[slot.Value];
                    Inventory.Add(oldItem);
                    Console.WriteLine($"Replaced {oldItem.Name} with {newItem.Name} in {slot.Value} slot");
                }
                else
                {
                    Console.WriteLine($"Equipped {newItem.Name} in {slot.Value} slot");
                }

                EquippedItems[slot.Value] = newItem;
                Inventory.Remove(newItem);
            }
        }

        private EquipmentSlot? GetSlotForItem(EquipableItem item)
        {
            return item switch
            {
                Weapon => EquipmentSlot.Weapon,
                RangeWeapon => EquipmentSlot.RangeWeapon,
                Armor => EquipmentSlot.Armor,
                Helmet => EquipmentSlot.Helmet,
                _ => null
            };
        }

        public void UseItems()
        {
            var itemsToUse = Inventory.OfType<UsableItem>().ToList();
            foreach (var item in itemsToUse)
            {
                item.Use(this);
                Inventory.Remove(item);
            }
        }

        public void LoseEquipmentDurability()
        {
            foreach (var equippedItem in EquippedItems.Values)
            {
                equippedItem.LoseDurability();
            }
        }
    }

    public class Room
    {
        public string Name { get; private set; }
        public Dictionary<int, Room> NextRooms { get; private set; }
        public bool HasLoot { get; private set; }
        public bool HasEnemy { get; private set; }
        public bool IsFinal { get; private set; }
        public List<Item> Loot { get; private set; }
        public Unit Enemy { get; private set; }

        public Room(string name, bool hasLoot = false, bool hasEnemy = false, bool isFinal = false, Unit enemy = null)
        {
            Name = name;
            NextRooms = new Dictionary<int, Room>();
            HasLoot = hasLoot;
            HasEnemy = hasEnemy;
            IsFinal = isFinal;
            Loot = new List<Item>();
            Enemy = enemy;

            if (hasLoot)
                GenerateLoot();
        }

        private void GenerateLoot()
        {
            var random = new Random();
            var lootTypes = new List<Item>
            {
                new Gold(random.Next(1, 10)),
                new HealthPotion(),
                new Whetstone(),
                new Weapon(),
                new RangeWeapon(),
                new Armor(),
                new Helmet()
            };

            Loot.Add(lootTypes[random.Next(lootTypes.Count)]);
        }

        public void AddConnection(int direction, Room room)
        {
            NextRooms[direction] = room;
        }
    }

    public abstract class DungeonFactory
    {
        public abstract Room CreateDungeon();
        public abstract Unit CreateEnemy();
    }

    public class EasyDungeonFactory : DungeonFactory
    {
        public override Room CreateDungeon()
        {
            var startRoom = new Room("Entrance Hall");
            var emptyRoom = new Room("Empty Corridor");
            var lootRoom = new Room("Treasure Room", hasLoot: true);
            var enemyRoom = new Room("Goblin Den", hasEnemy: true, enemy: new Goblin());
            var finalRoom = new Room("Throne Room", isFinal: true);

            startRoom.AddConnection(1, emptyRoom);
            emptyRoom.AddConnection(1, lootRoom);
            lootRoom.AddConnection(1, enemyRoom);
            enemyRoom.AddConnection(1, finalRoom);

            return startRoom;
        }

        public override Unit CreateEnemy()
        {
            return new Goblin();
        }
    }

    public class HardDungeonFactory : DungeonFactory
    {
        public override Room CreateDungeon()
        {
            var startRoom = new Room("Dark Entrance");
            var enemyRoom1 = new Room("Orc Guard Post", hasEnemy: true, enemy: new Orc());
            var lootRoom = new Room("Ancient Treasury", hasLoot: true);
            var enemyRoom2 = new Room("Goblin Ambush", hasEnemy: true, enemy: new Goblin());
            var trapRoom = new Room("Spiked Corridor");
            var bossRoom = new Room("Orc Chieftain Lair", hasEnemy: true, enemy: new Orc(), isFinal: true);

            startRoom.AddConnection(1, enemyRoom1);
            enemyRoom1.AddConnection(1, lootRoom);
            enemyRoom1.AddConnection(0, trapRoom);
            lootRoom.AddConnection(1, enemyRoom2);
            trapRoom.AddConnection(1, bossRoom);
            enemyRoom2.AddConnection(1, bossRoom);

            return startRoom;
        }

        public override Unit CreateEnemy()
        {
            var random = new Random();
            return random.Next(2) == 0 ? new Goblin() : new Orc();
        }
    }

    public static class DungeonBuilder
    {
        public static DungeonFactory CreateFactory(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Easy => new EasyDungeonFactory(),
                DifficultyLevel.Hard => new HardDungeonFactory(),
                _ => new EasyDungeonFactory()
            };
        }
    }

    public class GameEngine
    {
        private Player player;
        private Room currentRoom;
        private bool gameRunning;
        private DungeonFactory factory;

        public void StartGame()
        {
            Console.WriteLine("Welcome, player!");

            DifficultyLevel difficulty = ChooseDifficulty();
            factory = DungeonBuilder.CreateFactory(difficulty);

            Console.WriteLine("Enter your name:");
            string playerName = Console.ReadLine();

            player = new Player(playerName);
            currentRoom = factory.CreateDungeon();

            Console.WriteLine($"Hello {player.Name}");
            Console.WriteLine($"Entering the {difficulty} Dungeon");

            gameRunning = true;
            GameLoop();
        }

        private DifficultyLevel ChooseDifficulty()
        {
            while (true)
            {
                Console.WriteLine("Choose difficulty:");
                Console.WriteLine("1 - Easy");
                Console.WriteLine("2 - Hard");

                string input = Console.ReadLine();
                if (input == "1") return DifficultyLevel.Easy;
                if (input == "2") return DifficultyLevel.Hard;

                Console.WriteLine("Invalid choice! Please enter 1 or 2.");
            }
        }

        private void GameLoop()
        {
            while (gameRunning)
            {
                EnterRoom(currentRoom);

                if (!gameRunning) break;

                if (currentRoom.IsFinal)
                {
                    EndGame(true);
                    break;
                }

                ShowAvailableRoutes();
                ProcessMovementInput();
            }
        }

        private void EnterRoom(Room room)
        {
            Console.WriteLine($"Entered room {room.Name}");
            currentRoom = room;

            if (room.HasEnemy && room.Enemy != null)
            {
                StartCombat(room.Enemy);
            }

            if (room.HasLoot && room.Loot.Any())
            {
                CollectLoot(room.Loot);
            }
        }

        private void StartCombat(Unit enemy)
        {
            Console.WriteLine($"A {enemy.Name} appears!");

            while (enemy.IsAlive && player.IsAlive)
            {
                Console.WriteLine("Choose: 1-Rock, 2-Paper, 3-Scissors");
                string input = Console.ReadLine();

                if (!int.TryParse(input, out int playerChoice) || playerChoice < 1 || playerChoice > 3)
                {
                    Console.WriteLine("Invalid input! Please enter 1, 2 or 3.");
                    continue;
                }

                var random = new Random();
                int enemyChoice = random.Next(1, 4);

                Console.WriteLine($"Enemy chose: {GetChoiceName(enemyChoice)}");

                if (playerChoice == enemyChoice)
                {
                    Console.WriteLine("It's a tie!");
                }
                else if ((playerChoice == 1 && enemyChoice == 3) ||
                         (playerChoice == 2 && enemyChoice == 1) ||
                         (playerChoice == 3 && enemyChoice == 2))
                {
                    int playerDamage = player.CalculateDamage();
                    int defense = enemy.CalculateDefense();
                    int finalDamage = Math.Max(1, playerDamage - defense);

                    enemy.TakeDamage(finalDamage);
                    Console.WriteLine($"You hit the {enemy.Name} for {finalDamage} damage!");

                    player.LoseEquipmentDurability();
                }
                else
                {
                    int enemyDamage = enemy.BaseDamage;
                    int defense = player.CalculateDefense();
                    int finalDamage = Math.Max(1, enemyDamage - (enemyDamage * defense / 100));

                    player.TakeDamage(finalDamage);
                    Console.WriteLine($"The {enemy.Name} hits you for {finalDamage} damage!");

                    player.LoseEquipmentDurability();
                }

                Console.WriteLine($"Your health: {player.Health}/{player.MaxHealth}");
                Console.WriteLine($"Enemy health: {enemy.Health}/{enemy.MaxHealth}");
            }

            if (!player.IsAlive)
            {
                EndGame(false);
                return;
            }

            if (!enemy.IsAlive)
            {
                Console.WriteLine($"You defeated the {enemy.Name}!");
                player.UseItems();
            }
        }

        private string GetChoiceName(int choice)
        {
            return choice switch
            {
                1 => "Rock",
                2 => "Paper",
                3 => "Scissors",
                _ => "Unknown"
            };
        }

        private void CollectLoot(List<Item> loot)
        {
            foreach (var item in loot.ToList())
            {
                if (item is EquipableItem equipableItem)
                {
                    Console.WriteLine($"Found: {equipableItem.GetInfo()}");
                    Console.WriteLine("Do you want to equip it? (y/n)");
                    string answer = Console.ReadLine()?.ToLower();

                    if (answer == "y" || answer == "yes")
                    {
                        player.EquipItem(equipableItem);
                        loot.Remove(item);
                    }
                    else if (player.AddToInventory(item))
                    {
                        loot.Remove(item);
                    }
                    else
                    {
                        Console.WriteLine("Inventory full! Could not collect loot.");
                        break;
                    }
                }
                else if (player.AddToInventory(item))
                {
                    Console.WriteLine($"Found: {item.Name}");
                    loot.Remove(item);
                }
                else
                {
                    Console.WriteLine("Inventory full! Could not collect loot.");
                    break;
                }
            }
        }

        private void ShowAvailableRoutes()
        {
            Console.WriteLine("Available routes:");
            foreach (var direction in currentRoom.NextRooms.Keys)
            {
                string directionName = direction switch
                {
                    0 => "Forward",
                    -1 => "Left",
                    1 => "Right",
                    _ => "Unknown"
                };
                Console.WriteLine($"{direction}: {directionName}");
            }
        }

        private void ProcessMovementInput()
        {
            while (true)
            {
                Console.WriteLine("Enter direction number:");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int direction) && currentRoom.NextRooms.ContainsKey(direction))
                {
                    currentRoom = currentRoom.NextRooms[direction];
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid direction! Please enter a valid direction number.");
                }
            }
        }

        private void EndGame(bool victory)
        {
            if (victory)
            {
                Console.WriteLine($"Congratulations {player.Name}!");
                Console.WriteLine("Game completed successfully!");
            }
            else
            {
                Console.WriteLine("Game Over! You were defeated...");
            }

            Console.WriteLine("Final player info:");
            Console.WriteLine($"Health: {player.Health}/{player.MaxHealth}");
            Console.WriteLine($"Inventory: {player.Inventory.Count} items");

            gameRunning = false;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var game = new GameEngine();
            game.StartGame();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}