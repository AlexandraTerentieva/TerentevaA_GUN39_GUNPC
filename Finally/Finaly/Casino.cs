using System;
using System.Text.Json;
using CasinoGame.Games;
using CasinoGame.Interfaces;
using CasinoGame.Models;
using CasinoGame.Services;

namespace CasinoGame
{
    public class Casino : IGame
    {
        private PlayerProfile _player;
        private readonly ISaveLoadService<string> _saveLoadService;
        private readonly BlackjackGame _blackjackGame;
        private readonly DiceGame _diceGame;

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
                string name = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(name))
                    name = "Player";

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
                    string name = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(name))
                        name = "Player";

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

                if (choice == "1")
                {
                    Action winHandler = () => playerWon = true;
                    Action looseHandler = () => playerWon = false;
                    Action drawHandler = () => draw = true;

                    _blackjackGame.OnWin += winHandler;
                    _blackjackGame.OnLoose += looseHandler;
                    _blackjackGame.OnDraw += drawHandler;

                    _blackjackGame.PlayGame();

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

                    _diceGame.OnWin -= winHandler;
                    _diceGame.OnLoose -= looseHandler;
                    _diceGame.OnDraw -= drawHandler;
                }

                if (draw)
                {
                    Console.WriteLine($"It's a draw! Your ${bet} is returned.");
                }
                else if (playerWon)
                {
                    Console.WriteLine($"You won ${bet}!");
                    _player.Win(bet);
                }
                else
                {
                    Console.WriteLine($"You lost ${bet}!");
                    _player.Lose(bet);
                }

                SaveProfile();
            }

            Console.WriteLine("\nNo money? Kicked!");
            SaveProfile();
        }
    }
}