using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer.Models
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        public decimal Money { get; set; }
        public decimal Bet { get; set; }
        public decimal InsuranceBet { get; set; }
        public int Points { get; set; }
        public Player(decimal money)
        {
            Hand = new List<Card>();
            Points = 0;
            Bet = 0;
            InsuranceBet = 0;
            Money = money;
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
        public bool MakeBet(decimal bet)
        {
            if (bet > Money || bet<1)
                return false;
            Money -= bet;
            Bet += bet;
            return true;
        }
        public bool MakeInsuranceBet(decimal insuranceBet)
        {
            if (insuranceBet > Money || insuranceBet <= 0)
                return false;
            Money -= insuranceBet;
            InsuranceBet += insuranceBet;
            return true;
        }
        public void ResetHand(GameStatus gameStatus)
        {
            Hand.Clear();
            Bet = 0;
            InsuranceBet = 0;
            Points = 0;
            AddToHand(gameStatus);
            AddToHand(gameStatus);
        }
        public void ResetInsuranceBet()
        {
            InsuranceBet = 0;
        }
    }
}
