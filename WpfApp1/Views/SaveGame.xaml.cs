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
    /// Logika interakcji dla klasy SaveGame.xaml
    /// </summary>
    public partial class SaveGame : Window
    {
        public GameStatus GameStatus { get; set; }
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        public AudioPlayer AudioPlayer { get; set; }
        public SaveGame(GameStatus gameStatus, Player player, Dealer dealer, AudioPlayer audioPlayer)
        {
            InitializeComponent();
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            AudioPlayer = audioPlayer;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loadSaveFiles();
        }
        public void loadSaveFiles()
        {
            string[] saveDates = AppManager.GetSaveDates();

            textSave1.Text = "Save slot 1";
            textSave2.Text = "Save slot 2";
            textSave3.Text = "Save slot 3";
            dataSave1.Text = saveDates[0];
            dataSave2.Text = saveDates[1];
            dataSave3.Text = saveDates[2];
        }
        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            Play play = new Play(GameStatus, Player, Dealer, AudioPlayer);
            play.Show();
            this.Close();
        }
        private void bSaveSlot1_Click(object sender, RoutedEventArgs e)
        {
            AppManager.SaveFile(GameStatus, Player, Dealer, 0);
            Play();
        }
        private void bSaveSlot2_Click(object sender, RoutedEventArgs e)
        {
            AppManager.SaveFile(GameStatus, Player, Dealer, 1);
            Play();
        }

        private void bSaveSlot3_Click(object sender, RoutedEventArgs e)
        {
            AppManager.SaveFile(GameStatus, Player, Dealer, 2);
            Play();
        }
        private void Play()
        {
            Play play = new Play(GameStatus, Player, Dealer);
            play.Show();
            this.Close();
        }
    }
}
