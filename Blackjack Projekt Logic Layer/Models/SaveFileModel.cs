using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer.Models
{
    public class SaveFileModel
    {       
        //Player
        public List<Card> PlayerHand { get; set; }
        public decimal Money { get; set; }
        public decimal Bet { get; set; }
        public decimal InsurannceBet { get; set; }
        public int PlayerPoints { get; set; }
        //Dealer
        public List<Card> DealerHand { get; set; }
        public int DealerPoints { get; set; }
        //GameStatus
        public List<Card> Deck { get; set; }
        public decimal InitialMoney { get; set; }
        public int DeckQuantity { get; set; }
        public int DealerLimit { get; set; }
        public int InsurancePayment { get; set; }
        public int WinPayout { get; set; }
        public int GameplayMusic { get; set; }
        // Other
        public DateTime SaveTime { get; set; }
    }
}
