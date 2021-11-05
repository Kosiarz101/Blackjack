using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer.Models
{
    public static class GameManager
    {
        public static void PlayerLost(Player player, Dealer dealer, GameStatus gameStatus)
        {
            if (player.InsuranceBet > 0)
                player.Money += player.InsuranceBet * gameStatus.getInsurancePayment();
            dealer.ResetHand(gameStatus);
            player.ResetHand(gameStatus);
        }
        public static void PlayerNotLost(Player player, Dealer dealer, GameStatus gameStatus, decimal multiplier)
        {
            player.Money += multiplier * player.Bet;
            if (player.InsuranceBet > 0)
                player.Money += player.InsuranceBet * gameStatus.getInsurancePayment();
            player.ResetHand(gameStatus);
            dealer.ResetHand(gameStatus);
        }
    }
}
