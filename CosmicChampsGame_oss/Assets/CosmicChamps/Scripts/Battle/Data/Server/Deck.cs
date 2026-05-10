using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThirdParty.Extensions;

namespace CosmicChamps.Battle.Data.Server
{
    public class Deck
    {
        private readonly Queue<Card> _cardsQueue = new();
        private readonly List<Card> _cards = new();
        private Card _nextCard;

        public ReadOnlyCollection<Card> Cards => _cards.AsReadOnly ();
        public Card NextCard => _nextCard;

        public Deck (IEnumerable<Card> cards, int openedCardsCount)
        {
            var shuffledCards = new List<Card> (cards);
            shuffledCards.Shuffle (5);
            shuffledCards.ForEach (_cardsQueue.Enqueue);

            for (var i = 0; i < openedCardsCount; i++)
            {
                _cards.Add (GetNextCard ());
            }

            _nextCard = GetNextCard ();
        }

        private Card GetNextCard ()
        {
            if (_cardsQueue.Count == 0)
                throw new InvalidOperationException ("No more cards in queue");

            return _cardsQueue.Dequeue ();
        }

        public void UseCard (string cardId)
        {
            var cardIndex = _cards.FindIndex (x => x.Id == cardId);
            if (cardIndex < 0)
                throw new ArgumentException ($"Card {cardId} not found in the deck");

            _cardsQueue.Enqueue (_cards[cardIndex]);
            _cards[cardIndex] = _nextCard;

            _nextCard = GetNextCard ();
        }

        public Card GetCard (string cardId)
        {
            var card = _cards.FirstOrDefault (x => x.Id == cardId);
            if (card == null)
                throw new ArgumentException ($"Card {cardId} not found in the deck");

            return card;
        }
    }
}