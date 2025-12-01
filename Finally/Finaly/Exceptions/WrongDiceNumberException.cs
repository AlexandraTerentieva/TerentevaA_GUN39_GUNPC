using System;

namespace CasinoGame.Exceptions
{
    public class WrongDiceNumberException : Exception
    {
        public WrongDiceNumberException(int number, int min, int max)
            : base($"Number {number} is out of range. Allowed range: {min}-{max}")
        {
        }
    }
}
