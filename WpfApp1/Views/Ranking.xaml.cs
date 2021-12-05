using Blackjack_Projekt_Logic_Layer;
using Blackjack_Projekt_Logic_Layer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Ranking.xaml
    /// </summary>
    public partial class Ranking : Window
    {
        public GameStatus GameStatus { get; set; }
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        public List<RankingModel> Rankings { get; set; }
        private int iteration = 0;
        private int slotsCount = 4;
        public Ranking()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Rankings = AppManager.LoadFromRanking();
            Rankings = Rankings.OrderByDescending(x => x.MoneyEarned).ToList();
            ShowRankingRecords();
        }
        public Ranking(GameStatus gameStatus, Player player, Dealer dealer)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            Rankings = AppManager.LoadFromRanking();
            Rankings = Rankings.OrderByDescending(x => x.MoneyEarned).ToList();
            ShowRankingRecords();           
        }
        private void ShowRankingRecords()
        {                         
            try
            {
                StackP1tb1.Text = "Money Earned: " + Rankings[iteration].MoneyEarned.ToString();
                StackP1tb2.Text = "Initial Deck: " + Rankings[iteration].InitialDeck.ToString();
                StackP1tb3.Text = "Day of Play: " + Rankings[iteration].CreationDate.ToString();
            } 
            catch (ArgumentOutOfRangeException e)
            {
                StackP1tb1.Text = null;
                StackP1tb2.Text = null;
                StackP1tb3.Text = null;
            }

            if (Rankings.Count == 0)
            {
                StackP1tb1.Text = "You haven't played any round yet";
            }

            try
            {
                StackP2tb1.Text = "Money Earned: " + Rankings[iteration + 1].MoneyEarned.ToString();
                StackP2tb2.Text = "Initial Deck: " + Rankings[iteration + 1].InitialDeck.ToString();
                StackP2tb3.Text = "Day of Play: " + Rankings[iteration + 1].CreationDate.ToString();
            }
            catch (ArgumentOutOfRangeException e)
            {
                StackP2tb1.Text = null;
                StackP2tb2.Text = null;
                StackP2tb3.Text = null;
            }

            try
            {
                StackP3tb1.Text = "Money Earned: " + Rankings[iteration + 2].MoneyEarned.ToString();
                StackP3tb2.Text = "Initial Deck: " + Rankings[iteration + 2].InitialDeck.ToString();
                StackP3tb3.Text = "Day of Play: " + Rankings[iteration + 2].CreationDate.ToString();
            }
            catch (ArgumentOutOfRangeException e)
            {
                StackP3tb1.Text = null;
                StackP3tb2.Text = null;
                StackP3tb3.Text = null;
            }

            try
            {
                StackP4tb1.Text = "Money Earned: " + Rankings[iteration + 3].MoneyEarned.ToString();
                StackP4tb2.Text = "Initial Deck: " + Rankings[iteration + 3].InitialDeck.ToString();
                StackP4tb3.Text = "Day of Play: " + Rankings[iteration + 3].CreationDate.ToString();
            }
            catch (ArgumentOutOfRangeException e)
            {
                StackP4tb1.Text = null;
                StackP4tb2.Text = null;
                StackP4tb3.Text = null;
            }            

            if (iteration >= Rankings.Count - slotsCount)
                bRight.Visibility = Visibility.Hidden;
            else
                bRight.Visibility = Visibility.Visible;

            if (iteration == 0)
                bLeft.Visibility = Visibility.Hidden;
            else
                bLeft.Visibility = Visibility.Visible;

        }

        private void Right_Button_Click(object sender, RoutedEventArgs e)
        {
            iteration += slotsCount;
            ShowRankingRecords();
        }
        private void Left_Button_Click(object sender, RoutedEventArgs e)
        {
            iteration -= slotsCount;
            ShowRankingRecords();
        }
        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(GameStatus, Player, Dealer);
            mainWindow.Show();
            this.Close();
        }
        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureWindow areYouSureWindow = new AreYouSureWindow("Are you sure you want to clear all ranking?\n        There is no coming back from that");
            if((bool)areYouSureWindow.ShowDialog())
            {
                AppManager.ClearRanking();
                iteration = 0;
                Rankings = AppManager.LoadFromRanking();
                ShowRankingRecords();
            }           
        }       
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && !(iteration >= Rankings.Count - slotsCount))
            {
                iteration += slotsCount;
                ShowRankingRecords();
            }
            if (e.Key == Key.Left && !(iteration == 0))
            {
                iteration -= slotsCount;
                ShowRankingRecords();
            }
        }
    }
}
