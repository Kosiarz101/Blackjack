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
    /// Logika interakcji dla klasy Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public GameStatus GameStatus { get; set; }
        public Player Player { get; set; }
        public Dealer Dealer { get; set; }
        public AudioPlayer AudioPlayer { get; set; }
        private int selectedOptionDealerLimit;
        private int selectedOptionDeckQuantity;
        private int selectedOptionInsurancePayment;
        private int selectedOptionWinPayout;
        private decimal initialMoney;
        private int selectedOptionGameplayMusic;
        private bool isInitialMoneyCorrect = true;
        public Options(GameStatus gameStatus, Player player, Dealer dealer)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GameStatus = gameStatus;
            Player = player;
            Dealer = dealer;
            
            SetParameters();
            SetSlidersValue();
            AudioPlayer = new AudioPlayer(gameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic]);
        }
        public void SetParameters()
        {
            selectedOptionDealerLimit = GameStatus.getDealerLimitIndex();
            selectedOptionDeckQuantity = GameStatus.getDeckQuantityIndex();
            selectedOptionInsurancePayment = GameStatus.getInsurancePaymentIndex();
            selectedOptionWinPayout = GameStatus.getWinPayoutIndex();
            initialMoney = GameStatus.InitialMoney;
            selectedOptionGameplayMusic = GameStatus.getGameplayMusicIndex();
            tbOption11.Text = GameStatus.AvailableDealerLimits[selectedOptionDealerLimit];
            tbOption22.Text = GameStatus.AvailableDeckQuantities[selectedOptionDeckQuantity];
            tbOption33.Text = GameStatus.AvailableInsurancePayments[selectedOptionInsurancePayment];
            tbOption44.Text = GameStatus.AvailableWinPayouts[selectedOptionWinPayout];
            tbOption55.Text = GameStatus.InitialMoney.ToString();
            tbOption66.Text = GameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic];
        }
        public void SetSlidersValue()
        {
            slOption1.Value = GameStatus.getDealerLimitIndex();
            slOption1.Minimum = 0;
            slOption1.Maximum = GameStatus.AvailableDealerLimits.Length - 1;

            slOption2.Value = GameStatus.getDeckQuantityIndex();
            slOption2.Minimum = 0;
            slOption2.Maximum = GameStatus.AvailableDeckQuantities.Length - 1;

            slOption3.Value = GameStatus.getInsurancePaymentIndex();
            slOption3.Minimum = 0;
            slOption3.Maximum = GameStatus.AvailableInsurancePayments.Length - 1;

            slOption4.Value = GameStatus.getWinPayoutIndex();
            slOption4.Minimum = 0;
            slOption4.Maximum = GameStatus.AvailableWinPayouts.Length - 1;

            tboxOption5.Text = GameStatus.InitialMoney.ToString();
        }
        private void slOption1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedOptionDealerLimit = (int)slOption1.Value;
            tbOption11.Text = GameStatus.AvailableDealerLimits[selectedOptionDealerLimit];
            //tbOption11.Text = slOption1.Value.ToString();
            //if(slOption1.Value >=0 && slOption1.Value < 1)
            //{
            //    selectedOptionDealerLimit = 0;
            //    tbOption11.Text = GameStatus.AvailableDealerLimits[0];                   
            //}
            //else if(slOption1.Value >= 1 && slOption1.Value < 2)
            //{
            //    selectedOptionDealerLimit = 1;
            //    tbOption11.Text = GameStatus.AvailableDealerLimits[1];
            //}
            //else
            //{
            //    selectedOptionDealerLimit = 2;
            //    tbOption11.Text = GameStatus.AvailableDealerLimits[2];
            //}
        }
        public void slOption2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedOptionDeckQuantity = (int)slOption2.Value;
            tbOption22.Text = GameStatus.AvailableDeckQuantities[selectedOptionDeckQuantity];
        }

        private void slOption3_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedOptionInsurancePayment = (int)slOption3.Value;
            tbOption33.Text = GameStatus.AvailableInsurancePayments[selectedOptionInsurancePayment];
        }

        private void slOption4_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            selectedOptionWinPayout = (int)slOption4.Value;
            tbOption44.Text = GameStatus.AvailableWinPayouts[selectedOptionWinPayout];
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            GameStatus.ChangeOptionSettings(selectedOptionDealerLimit, selectedOptionInsurancePayment, selectedOptionWinPayout, selectedOptionDeckQuantity, selectedOptionGameplayMusic);
            if(!isInitialMoneyCorrect)
            {
                tbInfo.Foreground = Brushes.LightBlue;
                MainGrid.RowDefinitions.ElementAt(13).Height = new GridLength(1, GridUnitType.Star);
                tbInfo.Text = "Enter correct initial money value in order to save it";               
            }
            else
            {
                tbInfo.Foreground = Brushes.LightGreen;
                GameStatus.InitialMoney = initialMoney;
                MainGrid.RowDefinitions.ElementAt(13).Height = new GridLength(1, GridUnitType.Star);
                tbInfo.Text = "Options has been saved";
            }                          
        }
        private void Return_Button_Click(object sender, RoutedEventArgs e)
        {
            if(!IsChangeIsSaved())
            {
                AreYouSureWindow areYouSureWindow = new AreYouSureWindow("Are you sure you want to exit without saving?");
                if (!(bool)areYouSureWindow.ShowDialog())
                {
                    return;
                }
            }
            MainWindow mainWindow = new MainWindow(GameStatus, Player, Dealer);
            mainWindow.Show();
            this.Close();
        }
        private bool IsChangeIsSaved()
        {
            if (selectedOptionDealerLimit != GameStatus.getDealerLimitIndex() ||
               selectedOptionDeckQuantity != GameStatus.getDeckQuantityIndex() ||
               selectedOptionInsurancePayment != GameStatus.getInsurancePaymentIndex() ||
               selectedOptionWinPayout != GameStatus.getWinPayoutIndex() ||
               initialMoney != GameStatus.InitialMoney ||
               selectedOptionGameplayMusic != GameStatus.getGameplayMusicIndex())
            {
                return false;
            }
            else
                return true;
        }

        private void Text_Changed_Initial_Money(object sender, TextChangedEventArgs e)
        {
            if(!decimal.TryParse(tboxOption5.Text, out decimal result))
            {
                tbInfo.Foreground = Brushes.OrangeRed;
                isInitialMoneyCorrect = false;
                MainGrid.RowDefinitions.ElementAt(13).Height = new GridLength(1, GridUnitType.Star);
                tbInfo.Text = "Initial Money must be a decimal number";
            }
            else
            {
                if (result < 0 || result > 100000)
                {
                    tbInfo.Foreground = Brushes.OrangeRed;
                    isInitialMoneyCorrect = false;
                    MainGrid.RowDefinitions.ElementAt(13).Height = new GridLength(1, GridUnitType.Star);
                    tbInfo.Text = "Initial money must be less than 100 000 and greater than 0";
                }
                //Correct Value
                else
                {
                    isInitialMoneyCorrect = true;
                    initialMoney = decimal.Parse(tboxOption5.Text);
                    tbOption55.Text = initialMoney.ToString();
                    MainGrid.RowDefinitions.ElementAt(13).Height = new GridLength(0);
                    tbInfo.Text = null;
                }               
            }
        }

        private void bOption6Left_Click(object sender, RoutedEventArgs e)
        {
            selectedOptionGameplayMusic--;
            if (selectedOptionGameplayMusic < 0)
                selectedOptionGameplayMusic = GameStatus.AvailableGameplayMusic.Length - 1;
            tbOption66.Text = GameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic];
        }

        private void bOption6Right_Click(object sender, RoutedEventArgs e)
        {
            selectedOptionGameplayMusic = (selectedOptionGameplayMusic + 1) % GameStatus.AvailableGameplayMusic.Length;
            tbOption66.Text = GameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic];
        }

        private void bOption6_Click(object sender, RoutedEventArgs e)
        {
            if(!AudioPlayer.IsPlaying)
            {
                if (selectedOptionGameplayMusic != GameStatus.AvailableGameplayMusic.Length - 1)
                {
                    AudioPlayer.StartDifferentMusic(GameStatus.AvailableGameplayMusic[selectedOptionGameplayMusic]);
                }
            }
            else
            {
                AudioPlayer.StopMusic();
                AudioPlayer audioPlayer = new AudioPlayer("MainTheme.wav");
                audioPlayer.StartMusic();
            }    
        }
    }
}
