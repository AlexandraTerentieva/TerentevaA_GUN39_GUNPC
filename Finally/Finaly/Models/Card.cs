using CasinoGame.Models.Enums;

namespace CasinoGame.Models
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
}