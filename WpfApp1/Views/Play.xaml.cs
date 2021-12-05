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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.Views
{
    /// <summary>
    /// Logika interakcji dla klasy Play.xaml
    /// </summary>
    public partial class Play : Window
    {
        public GameStatus GameStatus { get; set; }
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        public Style ButtonPlayStyle { get; set; }
        public Style ButtonDisabledStyle { get; set; }
        readonly static int PlayerInitialOffsetY = 280;
        readonly static int PlayerInitialOffsetX = -690;
        readonly static int DealerInitialOffsetY = 160;
        readonly static int DealerInitialOffsetX = -690;
        readonly static double animTimeStandard = 0.5;
        int PlayerOffsetY = PlayerInitialOffsetY;
        int PlayerOffsetX = PlayerInitialOffsetX;
        int DealerOffsetY = DealerInitialOffsetY;
        int DealerOffsetX = DealerInitialOffsetX;      
        int iteration = 0;
        bool isRoundEnded = false;
        public List<Border> PlayerCards { get; set; }
        public List<Border> DealerCards { get; set; }
        public AudioPlayer AudioPlayer { get; set; }
        public Play()
        {
            InitializeComponent();
        }
        public Play(GameStatus gameStatus, Player player, Dealer dealer)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            PlayerCards = new List<Border>();
            DealerCards = new List<Border>();

            PlayerOffsetY = (int)CardDeck.Height + 20 + DealerOffsetY;
            ButtonPlayStyle = this.FindResource("ButtonPlay") as Style;
            ButtonDisabledStyle = this.FindResource("ButtonDisabled") as Style;

            if (GameStatus.getGameplayMusicIndex() != GameStatus.AvailableGameplayMusic.Length - 1)
            {
                AudioPlayer = new AudioPlayer(gameStatus.getGameplayMusic());
                AudioPlayer.StartMusicLooping();
            }
            else
            {
                AudioPlayer = new AudioPlayer(gameStatus.AvailableGameplayMusic[0]);
                AudioPlayer.StartMusicLooping();
                AudioPlayer.StopMusic();
            }

            LoadCards();
            UpdateView();
            AnimateCards(animTimeStandard);
            CheckState();         
        }
        public Play(GameStatus gameStatus, Player player, Dealer dealer, AudioPlayer audioPlayer)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            PlayerCards = new List<Border>();
            DealerCards = new List<Border>();

            PlayerOffsetY = (int)CardDeck.Height + 20 + DealerOffsetY;
            ButtonPlayStyle = this.FindResource("ButtonPlay") as Style;
            ButtonDisabledStyle = this.FindResource("ButtonDisabled") as Style;

            AudioPlayer = audioPlayer;

            LoadCards();
            UpdateView();
            AnimateCards(0.01);
            CheckState();
        }
        private void UpdateView()
        {
            tbPlayer1.Text = "Your Money: " + Player.Money;
            tbPlayer2.Text = "Your Bet: " + Player.Bet;
            tbPlayer3.Text = "Your Insurance Bet: " + Player.InsuranceBet;

            tbRules1.Text = "Dealer Limit: " + Dealer.DealerLimit;
            tbRules2.Text = "Insurance Payment: " + GameStatus.getInsurancePayment();
            tbRules3.Text = "Win Payout: " + GameStatus.getWinPayout();

            tbDeck1.Text = "Cards Remaining: " + GameStatus.Deck.Count;
            TextBlock tb = (TextBlock)DealerCards[1].Child;
            if(tb.Text == "    Card\nFace Down")
                tbDealerPoints.Text = "Dealer Points: " + Dealer.Hand[0].Value + " + Card face down";
            else
                tbDealerPoints.Text = "Dealer Points: " + Dealer.Points;
            tbPlayerPoints.Text = "Player Points: " + Player.Points;
        }
        private async void LoadCards()
        {           
            for (int i=0; i< Dealer.Hand.Count; i++)
            {
                Border border = new Border();
                if(i == 1)
                    border = CreateCard(border, "    Card\nFace Down", DealerCards);
                else
                    border = CreateCard(border, Dealer.Hand[i].Name, DealerCards);
            }
            foreach (Card card in Player.Hand)
            {
                Border border = new Border();
                border = CreateCard(border, card.Name, PlayerCards);
            }
        }
        private async void AnimateCards(double animTime)
        {
            if(animTime > 0.1 && Player.Bet == 0)
                await ShowBetWindow();
            for (int i = 0; i < Dealer.Hand.Count; i++)
            {
                Border border = DealerCards[i];
                MainGrid.Children.Add(border);
                MoveTo(border, DealerOffsetX, DealerOffsetY, animTime);               
                DealerOffsetX += (int)border.Width + 5;
            }
            for (int i = 0; i < Player.Hand.Count; i++)
            {
                Border border = PlayerCards[i];
                MainGrid.Children.Add(border);
                MoveTo(border, PlayerOffsetX, PlayerOffsetY, animTime);              
                PlayerOffsetX += (int)border.Width + 5;
            }
        }
        private async void Hit_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await PlayerHit();
                UpdateView();
                CheckState();
                UpdateView();
            }
            catch(ArgumentOutOfRangeException ea)
            {
                await EndGame("There are no more cards to play. \nHope you enjoy game so far");
            }                   
        }
        private async void Stand_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await DealerMove();
            }
            catch (ArgumentOutOfRangeException eb)
            {
                await EndGame("There are no more cards to play. \nHope you enjoy game so far");
            }
        }
        private async void Double_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await PlayerHit();
                if (!Player.MakeBet(Player.Bet))
                {
                    Message.Text = "You don't have enough money to double the bet";
                    return;
                }
                if (!CheckState())
                {
                    await DealerMove();
                }
            }
            catch (ArgumentOutOfRangeException ea)
            {
                await EndGame("There are no more cards to play. \nHope you enjoy game so far");
            }
            
        }
        private void Insureance_Button_Click(object sender, RoutedEventArgs e)
        {
            Bet bet = new Bet(Player, true);
            bet.ShowDialog();
            UpdateView();
        }
        private Border CreateCard(Border border, string name, List<Border> Cards)
        {
            border.Background = CardDeck.Background;
            border.CornerRadius = new CornerRadius(5);           

            border.SetValue(Grid.RowProperty, 1);
            border.SetValue(Grid.ColumnProperty, 7);
            border.SetValue(Grid.RowSpanProperty, 3);

            border.Height = CardDeck.Height;
            border.Width = CardDeck.Width;
            border.Margin = CardDeck.Margin;
            border.HorizontalAlignment = CardDeck.HorizontalAlignment;
            border.VerticalAlignment = CardDeck.VerticalAlignment;

            TextBlock textBlock = new TextBlock();
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Foreground = Brushes.White;
            textBlock.FontSize = 16;
            textBlock.Text = name;

            border.Child = textBlock;
            Cards.Add(border);
            return border;                      
        }
        public async Task MoveTo(Border border, double newX, double newY, double animTime)
        {
            TranslateTransform trans = new TranslateTransform();
            border.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(0, newY, TimeSpan.FromSeconds(animTime));
            DoubleAnimation anim2 = new DoubleAnimation(0, newX, TimeSpan.FromSeconds(animTime));
            trans.BeginAnimation(TranslateTransform.YProperty, anim1);
            trans.BeginAnimation(TranslateTransform.XProperty, anim2);
            DisableButtons();
            await Task.Delay(((int)(animTime * 1000)));
            EnableButtons();          
        }
        public async Task ShowBetWindow()
        {
            Bet bet = new Bet(Player, false);
            bool isBetCorrect = (bool)bet.ShowDialog();
            while (!isBetCorrect)
            {
                bet = new Bet(Player, false, "You need to enter a bet");
                isBetCorrect = (bool)bet.ShowDialog();
            }
            UpdateView();
        }
        private async Task DealerMove()
        {
            ShowDealerCard();
            //Change as value if necessary
            if (Dealer.Points > 21)
                Dealer.SearchForAs();
            //check insurance requirment
            if (Dealer.Hand[1].Value != 10)
                Player.ResetInsuranceBet();

            //Player Lost
            if (Dealer.Points > Player.Points)
            {
                GameManager.PlayerLost(Player, Dealer, GameStatus);
                EndRoundScreen("lostWAV.wav", "You Lost!");
            }
            //Draw
            else if (Dealer.Points == Player.Points && Dealer.Points > Dealer.DealerLimit)
            {
                GameManager.PlayerNotLost(Player, Dealer, GameStatus, 1);
                EndRoundScreen("drawWAV.wav", "Draw");
            }
            else
            {
                while (Dealer.Points <= Dealer.DealerLimit && Dealer.Points <= Player.Points)
                {
                    await DealerHit();
                    if (Dealer.Points > 21)
                        Dealer.SearchForAs();
                    UpdateView();

                }
                CheckFinalState();
            }
        }
        private async Task DealerHit()
        {
            Border border = new Border();
            Dealer.AddToHand(GameStatus);
            border = CreateCard(border, Dealer.Hand.Last().Name, DealerCards);
            MainGrid.Children.Add(border);
            await MoveTo(border, DealerOffsetX, DealerOffsetY, 1);
            DealerOffsetX += (int)border.Width + 5;
        }
        private async Task PlayerHit()
        {
            Border border = new Border();
            Player.AddToHand(GameStatus);
            border = CreateCard(border, Player.Hand.Last().Name, PlayerCards);
            MainGrid.Children.Add(border);
            await MoveTo(border, PlayerOffsetX, PlayerOffsetY, animTimeStandard);
            PlayerOffsetX += (int)border.Width + 5;
        }
        private bool CheckState()
        {
            try
            {
                CheckInsurance();
                //Player Lost
                if (Player.Points > 21)
                {
                    if (Player.SearchForAs())
                        return false;
                    ShowDealerCard();
                    Player.ResetInsuranceBet();
                    GameManager.PlayerLost(Player, Dealer, GameStatus);
                    EndRoundScreen("lostWAV.wav", "You Lost!");
                    return true;
                }
                else if (Player.Points == 21)
                {
                    DealerMove();
                    return true;
                }
                return false;
            }
            catch (ArgumentOutOfRangeException e)
            {
                EndGame("You have no money to continue the game. \nHope you enjoy it so far!");
                return true;
            }
            return false;
        }        
        private void CheckFinalState()
        {
            bool isPlayerBlackjack = Player.Points == 21 ? true : false;
            int multiplier = isPlayerBlackjack ? 2 : 1;
            //Player Win
            if (Dealer.Points > 21 || Dealer.Points < Player.Points)
            {
                GameManager.PlayerNotLost(Player, Dealer, GameStatus, GameStatus.getWinPayout() * multiplier);
                if (isPlayerBlackjack)                   
                    EndRoundScreen("ohYeahYes.wav", "You have blackjack!!! WOOOOH!");
                else
                    //menu.ShowWinScreen();
                    EndRoundScreen("victoryWAV.wav", "You win!");
            }
            //Player Lost
            else if (Dealer.Points <= 21 && Dealer.Points > Player.Points)
            {
                GameManager.PlayerLost(Player, Dealer, GameStatus);
                EndRoundScreen("lostWAV.wav", "You Lost!");
            }
            //Draw
            else if (Dealer.Points == Player.Points)
            {
                GameManager.PlayerNotLost(Player, Dealer, GameStatus, multiplier);
                if (isPlayerBlackjack)
                    EndRoundScreen("drawWAV.wav", "Sorry, Dealer also has blackjack!");
                else
                    EndRoundScreen("drawWAV.wav", "Draw");
            }
        }
        private void CheckInsurance()
        {
            if(Player.InsuranceBet == 0 && Dealer.Hand[0].Name.ToLower() == "as")
            {
                Message.Text = "You can pay insurance now";
                bInsurance.Style = ButtonPlayStyle;
            }
            else
            {
                Message.Text = null;
                bInsurance.Style = ButtonDisabledStyle;
            }
        }
        private void DisableButtons()
        {
            bHit.Style = ButtonDisabledStyle;
            bHit.Opacity = 0.5;
            bStand.Style = ButtonDisabledStyle;
            bStand.Opacity = 0.5;
            bDouble.Style = ButtonDisabledStyle;
            bDouble.Opacity = 0.5;
            CheckInsurance();
        }
        private void EnableButtons()
        {
            bHit.Style = ButtonPlayStyle;
            bHit.Opacity = 1;
            bStand.Style = ButtonPlayStyle;
            bStand.Opacity = 1;
            bDouble.Style = ButtonPlayStyle;
            bDouble.Opacity = 1;
            CheckInsurance();
        }
        private async Task Reset()
        {
            //Create New Round
            PlayerCards = new List<Border>();
            DealerCards = new List<Border>();
            PlayerOffsetX = PlayerInitialOffsetX;
            DealerOffsetX = DealerInitialOffsetX;
            LoadCards();
            UpdateView();
            AnimateCards(animTimeStandard);
        }
        private async void EndRoundScreen(string musicFileName, string message)
        {
            DisableButtons();           

            //Show Message
            Message.Text = message + " Press Enter to continue";           
            //Play Music
            AudioPlayer audioPlayer = new AudioPlayer(musicFileName);
            audioPlayer.StartMusic();

            isRoundEnded = true;

            if (Player.Money == 0)
            {
                await EndGame("You have no money to continue the game. \nHope you enjoy it so far!");
                return;
            }
        }
        private async void EndRoundWork()
        {
            //if (Player.Money == 0)
            //{
            //    await EndGame();
            //    return;
            //}

            //Animate Player cards
            for (int i = PlayerCards.Count - 1; i >= 0; i--)
            {
                CardOut(PlayerCards[i], false);
                await Task.Delay(100);
            }
            //Animate Dealer Cards
            for (int i = DealerCards.Count - 1; i >= 0; i--)
            {
                CardOut(DealerCards[i], true);
                await Task.Delay(100);
            }

            await Task.Delay(2000);

            //Make cards animated invisible - Dealer
            for (int i = 0; i < DealerCards.Count; i++)
            {
                DealerCards[i].Visibility = Visibility.Hidden;
            }
            //Make cards animated invisible - Player
            for (int i = 0; i < PlayerCards.Count; i++)
            {
                PlayerCards[i].Visibility = Visibility.Hidden;
            }

            //if (Player.Money == 0)
            //{
            //    await EndGame();
            //    return;
            //}

            PlayMusic();
            await Reset();
            Message.Text = null;
            EnableButtons();
        }
        private async Task EndGame(string message)
        {
            Message.Text += message;
            Info info = new Info(message);
            info.ShowDialog();
            AppManager.SaveToRanking(GameStatus, Player);
            MainWindow mainWindow = new MainWindow(GameStatus, Player, Dealer);
            mainWindow.Show();
            this.Close();
        }
        private void bMenu_Click(object sender, RoutedEventArgs e)
        {
            MiniMenu miniMenu = new MiniMenu(GameStatus, Player, Dealer, this);
            miniMenu.ShowDialog();
        }
        private void ShowDealerCard()
        {
            TextBlock tb = new TextBlock();
            tb.Text = Dealer.Hand[1].Name;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            DealerCards[1].Child = tb;
            UpdateView();
        }
        private void PlayMusic()
        {
            if (GameStatus.getGameplayMusicIndex() != GameStatus.AvailableGameplayMusic.Length)
            {
                AudioPlayer.StartMusicLooping();
            }
        }
        private void CardOut(Border border, bool isDealer)
        {
            // Create a transform. This transform
            // will be used to move the rectangle.
            TranslateTransform animatedTranslateTransform =
                new TranslateTransform();

            // Register the transform's name with the page
            // so that they it be targeted by a Storyboard.

            this.RegisterName("AnimatedTranslateTransform" + iteration, animatedTranslateTransform);
  
            border.RenderTransform = null;
            border.RenderTransform = animatedTranslateTransform;

            // Create the animation path.
            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            PolyBezierSegment pBezierSegment = new PolyBezierSegment();

            if (!isDealer)
            {
                pFigure.StartPoint = new Point(PlayerOffsetX, PlayerOffsetY);
                PlayerOffsetX -= (int)border.Width - 5;
                
                pBezierSegment.Points.Add(new Point(-100, PlayerOffsetY));
                pBezierSegment.Points.Add(new Point(400, PlayerOffsetY));
                pBezierSegment.Points.Add(new Point(600, PlayerOffsetY));
                pBezierSegment.Points.Add(new Point(900, PlayerOffsetY));
            }
            else
            {
                pFigure.StartPoint = new Point(DealerOffsetX, DealerOffsetY);
                DealerOffsetX -= (int)border.Width - 5;

                pBezierSegment.Points.Add(new Point(-100, DealerOffsetY));
                pBezierSegment.Points.Add(new Point(400, DealerOffsetY));
                pBezierSegment.Points.Add(new Point(600, DealerOffsetY));
                pBezierSegment.Points.Add(new Point(900, DealerOffsetY));
            }
            
            pFigure.Segments.Add(pBezierSegment);
            animationPath.Figures.Add(pFigure);

            // Freeze the PathGeometry for performance benefits.
            animationPath.Freeze();

            // Create a DoubleAnimationUsingPath to move the
            // rectangle horizontally along the path by animating
            // its TranslateTransform.
            DoubleAnimationUsingPath translateXAnimation =
                new DoubleAnimationUsingPath();
            translateXAnimation.PathGeometry = animationPath;
            translateXAnimation.Duration = TimeSpan.FromSeconds(2);

            // Set the Source property to X. This makes
            // the animation generate horizontal offset values from
            // the path information.
            translateXAnimation.Source = PathAnimationSource.X;

            // Set the animation to target the X property
            // of the TranslateTransform named "AnimatedTranslateTransform".
            Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform" + iteration);
            Storyboard.SetTargetProperty(translateXAnimation,
                new PropertyPath(TranslateTransform.XProperty));

            // Create a DoubleAnimationUsingPath to move the
            // rectangle vertically along the path by animating
            // its TranslateTransform.
            DoubleAnimationUsingPath translateYAnimation =
                new DoubleAnimationUsingPath();
            translateYAnimation.PathGeometry = animationPath;
            translateYAnimation.Duration = TimeSpan.FromSeconds(2);

            // Set the Source property to Y. This makes
            // the animation generate vertical offset values from
            // the path information.
            translateYAnimation.Source = PathAnimationSource.Y;

            // Set the animation to target the Y property
            // of the TranslateTransform named "AnimatedTranslateTransform".
            Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform" + iteration);
            Storyboard.SetTargetProperty(translateYAnimation,
                new PropertyPath(TranslateTransform.YProperty));

            // Create a Storyboard to contain and apply the animations.
            Storyboard pathAnimationStoryboard = new Storyboard();
            pathAnimationStoryboard.RepeatBehavior = new RepeatBehavior(1);
            pathAnimationStoryboard.Children.Add(translateXAnimation);
            pathAnimationStoryboard.Children.Add(translateYAnimation);
            pathAnimationStoryboard.Begin(this);
            iteration++;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(isRoundEnded && e.Key == Key.Enter)
            {
                EndRoundWork();
            }
        }
    }
}
