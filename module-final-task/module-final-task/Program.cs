using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CasinoGame
{
    // ========== ИНТЕРФЕЙСЫ ==========
    public interface ISaveLoadService<T>
    {
        void SaveData(T data, string identifier);
        T LoadData(string identifier);
    }

    public interface IGame
    {
        void StartGame();
    }

    // ========== ИСКЛЮЧЕНИЯ ==========
    public class WrongDiceNumberException : Exception
    {
        public WrongDiceNumberException(int number, int min, int max)
            : base($"Number {number} is out of range. Allowed range: {min}-{max}")
        {
        }
    }

    // ========== ПЕРЕЧИСЛЕНИЯ ==========
    public enum CardSuit
    {
        Diamonds,
        Hearts,
        Clubs,
        Spades
    }

    public enum CardRank
    {
        Six = 6,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    // ========== МОДЕЛИ ==========
    public struct Card
    {
        public CardSuit Suit { get; }
        public CardRank Rank { get; }

        public Card(CardSuit suit, CardRank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString() => $"{Rank} of {Suit}";
    }

    public struct Dice
    {
        private readonly int _min;
        private readonly int _max;
        private readonly Random _random;

        public int Number => _random.Next(_min, _max + 1);

        public Dice(int min, int max)
        {
            if (min < 1 || max > int.MaxValue)
            {
                throw new WrongDiceNumberException(min < 1 ? min : max, 1, int.MaxValue);
            }

            if (min > max)
            {
                throw new WrongDiceNumberException(min, min, max);
            }

            _min = min;
            _max = max;
            _random = new Random();
        }
    }

    public class PlayerProfile
    {
        public string Name { get; set; }
        public decimal Bank { get; set; }

        [JsonConstructor]
        public PlayerProfile() { }

        public PlayerProfile(string name, decimal initialBank = 1000)
        {
            Name = name;
            Bank = initialBank;
        }

        public bool CanBet(decimal amount) => amount <= Bank && amount > 0;
    }

    // ========== СЕРВИСЫ ==========
    public class FileSystemSaveLoadService : ISaveLoadService<string>
    {
        private readonly string _savePath;

        public FileSystemSaveLoadService(string savePath)
        {
            _savePath = savePath ?? throw new ArgumentNullException(nameof(savePath));

            if (!Directory.Exists(_savePath))
            {
                Directory.CreateDirectory(_savePath);
            }
        }

        public void SaveData(string data, string identifier)
        {
            try
            {
                string filePath = Path.Combine(_savePath, $"{identifier}.txt");
                File.WriteAllText(filePath, data);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save data: {ex.Message}", ex);
            }
        }

        public string LoadData(string identifier)
        {
            try
            {
                string filePath = Path.Combine(_savePath, $"{identifier}.txt");

                if (!File.Exists(filePath))
                {
                    return null;
                }

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load data: {ex.Message}", ex);
            }
        }
    }

    // ========== БАЗОВЫЙ КЛАСС ИГР ==========
    public abstract class CasinoGameBase
    {
        public event Action OnWin;
        public event Action OnLoose;
        public event Action OnDraw;

        protected CasinoGameBase()
        {
            FactoryMethod();
        }

        protected abstract void FactoryMethod();
        public abstract void PlayGame();

        protected void OnWinInvoke() => OnWin?.Invoke();
        protected void OnLooseInvoke() => OnLoose?.Invoke();
        protected void OnDrawInvoke() => OnDraw?.Invoke();
    }

    // ========== ИГРА БЛЭКДЖЕК ==========
    public class BlackjackGame : CasinoGameBase
    {
        private readonly int _numberOfCards;
        private List<Card> _cards;
        private Queue<Card> _deck;

        public BlackjackGame(int numberOfCards)
        {
            if (numberOfCards <= 0)
                throw new ArgumentException("Number of cards must be positive", nameof(numberOfCards));

            _numberOfCards = numberOfCards;
        }

        protected override void FactoryMethod()
        {
            _cards = new List<Card>();
            var suits = Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>();
            var ranks = Enum.GetValues(typeof(CardRank)).Cast<CardRank>();

            int cardsCreated = 0;
            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    if (cardsCreated >= _numberOfCards) break;

                    _cards.Add(new Card(suit, rank));
                    cardsCreated++;
                }
                if (cardsCreated >= _numberOfCards) break;
            }

            Shuffle();
        }

        private void Shuffle()
        {
            var random = new Random();
            _deck = new Queue<Card>(_cards.OrderBy(c => random.Next()));
        }

        private int GetCardValue(Card card)
        {
            return card.Rank switch
            {
                CardRank.Jack or CardRank.Queen or CardRank.King => 10,
                CardRank.Ace => 11,
                _ => (int)card.Rank
            };
        }

        private int CalculateHand(List<Card> hand)
        {
            int total = hand.Sum(GetCardValue);
            int aceCount = hand.Count(c => c.Rank == CardRank.Ace);

            while (total > 21 && aceCount > 0)
            {
                total -= 10;
                aceCount--;
            }

            return total;
        }

        public override void PlayGame()
        {
            var playerHand = new List<Card> { _deck.Dequeue(), _deck.Dequeue() };
            var computerHand = new List<Card> { _deck.Dequeue(), _deck.Dequeue() };

            Console.WriteLine("\n=== Blackjack ===");
            Console.WriteLine("Your cards:");
            playerHand.ForEach(c => Console.WriteLine($"  {c}"));

            int playerScore = CalculateHand(playerHand);
            int computerScore = CalculateHand(computerHand);

            Console.WriteLine($"Your score: {playerScore}");
            Console.WriteLine($"Computer score: {computerScore}");

            // Логика по ТЗ
            if (playerScore == computerScore && playerScore < 21)
            {
                Console.WriteLine("Equal scores, drawing one more card each...");
                playerHand.Add(_deck.Dequeue());
                computerHand.Add(_deck.Dequeue());
                playerScore = CalculateHand(playerHand);
                computerScore = CalculateHand(computerHand);

                Console.WriteLine($"Your new score: {playerScore}");
                Console.WriteLine($"Computer new score: {computerScore}");
            }

            // Определение победителя по ТЗ
            bool playerWins = false;
            bool isDraw = false;

            if (playerScore <= 21 && computerScore <= 21)
            {
                if (playerScore > computerScore)
                {
                    playerWins = true;
                }
                else if (computerScore > playerScore)
                {
                    playerWins = false;
                }
                else
                {
                    isDraw = true;
                }
            }
            else if (playerScore <= 21 && computerScore > 21)
            {
                playerWins = true;
            }
            else if (computerScore <= 21 && playerScore > 21)
            {
                playerWins = false;
            }
            else // оба > 21
            {
                isDraw = true;
            }

            if (isDraw)
            {
                OnDrawInvoke();
                Console.WriteLine("It's a draw!");
            }
            else if (playerWins)
            {
                OnWinInvoke();
                Console.WriteLine("You win!");
            }
            else
            {
                OnLooseInvoke();
                Console.WriteLine("Computer wins!");
            }
        }
    }

    // ========== ИГРА В КОСТИ ==========
    public class DiceGame : CasinoGameBase
    {
        private readonly int _numberOfDice;
        private readonly int _minValue;
        private readonly int _maxValue;
        private List<Dice> _dice;

        public DiceGame(int numberOfDice, int minValue, int maxValue)
        {
            if (numberOfDice <= 0)
                throw new ArgumentException("Number of dice must be positive", nameof(numberOfDice));

            _numberOfDice = numberOfDice;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        protected override void FactoryMethod()
        {
            _dice = new List<Dice>();
            for (int i = 0; i < _numberOfDice; i++)
            {
                _dice.Add(new Dice(_minValue, _maxValue));
            }
        }

        public override void PlayGame()
        {
            Console.WriteLine("\n=== Dice Game ===");

            int playerScore = _dice.Sum(d => d.Number);
            int computerScore = _dice.Sum(d => d.Number);

            Console.WriteLine($"Your dice rolls: {string.Join(", ", _dice.Select(d => d.Number))}");
            Console.WriteLine($"Computer dice rolls: {string.Join(", ", _dice.Select(d => d.Number))}");
            Console.WriteLine($"Your total: {playerScore}");
            Console.WriteLine($"Computer total: {computerScore}");

            if (playerScore > computerScore)
            {
                OnWinInvoke();
                Console.WriteLine("You win!");
            }
            else if (computerScore > playerScore)
            {
                OnLooseInvoke();
                Console.WriteLine("Computer wins!");
            }
            else
            {
                OnDrawInvoke();
                Console.WriteLine("It's a draw!");
            }
        }
    }

    // ========== КАЗИНО ==========
    public class Casino : IGame
    {
        private PlayerProfile _player;
        private readonly ISaveLoadService<string> _saveLoadService;
        private readonly BlackjackGame _blackjackGame;
        private readonly DiceGame _diceGame;
        private const decimal MaxBank = 1000000;

        public Casino()
        {
            _saveLoadService = new FileSystemSaveLoadService("./saves");
            _blackjackGame = new BlackjackGame(36);
            _diceGame = new DiceGame(2, 1, 6);
        }

        private void LoadOrCreateProfile()
        {
            Console.WriteLine("=== Welcome to Casino! ===");

            string savedData = _saveLoadService.LoadData("profile");

            if (string.IsNullOrEmpty(savedData))
            {
                Console.WriteLine("New player detected!");
                Console.Write("Enter your name: ");
                string name = Console.ReadLine();
                _player = new PlayerProfile(name, 1000);
                Console.WriteLine($"Welcome, {_player.Name}! You have ${_player.Bank} starting bank.");
            }
            else
            {
                try
                {
                    _player = JsonSerializer.Deserialize<PlayerProfile>(savedData);
                    Console.WriteLine($"Welcome back, {_player.Name}! You have ${_player.Bank} in your bank.");
                }
                catch
                {
                    Console.WriteLine("Error loading profile. Creating new profile.");
                    Console.Write("Enter your name: ");
                    string name = Console.ReadLine();
                    _player = new PlayerProfile(name, 1000);
                }
            }
        }

        private void SaveProfile()
        {
            try
            {
                string data = JsonSerializer.Serialize(_player);
                _saveLoadService.SaveData(data, "profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
            }
        }

        public void StartGame()
        {
            LoadOrCreateProfile();

            while (_player.Bank > 0)
            {
                Console.WriteLine($"\nYour bank: ${_player.Bank}");
                Console.WriteLine("\nChoose a game:");
                Console.WriteLine("1. Blackjack");
                Console.WriteLine("2. Dice Game");
                Console.WriteLine("3. Exit");
                Console.Write("Your choice (1-3): ");

                string choice = Console.ReadLine();

                if (choice == "3")
                {
                    SaveProfile();
                    Console.WriteLine($"\nGoodbye, {_player.Name}! Your profile has been saved.");
                    return;
                }

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Invalid choice! Please enter 1, 2, or 3.");
                    continue;
                }

                Console.Write($"Enter your bet (max: ${_player.Bank}): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal bet) || !_player.CanBet(bet))
                {
                    Console.WriteLine("Invalid bet amount!");
                    continue;
                }

                bool playerWon = false;
                bool draw = false;

                // Подписка на события
                if (choice == "1")
                {
                    Action winHandler = () => playerWon = true;
                    Action looseHandler = () => playerWon = false;
                    Action drawHandler = () => draw = true;

                    _blackjackGame.OnWin += winHandler;
                    _blackjackGame.OnLoose += looseHandler;
                    _blackjackGame.OnDraw += drawHandler;

                    _blackjackGame.PlayGame();

                    // Отписка от событий
                    _blackjackGame.OnWin -= winHandler;
                    _blackjackGame.OnLoose -= looseHandler;
                    _blackjackGame.OnDraw -= drawHandler;
                }
                else
                {
                    Action winHandler = () => playerWon = true;
                    Action looseHandler = () => playerWon = false;
                    Action drawHandler = () => draw = true;

                    _diceGame.OnWin += winHandler;
                    _diceGame.OnLoose += looseHandler;
                    _diceGame.OnDraw += drawHandler;

                    _diceGame.PlayGame();

                    // Отписка от событий
                    _diceGame.OnWin -= winHandler;
                    _diceGame.OnLoose -= looseHandler;
                    _diceGame.OnDraw -= drawHandler;
                }

                // Обработка результатов
                if (draw)
                {
                    Console.WriteLine($"It's a draw! Your ${bet} is returned.");
                }
                else if (playerWon)
                {
                    Console.WriteLine($"You won ${bet}!");
                    _player.Bank += bet;

                    // Проверка максимального банка по ТЗ
                    if (_player.Bank > MaxBank)
                    {
                        Console.WriteLine($"\n⚠️  Your bank (${_player.Bank}) exceeds maximum (${MaxBank})!");
                        decimal oldBank = _player.Bank;
                        _player.Bank /= 2;
                        Console.WriteLine($"You wasted half of your bank money in casino's bar.");
                        Console.WriteLine($"Your bank reduced from ${oldBank} to ${_player.Bank}");
                    }
                }
                else
                {
                    Console.WriteLine($"You lost ${bet}!");
                    _player.Bank -= bet;
                }

                // Сохранение после каждой игры
                SaveProfile();
            }

            // Если деньги закончились
            Console.WriteLine("\nNo money? Kicked!");
            SaveProfile();
        }
    }

    // ========== ТОЧКА ВХОДА ==========
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Casino casino = new Casino();
                casino.StartGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}