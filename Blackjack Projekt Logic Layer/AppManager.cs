using Blackjack_Projekt_Logic_Layer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack_Projekt_Logic_Layer
{
    public class AppManager
    {
        public static List<Card> CreateDeck(int quantity)
        {
            Random random = new Random();

            string[] cardList = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "jack", "queen", "king", "as" };
            string[] cardListDeck = new string[cardList.Length * quantity * 4];
            int j = cardList.Length - 1;

            for (int i = 0; i < cardListDeck.Length; i++)
            {
                cardListDeck[i] = cardList[j];
                j++;
                if (j > cardList.Length - 1)
                    j = 0;
            }

            j = Int32.Parse(cardList[0]);
            Dictionary<string, int> cards = new Dictionary<string, int>();

            for (int i = 0; i < cardList.Length; i++)
            {
                cards.Add(cardList[i], j);
                if (j < 10)
                    j++;
            }
            cards["as"] = 11;

            List<Card> deck = new List<Card>();

            for (int i = cardListDeck.Length - 1; i > 0; i--)
            {
                int randomInteger = random.Next(0, i - 1);
                Card card = new Card();
                card.Name = cardListDeck[randomInteger];
                card.Value = cards[cardListDeck[randomInteger]];
                deck.Add(card);

                string temp = cardListDeck[randomInteger];
                cardListDeck[randomInteger] = cardListDeck[i];
                cardListDeck[i] = temp;
            }
            Card lastCard = new Card();
            lastCard.Name = cardListDeck[0];
            lastCard.Value = cards[cardListDeck[0]];
            deck.Add(lastCard);

            return deck;
        }
        public static Player CreatePlayer(GameStatus gameStatus)
        {
            Player player = new Player(gameStatus.InitialMoney);
            player.AddToHand(gameStatus);
            player.AddToHand(gameStatus);
            return player;
        }
        public static Dealer CreateDealer(GameStatus gameStatus)
        {
            Dealer dealer = new Dealer(gameStatus.getDealerLimit());
            dealer.AddToHand(gameStatus);
            dealer.AddToHand(gameStatus);
            return dealer;
        }
        public static void SaveFile(GameStatus gameStatus, Player player, Dealer dealer, int saveSlot)
        {
            SaveFileModel saveFile = new SaveFileModel()
            {
                //Player
                Bet = player.Bet,
                InsurannceBet = player.InsuranceBet,
                PlayerHand = player.Hand,
                PlayerPoints = player.Points,
                Money = player.Money,
                //Dealer
                DealerHand = dealer.Hand,
                DealerPoints = dealer.Points,
                //GameStatus
                Deck = gameStatus.Deck,
                InitialMoney = gameStatus.InitialMoney,
                DealerLimit = gameStatus.getDealerLimitIndex(),
                DeckQuantity = gameStatus.getDeckQuantityIndex(),
                InsurancePayment = gameStatus.getInsurancePaymentIndex(),
                WinPayout = gameStatus.getWinPayoutIndex(),
                GameplayMusic = gameStatus.getGameplayMusicIndex(),
                //Other
                SaveTime = DateTime.Now
            };           
            string saveData = JsonConvert.SerializeObject(saveFile, Formatting.Indented);
            switch (saveSlot)
            {
                case 0:
                    File.WriteAllText(getPath("SaveFiles/saveSlot1.json"), saveData);
                    break;
                case 1:
                    File.WriteAllText(getPath("SaveFiles/saveSlot2.json"), saveData);
                    break;
                case 2:
                    File.WriteAllText(getPath("SaveFiles/saveSlot3.json"), saveData);
                    break;

            }
            //player

        }
        public static void LoadFile(GameStatus gameStatus, Player player, Dealer dealer, int saveSlot)
        {
            string dataFromFile = "";
            switch (saveSlot)
            {
                case 0:
                    dataFromFile = File.ReadAllText(getPath("SaveFiles/saveSlot1.json"));
                    break;
                case 1:
                    dataFromFile = File.ReadAllText(getPath("SaveFiles/saveSlot2.json"));
                    break;
                case 2:
                    dataFromFile = File.ReadAllText(getPath("SaveFiles/saveSlot3.json"));
                    break;

            }
            SaveFileModel saveFile = JsonConvert.DeserializeObject<SaveFileModel>(dataFromFile);

            //Player
            player.Bet = saveFile.Bet;
            player.InsuranceBet = saveFile.InsurannceBet;
            player.Hand = saveFile.PlayerHand;
            player.Points = saveFile.PlayerPoints;
            player.Money = saveFile.Money;
            //Dealer
            dealer.Hand = saveFile.DealerHand;
            dealer.Points = saveFile.DealerPoints;
            //GameStatus
            gameStatus.Deck = saveFile.Deck;
            gameStatus.InitialMoney = saveFile.InitialMoney;
            gameStatus.ChangeOptionSettings(saveFile.DealerLimit, saveFile.InsurancePayment, saveFile.WinPayout, saveFile.DeckQuantity, saveFile.GameplayMusic);
            string saveData = JsonConvert.SerializeObject(saveFile, Formatting.Indented);
        }
        public static string[] GetSaveDates()
        {
            string pathJson = getPath("SaveFiles/saveSlot");
            string[] dates = new string[3];
            for(int i=0; i<3; i++)
            {
                dates[i] = File.GetLastWriteTime(pathJson + (i+1)+".json").ToString("G");
            }
            return dates;
        }
        public static void SaveToRanking(GameStatus gameStatus, Player player)
        {
            List<RankingModel> rankings = new List<RankingModel>();          
            string pathJson = getPath("RankingFile/Ranking.json");

            rankings = isAnyEntry(pathJson);

            RankingModel ranking = new RankingModel()
            {
                InitialDeck = gameStatus.getDeckQuantity(),
                MoneyEarned = player.Money - gameStatus.InitialMoney,
                CreationDate = DateTime.Now
            };

            rankings.Add(ranking);
            string rankingsSerialized = JsonConvert.SerializeObject(rankings);
            File.WriteAllText(pathJson, rankingsSerialized);
        }
        public static List<RankingModel> LoadFromRanking()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string pathJson = getPath("RankingFile/Ranking.json");
            List<RankingModel> rankings = isAnyEntry(pathJson);
            return rankings;
        }
        private static List<RankingModel> isAnyEntry(string pathJson)
        {
            List<string> errors = new List<string>();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings()
            {
                Error = (sender, args) => { args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error,
                Formatting = Formatting.Indented
                
            };
            List<RankingModel> rankings = new List<RankingModel>();
            rankings = JsonConvert.DeserializeObject<List<RankingModel>>(File.ReadAllText(pathJson), jsonSettings);
            return rankings;
        }
        public static string getPath(string filePath)
        {
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(appPath, filePath);
        }
    }
}
