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

namespace invoice.Views
{
    /// <summary>
    /// Logique d'interaction pour CreateFactureTwo.xaml
    /// </summary>
    public partial class CreateFactureTwo : UserControl
    {
        public CreateFactureTwo()
        {
            InitializeComponent();
            tbxDiscountAmountPercent.Text = "0";
        }

        private void RadioButton_Percent_Click(object sender, RoutedEventArgs e)
        {
            tbxDiscountAmountFlat.Visibility = Visibility.Collapsed;
            tbxDiscountAmountPercent.Visibility = Visibility.Visible;
            tbxDiscountAmountPercent.Text = "0";
        }
        private void RadioButton_Flat_Click(object sender, RoutedEventArgs e)
        {
            tbxDiscountAmountPercent.Visibility = Visibility.Collapsed;
            tbxDiscountAmountFlat.Visibility = Visibility.Visible;
            tbxDiscountAmountPercent.Text = "0";
        }
    }
}
