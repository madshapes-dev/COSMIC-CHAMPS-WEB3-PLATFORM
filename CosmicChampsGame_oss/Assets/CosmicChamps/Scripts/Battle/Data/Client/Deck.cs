using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace CosmicChamps.Battle.Data.Client
{
    public class Deck
    {
        private readonly ReactiveCollection<Card> _cards;
        private readonly ReactiveProperty<Card> _nextCard;

        public IReadOnlyReactiveCollection<Card> Cards => _cards;
        public IReadOnlyReactiveProperty<Card> NextCard => _nextCard;

        public Deck (IEnumerable<Card> cards, Card nextCard)
        {
            _cards = new ReactiveCollection<Card> (cards);
            _nextCard = new ReactiveProperty<Card> (nextCard);
        }

        public void UpdateNextCard (Card usedCard, Card newNextCard)
        {
            var usedCardIndex = _cards.IndexOf (usedCard);
            if (usedCardIndex < 0)
                throw new InvalidOperationException ($"Card {usedCard.Id} not found in the deck");

            _cards[usedCardIndex] = _nextCard.Value;
            _nextCard.Value = newNextCard;
        }


        public Card GetCard (string cardId)
        {
            var card = _cards.FirstOrDefault (x => x.Id == cardId);
            if (card == null)
                throw new InvalidOperationException ($"Card {cardId} not found in the deck");

            return card;
        }
    }
}