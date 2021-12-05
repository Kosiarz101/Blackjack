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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModels;
using WpfApp1.Views;

namespace WpfApp1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameStatus GameStatus { get; set; }
        public Dealer Dealer { get; set; }
        public Player Player { get; set; }
        public AudioPlayer AudioPlayer { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            GameStatus = new GameStatus();
            List<Card> Deck = AppManager.CreateDeck(1);
            GameStatus.Deck = Deck;
            Player = AppManager.CreatePlayer(GameStatus);
            Dealer = AppManager.CreateDealer(GameStatus);
            AudioPlayer = new AudioPlayer("MainTheme.wav");
            if(GameStatus.getGameplayMusicIndex() != GameStatus.AvailableGameplayMusic.Length - 1)
            {
                AudioPlayer.StartMusicLooping();
            }
        }
        public MainWindow(GameStatus gameStatus, Player player, Dealer dealer)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
        }
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
            Play play = new Play(GameStatus, Player, Dealer);
            play.Show();
            this.Close();
        }

        private void Options_Button_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options(GameStatus, Player, Dealer);
            options.Show();
            this.Close();
            //Window1 w = new Window1();
            //w.Show();
        }

        private void LoadGame_Button_Click(object sender, RoutedEventArgs e)
        {
            LoadGame loadGame = new LoadGame(GameStatus, Player, Dealer);
            loadGame.Show();
            this.Close();
        }
        private void Credits_Button_Click(object sender, RoutedEventArgs e)
        {
            Credits credits = new Credits(GameStatus, Player, Dealer);
            credits.Show();
            this.Close();
        }

        private void Ranking_Button_Click(object sender, RoutedEventArgs e)
        {
            Ranking ranking = new Ranking(GameStatus, Player, Dealer);
            ranking.Show();
            this.Close();
        }
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureWindow areYouSureWindow = new AreYouSureWindow("Are you sure you want to exit?");
            if ((bool)areYouSureWindow.ShowDialog())
            {
                System.Environment.Exit(0);
            }
        }
        private void NewGame()
        {           
            List<Card> Deck = AppManager.CreateDeck(GameStatus.getDeckQuantity());
            GameStatus.Deck = Deck;
            Player = AppManager.CreatePlayer(GameStatus);
            Dealer = AppManager.CreateDealer(GameStatus);
            Player.Money = GameStatus.InitialMoney;
            Dealer.DealerLimit = GameStatus.getDealerLimit();
        }
    }
}
