using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CasinoGame.Models
{
    public class PlayerProfile
    {
        public string Name { get; set; }
        public decimal Bank { get; set; }
        private const decimal MaxBank = 1000000;

        [JsonConstructor]
        public PlayerProfile() { }

        public PlayerProfile(string name, decimal initialBank = 1000)
        {
            Name = name;
            Bank = Math.Min(initialBank, MaxBank);  // Исправлено: Math.Min, а не Math.Hin
        }

        public bool CanBet(decimal amount) => amount <= Bank && amount > 0;

        public void Win(decimal amount)
        {
            Bank += amount;

            if (Bank > MaxBank)
            {
                Console.WriteLine("You broke the casino! A new one will be built.");
                Bank = Bank / 2;  // Уменьшаем вдвое
                Console.WriteLine("You wasted half of your bank money in casino's bar");
            }
        }

        public void Lose(decimal amount)
        {
            Bank -= amount;
            if (Bank < 0) Bank = 0;
        }
    }
}