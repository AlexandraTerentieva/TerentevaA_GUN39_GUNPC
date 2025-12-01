using CasinoApp.Enums;
using CasinoApp.Exceptions;
using CasinoApp.Games;
using CasinoApp.Interfaces;
using CasinoApp.Models;
using CasinoApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CasinoApp.Services
{
    public interface ISaveLoadService<T>
    {
        void SaveData(T data, string identifier);
        T LoadData(string identifier);
    }
}

namespace CasinoApp.Interfaces
{
    public interface IGame
    {
        void StartGame();
    }
}

namespace CasinoApp.Exceptions
{
    public class WrongDiceNumberException : Exception
    {
        public WrongDiceNumberException(int number, int min, int max)
            : base($"Number {number} is out of range. Allowed range: {min}-{max}")
        {
        }
    }
}

namespace CasinoApp.Enums
{
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
}

namespace CasinoApp.Models
{
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

            _min = min;
            _max = max;
            _random = new Random();
        }
    }

    [Serializable]
    public class PlayerProfile
    {
        public string Name { get; set; }
        public decimal Bank { get; set; }

        public PlayerProfile() { }

        public PlayerProfile(string name, decimal initialBank = 1000)
        {
            Name = name;
            Bank = initialBank;
        }

        public bool CanBet(decimal amount) => amount <= Bank && amount > 0;
    }
}

namespace CasinoApp.Services
{
    public class FileSystemSaveLoadService<T> : ISaveLoadService<T>
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

        public void SaveData(T data, string identifier)
        {
            try
            {
                string filePath = Path.Combine(_savePath, $"{identifier}.txt");
                string json = JsonSerializer.Serialize(data);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to save data: {ex.Message}");
            }
        }

        public T LoadData(string identifier)
        {
            try
            {
                string filePath = Path.Combine(_savePath, $"{identifier}.txt");

                if (!File.Exists(filePath))
                {
                    return default;
                }

                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to load data: {ex.Message}");
            }
        }
    }
}

namespace CasinoApp.Games
{
    public abstract class CasinoGameBase
    {
        public event Action OnWin;
        public event Action OnLoose;
        public event Action OnDraw;

        protected Queue<Card> Deck;

        protected CasinoGameBase()
        {
        }

        protected abstract void FactoryMethod();

        public abstract void PlayGame();

        protected void OnWinInvoke() => OnWin?.Invoke();
        protected void OnLooseInvoke() => OnLoose?.Invoke();
        protected void OnDrawInvoke() => OnDraw?.Invoke();

        protected void Shuffle(List<Card> cards)
        {
            var random = new Random();
            Deck = new Queue<Card>(cards.OrderBy(c => random.Next()));
        }
    }

    public class BlackjackGame : CasinoGameBase
    {
        private readonly int _numberOfCards;
        private List<Card> _cards;

        public BlackjackGame(int numberOfCards)
        {
            if (numberOfCards <= 0)
                throw new ArgumentException("Number of cards must be positive", nameof(numberOfCards));

            _numberOfCards = numberOfCards;
            FactoryMethod();
        }

        protected override void FactoryMethod()
        {
            _cards = new List<Card>();
            var suits = Enum.GetValues(typeof(CardSuit));
            var ranks = Enum.GetValues(typeof(CardRank));

            int cardsCreated = 0;
            foreach (CardSuit suit in suits)
            {
                foreach (CardRank rank in ranks)
                {
                    if (cardsCreated >= _numberOfCards) break;

                    _cards.Add(new Card(suit, rank));
                    cardsCreated++;
                }
                if (cardsCreated >= _numberOfCards) break;
            }

            Shuffle(_cards);
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
            var playerHand = new List<Card> { Deck.Dequeue(), Deck.Dequeue() };
            var computerHand = new List<Card> { Deck.Dequeue(), Deck.Dequeue() };

            Console.WriteLine("\n=== Blackjack ===");
            Console.WriteLine("Your cards:");
            playerHand.ForEach(c => Console.WriteLine($"  {c}"));

            int playerScore = CalculateHand(playerHand);
            int computerScore = CalculateHand(computerHand);

            Console.WriteLine($"Your score: {playerScore}");

            if (playerScore == computerScore && playerScore < 21)
            {
                playerHand.Add(Deck.Dequeue());
                computerHand.Add(Deck.Dequeue());
                playerScore = CalculateHand(playerHand);
                computerScore = CalculateHand(computerHand);
            }

            if ((playerScore <= 21 && computerScore > 21) ||
                (playerScore <= 21 && playerScore > computerScore))
            {
                OnWinInvoke();
            }
            else if ((computerScore <= 21 && playerScore > 21) ||
                     (computerScore <= 21 && computerScore > playerScore))
            {
                OnLooseInvoke();
            }
            else
            {
                OnDrawInvoke();
            }
        }
    }

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
            FactoryMethod();
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

            Console.WriteLine($"Your score: {playerScore}");
            Console.WriteLine($"Computer score: {computerScore}");

            if (playerScore > computerScore)
            {
                OnWinInvoke();
            }
            else if (computerScore > playerScore)
            {
                OnLooseInvoke();
            }
            else
            {
                OnDrawInvoke();
            }
        }
    }
}

namespace CasinoApp
{
    public class Casino : IGame
    {
        private PlayerProfile _player;
        private readonly ISaveLoadService<PlayerProfile> _saveLoadService;
        private readonly BlackjackGame _blackjackGame;
        private readonly DiceGame _diceGame;
        private const decimal MaxBank = 1000000;

        public Casino()
        {
            _saveLoadService = new FileSystemSaveLoadService<PlayerProfile>("./saves");

            _blackjackGame = new BlackjackGame(36);
            _diceGame = new DiceGame(2, 1, 6);
        }

        private void LoadOrCreateProfile()
        {
            Console.WriteLine("Welcome to Casino!");

            _player = _saveLoadService.LoadData("profile");

            if (_player == null)
            {
                Console.WriteLine("New player detected!");
                Console.Write("Enter your name: ");
                string name = Console.ReadLine();
                _player = new PlayerProfile(name, 1000);
                Console.WriteLine($"Welcome, {_player.Name}! You have ${_player.Bank} starting bank.");
            }
            else
            {
                Console.WriteLine($"Welcome back, {_player.Name}! You have ${_player.Bank} in your bank.");
            }
        }

        private void SaveProfile()
        {
            _saveLoadService.SaveData(_player, "profile");
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
                Console.Write("Your choice: ");

                string choice = Console.ReadLine();

                if (choice == "3")
                {
                    SaveProfile();
                    Console.WriteLine($"\nGoodbye, {_player.Name}! Your profile has been saved.");
                    return;
                }

                Console.Write($"Enter your bet (max: ${_player.Bank}): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal bet) || !_player.CanBet(bet))
                {
                    Console.WriteLine("Invalid bet amount!");
                    continue;
                }

                bool playerWon = false;
                bool draw = false;

                void OnWinHandler() { playerWon = true; }
                void OnLooseHandler() { playerWon = false; }
                void OnDrawHandler() { draw = true; }

                if (choice == "1")
                {
                    _blackjackGame.OnWin += OnWinHandler;
                    _blackjackGame.OnLoose += OnLooseHandler;
                    _blackjackGame.OnDraw += OnDrawHandler;

                    _blackjackGame.PlayGame();

                    _blackjackGame.OnWin -= OnWinHandler;
                    _blackjackGame.OnLoose -= OnLooseHandler;
                    _blackjackGame.OnDraw -= OnDrawHandler;
                }
                else if (choice == "2")
                {
                    _diceGame.OnWin += OnWinHandler;
                    _diceGame.OnLoose += OnLooseHandler;
                    _diceGame.OnDraw += OnDrawHandler;

                    _diceGame.PlayGame();

                    _diceGame.OnWin -= OnWinHandler;
                    _diceGame.OnLoose -= OnLooseHandler;
                    _diceGame.OnDraw -= OnDrawHandler;
                }
                else
                {
                    Console.WriteLine("Invalid choice!");
                    continue;
                }

                if (draw)
                {
                    Console.WriteLine("It's a draw! Your bet is returned.");
                }
                else if (playerWon)
                {
                    Console.WriteLine($"You won ${bet}!");
                    _player.Bank += bet;

                    if (_player.Bank > MaxBank)
                    {
                        decimal excess = _player.Bank - MaxBank;
                        _player.Bank = MaxBank;
                        Console.WriteLine($"\nYou broke the casino with ${excess} excess! A new casino will be built.");
                        Console.WriteLine("Your bank is now capped at $1,000,000.");
                    }
                }
                else
                {
                    Console.WriteLine($"You lost ${bet}!");
                    _player.Bank -= bet;
                }

                SaveProfile();
            }

            Console.WriteLine("\nNo money? Kicked!");
            SaveProfile();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                var casino = new Casino();
                casino.StartGame();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}