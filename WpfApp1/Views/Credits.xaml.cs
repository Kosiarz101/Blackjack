using Blackjack_Projekt_Logic_Layer;
using Blackjack_Projekt_Logic_Layer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Credits.xaml
    /// </summary>
    public partial class Credits : Window
    {
        public GameStatus GameStatus { get; set; }
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        private Storyboard StoryboardStart;
        private Storyboard StoryboardStop;
        private int animDuration = 1;
        public Credits()
        {
            InitializeComponent();           
        }
        public Credits(GameStatus gameStatus, Player player, Dealer dealer)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            CreateAnim();
        }
        private void CreateAnim()
        {
            //Start title animation
            var creditsAnimStart1 = new DoubleAnimation();
            creditsAnimStart1.From = 0.0;
            creditsAnimStart1.To = 1.0;
            creditsAnimStart1.Duration = new Duration(TimeSpan.FromSeconds(animDuration));
            creditsAnimStart1.AutoReverse = false;
            creditsAnimStart1.RepeatBehavior = new RepeatBehavior(1);

            //Start author animation
            var creditsAnimStart2 = new DoubleAnimation();
            creditsAnimStart2.From = 0.0;
            creditsAnimStart2.To = 1.0;
            creditsAnimStart2.Duration = new Duration(TimeSpan.FromSeconds(animDuration));
            creditsAnimStart2.AutoReverse = false;
            creditsAnimStart2.RepeatBehavior = new RepeatBehavior(1);

            //Stop title animation
            var creditsAnimStop1 = new DoubleAnimation();
            creditsAnimStop1.From = 1.0;
            creditsAnimStop1.To = 0.0;
            creditsAnimStop1.Duration = new Duration(TimeSpan.FromSeconds(animDuration));
            creditsAnimStop1.AutoReverse = false;
            creditsAnimStop1.RepeatBehavior = new RepeatBehavior(1);

            //Stop author animation
            var creditsAnimStop2 = new DoubleAnimation();
            creditsAnimStop2.From = 1.0;
            creditsAnimStop2.To = 0.0;
            creditsAnimStop2.Duration = new Duration(TimeSpan.FromSeconds(animDuration));
            creditsAnimStop2.AutoReverse = false;
            creditsAnimStop2.RepeatBehavior = new RepeatBehavior(1);

            StoryboardStart = new Storyboard();
            StoryboardStart.Children.Add(creditsAnimStart1);
            StoryboardStart.Children.Add(creditsAnimStart2);
            Storyboard.SetTargetName(creditsAnimStart1, Title.Name);
            Storyboard.SetTargetName(creditsAnimStart2, Author.Name);

            StoryboardStop = new Storyboard();
            StoryboardStop.Children.Add(creditsAnimStop1);
            StoryboardStop.Children.Add(creditsAnimStop2);
            Storyboard.SetTargetName(creditsAnimStop1, Title.Name);
            Storyboard.SetTargetName(creditsAnimStop2, Author.Name);

            Storyboard.SetTargetProperty(creditsAnimStart1, new PropertyPath(TextBlock.OpacityProperty));
            Storyboard.SetTargetProperty(creditsAnimStart2, new PropertyPath(TextBlock.OpacityProperty));
            Storyboard.SetTargetProperty(creditsAnimStop1, new PropertyPath(TextBlock.OpacityProperty));
            Storyboard.SetTargetProperty(creditsAnimStop2, new PropertyPath(TextBlock.OpacityProperty));
        }
        private string[] GetCredits()
        {
            string pathTxt = AppManager.getPath("Credits/Credits.txt");
            string[] credits = File.ReadAllLines(pathTxt);
            return credits;
            //Storyboard sb = this.FindResource("CreditsAnim1") as Storyboard;
            //if (sb != null) { BeginStoryboard(sb); }
            //System.Threading.Thread.Sleep(2000);
            //if (sb != null) { BeginStoryboard(sb); }
        }
        private async Task DoAnim()
        {
            string[] credits = GetCredits();
            for (int i = 0; i < credits.Length; i++)
            {
                Title.Text = credits[i];
                Author.Text = credits[i + 1];
                i = i + 2;
                this.Dispatcher.Invoke(() =>
                {
                    StoryboardStart.Begin(this);
                });

                await Task.Delay(3000);
                this.Dispatcher.Invoke(() =>
                {
                    StoryboardStop.Begin(this);
                });
                await Task.Delay(animDuration * 1000);
            }
            Return();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await DoAnim();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Return();  
        }
        private void Return()
        {
            MainWindow mainWindow = new MainWindow(GameStatus, Player, Dealer);
            mainWindow.Show();
            this.Close();
        }
    }
}
