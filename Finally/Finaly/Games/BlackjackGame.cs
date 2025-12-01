using System;
using System.Collections.Generic;
using System.Linq;
using CasinoGame.Models;
using CasinoGame.Models.Enums;

namespace CasinoGame.Games
{
    public class BlackjackGame : CasinoGameBase
    {
        private readonly Random _random = new Random();
        private Queue<Card> _deck;

        public BlackjackGame(int numberOfCards)
        {
            // Игнорируем numberOfCards для простоты, используем всегда 36 карт
            InitializeDeck();
        }

        protected override void FactoryMethod()
        {
            InitializeDeck();
        }

        private void InitializeDeck()
        {
            var cards = new List<Card>();
            var suits = new[] { CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Clubs, CardSuit.Spades };
            var ranks = Enum.GetValues(typeof(CardRank)).Cast<CardRank>();

            // Создаем 36 карт
            foreach (var suit in suits)
            {
                foreach (var rank in ranks)
                {
                    cards.Add(new Card(suit, rank));
                }
            }

            // Тасуем
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }

            _deck = new Queue<Card>(cards);
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

        private Card DrawCard()
        {
            if (_deck == null || _deck.Count == 0)
            {
                InitializeDeck();
            }

            return _deck.Dequeue();
        }

        public override void PlayGame()
        {
            var playerHand = new List<Card> { DrawCard(), DrawCard() };
            var computerHand = new List<Card> { DrawCard(), DrawCard() };

            Console.WriteLine("\n=== Blackjack ===");
            Console.WriteLine("Your cards:");
            foreach (var card in playerHand)
                Console.WriteLine($"  {card}");

            int playerScore = CalculateHand(playerHand);
            int computerScore = CalculateHand(computerHand);

            Console.WriteLine($"Your score: {playerScore}");
            Console.WriteLine($"Computer score: {computerScore}");

            if (playerScore == computerScore)
            {
                Console.WriteLine("Equal scores, drawing one more card each...");
                playerHand.Add(DrawCard());
                computerHand.Add(DrawCard());

                playerScore = CalculateHand(playerHand);
                computerScore = CalculateHand(computerHand);

                Console.WriteLine($"Your new score: {playerScore}");
                Console.WriteLine($"Computer new score: {computerScore}");
            }

            bool playerWins = false;
            bool isDraw = false;

            if (playerScore <= 21 && (computerScore > 21 || computerScore < playerScore))
            {
                playerWins = true;
            }
            else if (computerScore <= 21 && (playerScore > 21 || playerScore < computerScore))
            {
                playerWins = false;
            }
            else if (playerScore >= 21 && computerScore >= 21)
            {
                isDraw = true;
            }
            else if (playerScore == computerScore)
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
}