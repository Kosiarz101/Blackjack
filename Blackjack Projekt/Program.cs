using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Blackjack_Projekt_Logic_Layer;
using Blackjack_Projekt_Logic_Layer.Models;

namespace Blackjack_Projekt
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
            Menu menu = new Menu();
            GameStatus gameStatus = new GameStatus();

            menu.ShowTitle();
            TitleMenu(menu, gameStatus);
            menu.ShowTitle();
        }
        public static void MainGame(Menu menu, GameStatus gameStatus, Player player, Dealer dealer)
        {
            AudioPlayer audioPlayer = new AudioPlayer(gameStatus.getGameplayMusic());
            //Turn on gameplay music if any track was chosen in the options menu
            if(gameStatus.getGameplayMusic() != "None")
                audioPlayer.StartMusicLooping();

            if (player.Bet == 0)
                MakeBet(menu, gameStatus, player);         
            bool isRoundEnded = false;           

            while (true)
            {             
                List<string> options = new List<string>() { "Hit", "Stand", "Double" };
                string message = null;
                ConsoleColor consoleColor = ConsoleColor.White;
                ConsoleKey keyEntered = ConsoleKey.A;
                int selectedOption = 0;

                //Insurance Condition
                if (dealer.Hand[0].Name == "as" && player.InsuranceBet == 0)
                {
                    message = "Dealer's first card is as. You can buy insurance!";
                    consoleColor = ConsoleColor.Gray;
                    options.Add("Insurance");
                }

                do
                {
                    Console.Clear();
                    try
                    {
                        isRoundEnded = CheckState(menu, gameStatus, player, dealer);
                    }
                    catch(ArgumentOutOfRangeException e)
                    {
                        EndGame(menu, gameStatus, player);
                        AppManager.SaveToRanking(gameStatus, player);
                    }
                    if (isRoundEnded)
                        break;

                    menu.ShowGameplayTitle(player, gameStatus);
                    menu.ShowCardsHidden(player, dealer);                       

                    if (message != null)
                        menu.WriteLineCenter(message, consoleColor);
                    menu.ShowMenuOptions(selectedOption, options);
                    Console.Write("\t Press S to save changes");
                    menu.WriteLineRight("Press ESC to Exit          ");
                    keyEntered = menu.ReadKey();

                   
                    if (keyEntered == ConsoleKey.S)
                        SaveGame(menu, gameStatus, player, dealer);
                    selectedOption = menu.CheckArrowKeyVertical(keyEntered, selectedOption, options);

                } while (keyEntered != ConsoleKey.Enter && keyEntered != ConsoleKey.Escape);  
                
                if(keyEntered == ConsoleKey.Escape)
                {
                    if (ExitGame(menu, gameStatus, player, dealer))
                    {
                        audioPlayer.StopMusic();
                        AppManager.SaveToRanking(gameStatus, player);
                    }                
                    return;
                }
               
                if(!isRoundEnded)
                {
                    switch (selectedOption)
                    {
                        case 0:
                            try
                            {
                                player.AddToHand(gameStatus);
                                isRoundEnded = CheckState(menu, gameStatus, player, dealer);
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                EndGame(menu, gameStatus, player);
                            }                           
                            break;
                        case 1:
                            bool isPlayerBlackjack = player.Points == 21 ? true : false;
                            try
                            {
                                isRoundEnded = DealerMove(menu, gameStatus, player, dealer, isPlayerBlackjack);
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                EndGame(menu, gameStatus, player);
                            }                           
                            break;
                        case 2:
                            try
                            {
                                player.AddToHand(gameStatus);
                                if (!player.MakeBet(player.Bet))
                                {
                                    Console.WriteLine("You don't have enough money to double the bet");
                                }
                                isRoundEnded = CheckState(menu, gameStatus, player, dealer);
                                if (!isRoundEnded)
                                {
                                    isRoundEnded = DealerMove(menu, gameStatus, player, dealer, false);
                                }
                            }
                            catch (ArgumentOutOfRangeException e)
                            {
                                EndGame(menu, gameStatus, player);
                            }
                            break;
                        case 3:
                            if(options.Count > 3)
                            {                              
                                MakeInsuranceBet(menu, gameStatus, player);                              
                            }
                            break;
                    }
                }
                if(gameStatus.isGameEnded)
                {
                    gameStatus.isGameEnded = false;
                    return;
                }
                if(isRoundEnded)
                {                   
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    if (gameStatus.getGameplayMusic() != "None")
                        audioPlayer.StartMusicLooping();
                    MakeBet(menu, gameStatus, player);
                    isRoundEnded = false;
                }
            }
        }
        public static bool CheckState(Menu menu, GameStatus gameStatus, Player player, Dealer dealer)
        {
            //Player Lost
            if(player.Points > 21)
            {
                if (player.SearchForAs())
                    return false;
                player.ResetInsuranceBet();
                Console.Clear();
                menu.ShowGameplayTitle(player, gameStatus);
                menu.ShowCards(player, dealer);
                GameManager.PlayerLost(player, dealer, gameStatus);              
                menu.ShowLostScreen();
                return true;
            }
            else if(player.Points == 21)
            {
                DealerMove(menu, gameStatus, player, dealer, true);
                return true;
            }
            return false;
        }
        public static bool DealerMove(Menu menu, GameStatus gameStatus, Player player, Dealer dealer, bool isPlayerBlackjack)
        {
            Console.Clear();
            menu.ShowGameplayTitle(player, gameStatus);
            menu.ShowCards(player, dealer);
            System.Threading.Thread.Sleep(500);
            //Change as value if necessary
            if (dealer.Points > 21)
                dealer.SearchForAs();
            //check insurance requirment
            if (dealer.Hand[1].Value != 10)
                player.ResetInsuranceBet();

            //Player Lost
            if (dealer.Points > player.Points)
            {
                GameManager.PlayerLost(player, dealer, gameStatus);
                menu.ShowLostScreen();
            }
            //Draw
            else if (dealer.Points == player.Points && dealer.Points > dealer.DealerLimit)
            {
                GameManager.PlayerNotLost(player, dealer, gameStatus, 1);
                menu.ShowDrawScreen();
            }
            else
            {
                while (dealer.Points <= dealer.DealerLimit && dealer.Points <= player.Points)
                {
                    dealer.AddToHand(gameStatus);
                    Console.Clear();
                    if (dealer.Points > 21)
                        dealer.SearchForAs();
                    menu.ShowGameplayTitle(player, gameStatus);
                    menu.ShowCards(player, dealer);
                    System.Threading.Thread.Sleep(500);                    
                }
                CheckFinalState(menu, gameStatus, player, dealer, isPlayerBlackjack);
            }           
            return true;
        }
        public static void CheckFinalState(Menu menu, GameStatus gameStatus, Player player, Dealer dealer, bool isPlayerBlackjack)
        {
            int multiplier = isPlayerBlackjack ? 2 : 1;

            try
            {
                //Player Win
                if (dealer.Points > 21 || dealer.Points < player.Points)
                {
                    GameManager.PlayerNotLost(player, dealer, gameStatus, gameStatus.getWinPayout() * multiplier);
                    if (isPlayerBlackjack)
                        menu.ShowBlackjackScreen();
                    else
                        menu.ShowWinScreen();
                }
                //Player Lost
                else if (dealer.Points <= 21 && dealer.Points > player.Points)
                {
                    GameManager.PlayerLost(player, dealer, gameStatus);
                    menu.ShowLostScreen();
                }
                //Draw
                else if (dealer.Points == player.Points)
                {
                    GameManager.PlayerNotLost(player, dealer, gameStatus, multiplier);
                    if (isPlayerBlackjack)
                        menu.ShowBlackjackDrawScreen();
                    else
                        menu.ShowDrawScreen();
                }
            } catch (ArgumentOutOfRangeException e)
            {
                EndGame(menu, gameStatus, player);
            }
                                                           
        }
        public static void MakeBet(Menu menu, GameStatus gameStatus, Player player)
        {
            string[] messages = { "", "You need enter a number\n", "Your bet must be greater than zero and lower than your accumulated money\n" };
            int messageIndex = 0;

            while (true)
            {
                Console.Clear();

                menu.ShowGameplayTitle(player, gameStatus);
                menu.WriteLineCenter(messages[messageIndex]);
                menu.WriteLineCenter("Enter your bet: ");
                Console.SetCursorPosition((Console.WindowWidth - "Enter your bet: ".Length + 1) / 2, Console.CursorTop);

                if (!decimal.TryParse(Console.ReadLine(), out decimal result))
                {
                    messageIndex = 1;
                    continue;
                }

                if (!player.MakeBet(result))
                {
                    messageIndex = 2;
                    continue;
                }
                break;
            }
        }
        public static void MakeInsuranceBet(Menu menu, GameStatus gameStatus, Player player)
        {
            string[] messages = { "", "You need enter a number\n", "Your bet must be greater than zero and lower than your accumulated money\n" };
            int messageIndex = 0;

            while (true)
            {
                Console.Clear();

                menu.ShowGameplayTitle(player, gameStatus);
                Console.WriteLine(messages[messageIndex]);
                Console.WriteLine("Enter your bet: ");

                if (!decimal.TryParse(Console.ReadLine(), out decimal result))
                {
                    messageIndex = 1;
                    continue;
                }

                if (!player.MakeInsuranceBet(result))
                {
                    messageIndex = 2;
                    continue;
                }
                break;
            }
        }
        public static void EndGame(Menu menu, GameStatus gameStatus, Player player)
        {
            menu.WriteLineCenter("Deck is over. Hope you enjoy playing :)\n");
            gameStatus.isGameEnded = true;
            AppManager.SaveToRanking(gameStatus, player);
            Console.ReadKey();
            menu.WriteLineCenter("Press any key to continue");
        }
        public static void SaveGame(Menu menu, GameStatus gameStatus, Player player, Dealer dealer)
        {
            string[] dates = AppManager.GetSaveDates();
            string[] options = { "Save Slot 1", "Save Slot 2", "Save Slot 3" };           
            int selectedOption = 0;
            int cursorPosition;
            ConsoleKey keyEntered;

            while (true)
            {
                Console.Clear();
                string[] optionsDates = { $"{dates[0]}", $"{dates[1]}", $"{dates[2]}" };

                menu.ShowGameplayTitle(player, gameStatus);
                cursorPosition = Console.CursorTop;
                do
                {
                    Console.SetCursorPosition(Console.CursorLeft, cursorPosition);
                    menu.ShowSaveFileOptions(selectedOption, options, optionsDates);

                    keyEntered = menu.ReadKey();
                    selectedOption = menu.CheckArrowKeyVertical(keyEntered, selectedOption, options);

                } while (keyEntered != ConsoleKey.Enter && keyEntered != ConsoleKey.Escape);

                if (keyEntered == ConsoleKey.Escape)
                    return;

                switch (selectedOption)
                {
                    case 0:
                        Console.Clear();
                        AppManager.SaveFile(gameStatus, player, dealer, 0);
                        break;
                    case 1:
                        Console.Clear();
                        AppManager.SaveFile(gameStatus, player, dealer, 1);
                        break;
                    case 2:
                        Console.Clear();
                        AppManager.SaveFile(gameStatus, player, dealer, 2);
                        break;

                }
            }
        }
        public static void TitleMenu(Menu menu, GameStatus gameStatus)
        {
            AudioPlayer mainTheme = new AudioPlayer("MainTheme.wav");
            mainTheme.StartMusic();
            string[] options = { "Play", "Load", "Options", "Ranking", "Credits", "Exit" };
            int selectedOption = 0;
            int cursorPosition = Console.CursorTop;
            ConsoleKey keyEntered;         

            while (true)
            {
                if(!mainTheme.IsPlaying)
                    mainTheme.StartMusic();
                
                Console.Clear();
                menu.ShowTitle();
                cursorPosition = Console.CursorTop;

                List<Card> deck = AppManager.CreateDeck(gameStatus.getDeckQuantity());
                gameStatus.Deck = deck;
                Player player = AppManager.CreatePlayer(gameStatus);
                Dealer dealer = AppManager.CreateDealer(gameStatus);

                do
                {
                    Console.SetCursorPosition(Console.CursorLeft, cursorPosition);
                    menu.ShowMenuOptions(selectedOption, options);

                    keyEntered = menu.ReadKey();
                    selectedOption = menu.CheckArrowKeyVertical(keyEntered, selectedOption, options);

                } while (keyEntered != ConsoleKey.Enter);

                switch (selectedOption)
                {
                    case 0:
                        Console.Clear();
                        mainTheme.StopMusic();
                        MainGame(menu, gameStatus, player, dealer);
                        break;
                    case 1:
                        Console.Clear();
                        LoadGame(menu, gameStatus, player, dealer, mainTheme);
                        break;
                    case 2:
                        Console.Clear();
                        Options(menu, gameStatus);
                        break;
                    case 3:
                        Console.Clear();
                        LoadRanking(menu);
                        break;
                    case 4:
                        Console.Clear();
                        menu.ShowCredits();
                        Console.ReadKey();
                        break;
                    case 5:
                        Console.Clear();
                        Exit(menu);
                        break;


                }
            }           
        }
        public static void LoadGame(Menu menu, GameStatus gameStatus, Player player, Dealer dealer, AudioPlayer mainTheme = null)
        {
            string[] dates = AppManager.GetSaveDates();
            string[] options = { "Save Slot 1", "Save Slot 2", "Save Slot 3" };
            bool isChosen = false;
            int selectedOption = 0;
            int cursorPosition = Console.CursorTop;
            ConsoleKey keyEntered;

            while (!isChosen)
            {
                string[] optionsDates = { $"{dates[0]}", $"{dates[1]}", $"{dates[2]}" };

                menu.ShowTitle();
                cursorPosition = Console.CursorTop;
                do
                {
                    Console.SetCursorPosition(Console.CursorLeft, cursorPosition);
                    menu.ShowSaveFileOptions(selectedOption, options, optionsDates);

                    keyEntered = menu.ReadKey();
                    selectedOption = menu.CheckArrowKeyVertical(keyEntered, selectedOption, options);

                } while (keyEntered != ConsoleKey.Enter && keyEntered != ConsoleKey.Escape);

                if (keyEntered == ConsoleKey.Escape)
                    return;

                if (mainTheme != null && mainTheme.IsPlaying)
                    mainTheme.StopMusic();
                switch (selectedOption)
                {                  
                    case 0:
                        Console.Clear();
                        AppManager.LoadFile(gameStatus, player, dealer, 0);
                        MainGame(menu, gameStatus, player, dealer);
                        return;
                    case 1:
                        Console.Clear();
                        AppManager.LoadFile(gameStatus, player, dealer, 1);
                        MainGame(menu, gameStatus, player, dealer);
                        return;
                    case 2:
                        Console.Clear();
                        AppManager.LoadFile(gameStatus, player, dealer, 2);
                        MainGame(menu, gameStatus, player, dealer);
                        return;

                }
            }           
        }
        public static bool ExitGame(Menu menu, GameStatus gameStatus, Player player, Dealer dealer)
        {
            string[] options = { "yes", "no" };
            int selectedOption = 0;
            ConsoleKey keyEntered;

            do
            {
                Console.Clear();
                Console.SetCursorPosition(Console.CursorLeft, 0);
                menu.ShowTitle();
                menu.WriteLineCenter("Do you want to save your current progress?");
                menu.ShowMenuOptions(selectedOption, options);

                keyEntered = menu.ReadKey();
                selectedOption = menu.CheckArrowKeyVertical(keyEntered, selectedOption, options);

            } while (keyEntered != ConsoleKey.Enter && keyEntered != ConsoleKey.Escape);

            if (keyEntered == ConsoleKey.Escape)
            {
                return false;
            }

            switch (selectedOption)
            {
                case 0:
                    SaveGame(menu, gameStatus, player, dealer);
                    return true;
                case 1:
                    return true;
            }
            return false;
        }
        public static void Options(Menu menu, GameStatus gameStatus)
        {
            int selectedOptionVertical = 0;
            int selectedOptionDealerLimits = gameStatus.getDealerLimitIndex();
            int selectedOptionInsurancePayments = gameStatus.getInsurancePaymentIndex();
            int selectedOptionWinPayouts = gameStatus.getWinPayoutIndex();
            int selectedOptionDeckQuantity = gameStatus.getDeckQuantityIndex();
            int selectedOptionGameplayMusic = gameStatus.getGameplayMusicIndex();
            decimal initialMoney = gameStatus.InitialMoney;
            ConsoleKey keyEntered;

            string[] options = { "Dealer Limit", "Insurance Price", "Win Payout", "Initial Money", "Deck Quantity", "Gameplay Music" };
            string[] tips = { "The limit above which dealer won't hit", "Ratio of money which player gets when player pay insurance and dealer gets blackjack",
                              "Ratio of money which player gets when player wins", "The value of initial money. PRESS E TO EDIT", "The number of decks that will be played",
                              "Music that will be played during gameplay. PRESS E TO HEAR PREVIEW"};
            do
            {
                string[] optionsToChose = {
                $"< {gameStatus.AvailableDealerLimits[selectedOptionDealerLimits]} >",
                $"< {gameStatus.AvailableInsurancePayments[selectedOptionInsurancePayments]} >",
                $"< {gameStatus.AvailableWinPayouts[selectedOptionWinPayouts]} >",
                $"0 <= {initialMoney} <= 100 000",
                $"< {gameStatus.AvailableDeckQuantities[selectedOptionDeckQuantity]} >",
                $"< {gameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic]} >"
                };

                Console.Clear();
                menu.ShowTitle();
                menu.ShowMenuOptions(selectedOptionVertical, options, optionsToChose, tips);
                Console.Write("\t Press Enter to save changes");
                menu.WriteLineRight("Press ESC to discard changes          ");

                keyEntered = menu.ReadKey();
                selectedOptionVertical = menu.CheckArrowKeyVertical(keyEntered, selectedOptionVertical, options);
                switch (selectedOptionVertical)
                {
                    case 0:
                        selectedOptionDealerLimits = menu.CheckArrowKeyHorizontal(keyEntered, selectedOptionDealerLimits, gameStatus.AvailableDealerLimits);
                        break;
                    case 1:
                        selectedOptionInsurancePayments = menu.CheckArrowKeyHorizontal(keyEntered, selectedOptionInsurancePayments, gameStatus.AvailableInsurancePayments);
                        break;
                    case 2:
                        selectedOptionWinPayouts = menu.CheckArrowKeyHorizontal(keyEntered, selectedOptionWinPayouts, gameStatus.AvailableWinPayouts);
                        break;
                    case 3:
                        if (keyEntered == ConsoleKey.E)
                        {
                            Console.SetCursorPosition(0, Console.CursorTop - 8);
                            menu.ClearOneLine(100);
                            Console.SetCursorPosition((Console.WindowWidth - optionsToChose[3].Length + 1) / 2, Console.CursorTop);
                            string initMoneyTemp = Console.ReadLine();
                            if (!Int32.TryParse(initMoneyTemp, out int result))
                            {
                                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                                menu.ClearOneLine();
                                Console.ForegroundColor = ConsoleColor.Red;
                                menu.WriteLineCenter("Initial money must be an integer number");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.ReadKey();
                            }
                            else if (result < 0 || result > 100000)
                            {
                                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                                menu.ClearOneLine();
                                Console.ForegroundColor = ConsoleColor.Red;
                                menu.WriteLineCenter("Initial money must be less than 100 000 and greater than 0");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.ReadKey();
                            }
                            else
                                initialMoney = result;
                        }
                        break;
                    case 4:
                        selectedOptionDeckQuantity = menu.CheckArrowKeyHorizontal(keyEntered, selectedOptionDeckQuantity, gameStatus.AvailableDeckQuantities);                    
                        break;
                    case 5:
                        selectedOptionGameplayMusic = menu.CheckArrowKeyHorizontal(keyEntered, selectedOptionGameplayMusic, gameStatus.AvailableGameplayMusic);
                        if (keyEntered == ConsoleKey.E && "None" != gameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic])
                        {
                            AudioPlayer audioPlayer = new AudioPlayer(gameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic]);
                            audioPlayer.StartMusic();
                            menu.WriteLineCenter("Press any key to stop\n");
                            Console.ReadKey();
                            audioPlayer.StartDifferentMusic("MainTheme.wav");
                        }
                        break;
                }

            } while (keyEntered != ConsoleKey.Enter && keyEntered != ConsoleKey.Escape);

            if (keyEntered == ConsoleKey.Escape)
                return;

            int dealerLimit = selectedOptionDealerLimits;
            int insurancePayment = selectedOptionInsurancePayments;
            int winPayout = selectedOptionWinPayouts;
            int deckQuantity = selectedOptionDeckQuantity;
            int gameplayMusic = selectedOptionGameplayMusic;
            gameStatus.InitialMoney = initialMoney;
            gameStatus.ChangeOptionSettings(dealerLimit, insurancePayment, winPayout, deckQuantity, gameplayMusic);

        }
        public static void LoadRanking(Menu menu)
        {
            ConsoleKey consoleKey;
            do
            {
                Console.Clear();
                List<RankingModel> rankings = AppManager.LoadFromRanking();
                rankings = rankings.OrderByDescending(x => x.MoneyEarned).ToList();
                menu.ShowTitle();
                if (rankings.Count > 0)
                {
                    foreach (RankingModel ranking in rankings)
                    {
                        menu.WriteLineCenter("Money Earned: " + ranking.MoneyEarned.ToString());
                        menu.WriteLineCenter("Initial Deck: " + ranking.InitialDeck.ToString());
                        menu.WriteLineCenter("Day of Play: " + ranking.CreationDate.ToString("G"));
                        menu.WriteLineCenter("");
                    }
                }
                else
                {
                    menu.WriteLineCenter("You haven't play any round yet");
                }
                consoleKey = menu.ReadKey();
            } while (consoleKey != ConsoleKey.Escape);                       
        }
        public static void Exit(Menu menu)
        {
            string[] options = { "yes", "no" };
            int selectedOption = 0;
            ConsoleKey keyEntered;

            do
            {
                Console.SetCursorPosition(Console.CursorLeft, 0);
                menu.ShowTitle();
                menu.WriteLineCenter("Are you sure you want to exit?");
                menu.ShowMenuOptions(selectedOption, options);

                keyEntered = menu.ReadKey();
                selectedOption = menu.CheckArrowKeyVertical(keyEntered, selectedOption, options);

            } while (keyEntered != ConsoleKey.Enter && keyEntered != ConsoleKey.Escape);

            if (keyEntered == ConsoleKey.Escape)
            {
                return;
            }

            switch (selectedOption)
            {
                case 0:
                    Environment.Exit(0);
                    break;
                case 1:
                    return;
            }
        }
    }
        
    
}
