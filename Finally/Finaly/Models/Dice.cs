using System;
using CasinoGame.Exceptions;

namespace CasinoGame.Models
{
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
}