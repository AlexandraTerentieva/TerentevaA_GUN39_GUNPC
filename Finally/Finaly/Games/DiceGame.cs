using System;
using System.Collections.Generic;
using System.Linq;
using CasinoGame.Models;

namespace CasinoGame.Games
{
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

            var playerDice = new List<int>();
            var computerDice = new List<int>();

            for (int i = 0; i < _numberOfDice; i++)
            {
                playerDice.Add(new Dice(_minValue, _maxValue).Number);
                computerDice.Add(new Dice(_minValue, _maxValue).Number);
            }

            int playerScore = playerDice.Sum();
            int computerScore = computerDice.Sum();

            Console.WriteLine($"Your dice rolls: {string.Join(", ", playerDice)}");
            Console.WriteLine($"Computer dice rolls: {string.Join(", ", computerDice)}");
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
}