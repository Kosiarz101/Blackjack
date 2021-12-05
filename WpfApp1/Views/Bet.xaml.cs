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
    /// Logika interakcji dla klasy Bet.xaml
    /// </summary>
    public partial class Bet : Window
    {
        public Player Player { get; set; }
        public bool IsBetInsurance { get; set; }
        public Bet()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        public Bet(Player player, bool isBetInsurance, string message = null)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Player = player;
            IsBetInsurance = isBetInsurance;
            if (message != null)
                tbMessageInput.Text = message;
            tbMoney.Text = "Your Money: " + Player.Money.ToString();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            switch(IsBetInsurance)
            {
                case true:
                    MakeInsuranceBet();
                    break;
                case false:
                    MakeBet();
                    break;
            }
        }
        private void MakeBet()
        {
            if (!decimal.TryParse(tbInput.Text, out decimal result))
            {
                tbMessageInput.Text = "You need enter an integer number";
            }
            else
            {
                if (!Player.MakeBet(result))
                {
                    tbMessageInput.Text = "Your bet must be greater than zero and lower than your accumulated money";
                }
                else
                {
                    DialogResult = true;
                    return;
                }
            }
        }
        private void MakeInsuranceBet()
        {
            if (!decimal.TryParse(tbInput.Text, out decimal result))
            {
                tbMessageInput.Text = "You need enter an integer number";
            }
            else
            {
                if (!Player.MakeInsuranceBet(result))
                {
                    tbMessageInput.Text = "Your bet must be greater than zero and lower than your accumulated money";
                }
                else
                {
                    DialogResult = true;
                    return;
                }
            }
        }
    }
}
