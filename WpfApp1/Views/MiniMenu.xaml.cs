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
    /// Logika interakcji dla klasy MiniMenu.xaml
    /// </summary>
    public partial class MiniMenu : Window
    {
        public GameStatus GameStatus { get; set; }
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        public Play Play { get; set; }
        public MiniMenu(GameStatus gameStatus, Player player, Dealer dealer, Play play)
        {
            InitializeComponent();
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            Play = play;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void SaveGame_Click(object sender, RoutedEventArgs e)
        {
            SaveGame saveGame = new SaveGame(GameStatus, Player, Dealer, Play.AudioPlayer);
            saveGame.Show();
            Play.Close();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            AreYouSureWindow areYouSureWindow = new AreYouSureWindow("Are you sure you want to exit?");
            if (!(bool)areYouSureWindow.ShowDialog())
            {
                return;
            }
            MainWindow mainWindow = new MainWindow(GameStatus, Player, Dealer);
            mainWindow.Show();
            Play.AudioPlayer.StartDifferentMusic("MainTheme.wav");
            Play.Close();
            this.Close();
        }
    }
}
