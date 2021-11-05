using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer.Models
{
    public class Dealer
    {
        public List<Card> Hand { get; set; }
        public int DealerLimit { get; set; }
        public int Points { get; set; }
        public Dealer(int dealerLimit)
        {
            Hand = new List<Card>();
            Points = 0;
            DealerLimit = dealerLimit;
        }
        public void AddToHand(GameStatus gameStatus)
        {
            Card card = gameStatus.Deck.ElementAt(0);
            gameStatus.Deck.RemoveAt(0);
            Points += card.Value;
            Hand.Add(card);
        }
        public bool SearchForAs()
        {
            for (int i = 0; i < Hand.Count; i++)
            {
                if (Hand[i].Value == 11)
                {
                    Hand[i].Value = 1;
                    Points -= 10;
                    if (Points <= 21)
                        return true;
                }
            }
            return false;
        }
        public void ResetHand(GameStatus gameStatus)
        {
            Hand.Clear();
            Points = 0;
            AddToHand(gameStatus);
            AddToHand(gameStatus);
        }
    }
}
