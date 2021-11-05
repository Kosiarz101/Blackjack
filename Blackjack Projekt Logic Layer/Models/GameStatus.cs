using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer.Models
{
    public class GameStatus
    {
        public List<Card> Deck { get; set; }
        public string[] AvailableDealerLimits { get; } = { "16", "17", "18" };
        public string[] AvailableGameplayMusic { get; } = { "GameplayClassicalTheme.wav", "GameplayRockTheme.wav", "GameplayTrapTheme.wav", "None" };
        public string[] AvailableInsurancePayments { get; } = { "1,5:1", "2:1", "2,5:1" };
        public string[] AvailableWinPayouts { get; } = { "1,5:1", "2:1", "2,5:1", "3:1" };
        public string[] AvailableDeckQuantities { get; } = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        public decimal InitialMoney { get; set; } = 1000;
        public bool isGameEnded { get; set; } = false;
        private int deckQuantity = 0;
        private int dealerLimit = 0;
        private int gameplayMusic = 0;
        private int insurancePayment = 0;
        private int winPayout = 0;
        public void ChangeOptionSettings(int dealerLimit, int insurancePayment, int winPayout, int deckQuantity, int gameplayMusic)
        {
            this.dealerLimit = dealerLimit;
            this.insurancePayment = insurancePayment;
            this.winPayout = winPayout;
            this.deckQuantity = deckQuantity;
            this.gameplayMusic = gameplayMusic;
        } 
        public int getGameplayMusicIndex()
        {
            return gameplayMusic;
        }
        public string getGameplayMusic()
        {
            return AvailableGameplayMusic[gameplayMusic];
        }
        public int getDealerLimit()
        {
            return Int32.Parse(AvailableDealerLimits[dealerLimit]);
        }
        public int getDealerLimitIndex()
        {
            return dealerLimit;
        }
        public int getInsurancePaymentIndex()
        {
            return insurancePayment;
        }
        public decimal getInsurancePayment()
        {
            string[] splitstr = AvailableInsurancePayments[insurancePayment].Split(':');
            return Decimal.Parse(splitstr[0], new NumberFormatInfo { NumberDecimalSeparator = "," });
        }
        public int getWinPayoutIndex()
        {
            return winPayout;
        }
        public decimal getWinPayout()
        {
            string[] splitstr = AvailableWinPayouts[winPayout].Split(':');
            return Decimal.Parse(splitstr[0], new NumberFormatInfo { NumberDecimalSeparator = "," });
        }
        public int getDeckQuantityIndex()
        {
            return deckQuantity;
        }
        public int getDeckQuantity()
        {
            return Int32.Parse(AvailableDeckQuantities[deckQuantity]);
        }
    }
}
