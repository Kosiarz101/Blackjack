using Blackjack_Projekt_Logic_Layer.Models;
using ConsoleTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blackjack_Projekt_Logic_Layer;

namespace Blackjack_Projekt
{
    class Menu
    {
        public void ShowMenuOptions(string[] options)
        {
            for(int i=0; i<options.Length; i++)
            {
                Console.WriteLine($"{options[i]}");
            }
        }
        public void ClearOneLine(int n = 100)
        {
            string cleaner = "";
            for(int i=0; i< n; i++)
            {
                cleaner += " ";
            }
            Console.Write(cleaner);
            Console.SetCursorPosition(0, Console.CursorTop);
        }
        public void ShowGameplayTitle(Player player, GameStatus gameStatus)
        {
            string count = $"   Your Insurance Bet: {player.InsuranceBet}  " + $"Insurance Payment: {gameStatus.AvailableInsurancePayments[gameStatus.getInsurancePaymentIndex()]} " +
                $"Cards Remaining: {gameStatus.Deck.Count}  ";
            string space = "";
            for (int i = count.Length; i < Console.WindowWidth - 1; i++)
                space += " ";
            string temp1 = "";
            string temp2 = "";
            string message1 = "Press S to save progress";
            string message2 = "Press ESC to exit";
            for (int i = 0; i<space.Length - 4 - message1.Length; i++)
            {
                temp1 += " ";
            }
            for (int i = 0; i < space.Length - 4 - message2.Length; i++)
            {
                temp2 += " ";
            }

            ConsoleTable consoleTable = new ConsoleTable("Blackjack", "", "", "");
            consoleTable.AddRow("Player", "Rules", "Deck", space);
            consoleTable.AddRow($"Your Money: {player.Money}", $"Dealer Limit: {gameStatus.getDealerLimit()}", $"Cards Remaining: {gameStatus.Deck.Count}", temp1 + message1);
            consoleTable.AddRow($"Your Bet: {player.Bet}", $"Insurance Payment: {gameStatus.AvailableInsurancePayments[gameStatus.getInsurancePaymentIndex()]}", "", "");
            consoleTable.AddRow($"Your Insurance Bet: {player.InsuranceBet}", $"Win Payout: {gameStatus.AvailableWinPayouts[gameStatus.getWinPayoutIndex()]}", "", temp2 + message2);
            consoleTable.Write(Format.Minimal);

        }
        public void ShowMenuOptions(int optionSelected, string[] options, string[] optionsToChose, string[] tips)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine("");
                Console.SetCursorPosition((Console.WindowWidth - options[i].Length) / 2, Console.CursorTop);
                Console.WriteLine($"{options[i]}");
                if (optionSelected == i)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.SetCursorPosition((Console.WindowWidth - optionsToChose[i].Length) / 2, Console.CursorTop);
                Console.WriteLine($"{optionsToChose[i]}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition((Console.WindowWidth - tips[i].Length) / 2, Console.CursorTop);
                Console.WriteLine($"{tips[i]}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine($"");
        }
        public void ShowMenuOptions(int optionSelected, IList<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {              
                Console.WriteLine();
                Console.SetCursorPosition((Console.WindowWidth - options[i].Length + 1) / 2, Console.CursorTop);
                if (optionSelected == i)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.WriteLine($"{options[i]}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        public void ShowSaveFileOptions(int optionSelected, string[] options, string[] dates)
        {
            string lastTimeSaved = "Last Time Saved: ";
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine();
                Console.SetCursorPosition((Console.WindowWidth - options[i].Length) / 2, Console.CursorTop);
                if (optionSelected == i)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.WriteLine($"{options[i]}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition((Console.WindowWidth - (dates[i].Length + lastTimeSaved.Length)) / 2, Console.CursorTop);
                Console.Write(lastTimeSaved);
                Console.WriteLine($"{dates[i]}");              
            }
        }
        public void ShowCardsHidden(Player player, Dealer dealer)
        {
            Console.Write("Dealer's Cards: " + dealer.Hand[0].Name + " | " + "Card face down");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\t(Total Value: {dealer.Hand[0].Value} + One value hidden)\n");
            Console.ForegroundColor = ConsoleColor.White; 
            Console.Write("Your Cards: " + player.Hand[0].Name);
            for (int i=1; i<player.Hand.Count; i++)
            {
                Console.Write(" | " + player.Hand[i].Name);
                
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\t(Total Value: {player.Points})\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n");
        }
        public void ShowCards(Player player, Dealer dealer)
        {
            Console.Write("Dealer's Cards: " + dealer.Hand[0].Name);
            for (int i = 1; i < dealer.Hand.Count; i++)
            {
                Console.Write(" | " + dealer.Hand[i].Name);

            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\t(Total Value: {dealer.Points})\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n");
            Console.Write("Your Cards: " + player.Hand[0].Name);
            for (int i = 1; i < player.Hand.Count; i++)
            {
                Console.Write(" | " + player.Hand[i].Name);

            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\t(Total Value: {player.Points})\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n");
        }
        public void ShowBlackjackScreen()
        {
            AudioPlayer audioPlayer = new AudioPlayer("victoryWAV.wav");
            audioPlayer.StartMusic();

            string message = "Blackjack!";
            string cheer = " WOOOOOOOOOOOOOOOOOOOOOOOOOW\n";
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
                System.Threading.Thread.Sleep(200);
            }
            AudioPlayer audioPlayerCheer = new AudioPlayer("ohYeahYes.wav");
            audioPlayerCheer.StartMusic();
            for (int i = 0; i < cheer.Length; i++)
            {
                Console.Write(cheer[i]);
                System.Threading.Thread.Sleep(50);
            }
            Console.ResetColor();
        }
        public void ShowBlackjackDrawScreen()
        {
            string message = "A dealer also has blackjack";
            string face = ":/\n";
            AudioPlayer audioPlayer = new AudioPlayer("drawWAV.wav");
            audioPlayer.StartMusic();
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
                System.Threading.Thread.Sleep(200);
            }
            System.Threading.Thread.Sleep(500);
            Console.Write(face);
            Console.ResetColor();
        }
        public void ShowDrawScreen()
        {
            string message = "Draw\n";
            AudioPlayer audioPlayer = new AudioPlayer("drawWAV.wav");
            audioPlayer.StartMusic();
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
                System.Threading.Thread.Sleep(200);
            }
            Console.ResetColor();
        }
        public void ShowWinScreen()
        {
            PlayMusic("victoryWAV.wav");

            string message = "You Win!\n";
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < message.Length; i++)
            {
                Console.Write(message[i]);
                System.Threading.Thread.Sleep(200);
            }
            Console.ResetColor();
        }
        public void ShowLostScreen()
        {
            PlayMusic("lostWAV.wav");

            string message = "You Lost\n";
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i=0; i<message.Length; i++)
            {
                Console.Write(message[i]);
                System.Threading.Thread.Sleep(200);
            }          
            Console.ResetColor();
        }
        public void ShowCredits()
        {
            string pathTxt = AppManager.getPath("Credits/Credits.txt");

            string[] credits = File.ReadAllLines(pathTxt);
            Console.WriteLine("");
            Console.WriteLine("");
            for(int i=0; i<credits.Length; i++)
            {
                System.Threading.Thread.Sleep(500);
                for(int j=i; j>=0; j--)
                {
                    Console.SetCursorPosition(0, Console.WindowHeight - j);
                    ClearOneLine();
                    WriteLineCenter(credits[Math.Abs(j-i)]);
                }
            }
        }
        public void ShowTitle()
        {
            string[] title = new string[11];
            Console.WriteLine("");
            Console.WriteLine("");
            title[0] = @"            /       \ /  |                    /  |                                  /  |";
            title[1] = @"$$$$$$$  |$$ |  ______    _______ $$ |   __      __   ______    _______ $$ |   __ ";
            title[2] = @"$$ |__$$ |$$ | /      \  /       |$$ |  /  |    /  | /      \  /       |$$ |  /  |";
            title[3] = @"$$    $$< $$ | $$$$$$  |/$$$$$$$/ $$ |_/$$/     $$/  $$$$$$  |/$$$$$$$/ $$ |_/$$/ ";
            title[4] = @"$$$$$$$  |$$ | /    $$ |$$ |      $$   $$<      /  | /    $$ |$$ |      $$   $$<  ";
            title[5] = @"$$ |__$$ |$$ |/$$$$$$$ |$$ \_____ $$$$$$  \     $$ |/$$$$$$$ |$$ \_____ $$$$$$  \ ";
            title[6] = @"$$    $$/ $$ |$$    $$ |$$       |$$ | $$  |    $$ |$$    $$ |$$       |$$ | $$  |";
            title[7] = @"$$$$$$$/  $$/  $$$$$$$/  $$$$$$$/ $$/   $$/__   $$ | $$$$$$$/  $$$$$$$/ $$/   $$/ ";
            title[8] = @"                                          /  \__$$ |                              ";
            title[9] = @"                                          $$    $$/                               ";
            title[10] = @"                                           $$$$$$/                                ";           
            
            for(int i=0; i<title.Length; i++)
            {
                WriteLineCenter(title[i]);
            }
            Console.WriteLine("");
            Console.WriteLine("");
        }
        public int CheckArrowKeyVertical(ConsoleKey keyEntered, int selectedOption, IList<string> options)
        {
            if (keyEntered == ConsoleKey.UpArrow)
            {
                selectedOption--;
                if (selectedOption < 0)
                    selectedOption = options.Count - 1;
                return selectedOption;
            }
            else if (keyEntered == ConsoleKey.DownArrow)
            {
                selectedOption = (selectedOption + 1) % options.Count;
                return selectedOption;
            }
            return selectedOption;
        }
        public int CheckArrowKeyHorizontal(ConsoleKey keyEntered, int selectedOption, string[] options)
        {
            if (keyEntered == ConsoleKey.LeftArrow)
            {
                selectedOption--;
                if (selectedOption < 0)
                    selectedOption = options.Length - 1;
                return selectedOption;
            }
            else if (keyEntered == ConsoleKey.RightArrow)
            {
                selectedOption = (selectedOption + 1) % options.Length;
                return selectedOption;
            }
            return selectedOption;
        }
        public Dictionary<string, int> CheckConsoleSizeMainMenu(Dictionary<string, int> consoleSize)
        {
            if (Console.WindowWidth != consoleSize["Width"] || Console.WindowHeight != consoleSize["Height"])
            {
                Console.Clear();
                ShowTitle();
                consoleSize["Width"] = Console.WindowWidth;
                consoleSize["Height"] = Console.WindowHeight;
            }
            return consoleSize;
        }
        public Dictionary<string, int> CheckConsoleSizeGameplay(Dictionary<string, int> consoleSize, GameStatus gameStatus, Player player)
        {
            if (Console.WindowWidth != consoleSize["Width"] || Console.WindowHeight != consoleSize["Height"])
            {
                Console.Clear();
                ShowGameplayTitle(player, gameStatus);
                consoleSize["Width"] = Console.WindowWidth;
                consoleSize["Height"] = Console.WindowHeight;
            }
            return consoleSize;
        }
        public void WriteLineCenter(string message, ConsoleColor consoleColor = ConsoleColor.White)
        {
            Console.ForegroundColor = consoleColor;
            int position = (Console.WindowWidth - message.Length) / 2;
            position = (position<0) ? 0:position;
            Console.SetCursorPosition(position, Console.CursorTop);
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public void WriteLineRight(string message)
        {
            int position = (Console.WindowWidth - message.Length + 1);
            position = (position < 0) ? 0 : position;
            Console.SetCursorPosition(position, Console.CursorTop);
            Console.WriteLine(message);
        }
        public ConsoleKey ReadKey()
        {
            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
            return consoleKeyInfo.Key;
        }
        public void PlayMusic(string fileName)
        {
            AudioPlayer audioPlayer = new AudioPlayer(fileName);
            audioPlayer.StartMusic();
        }
    }
}
