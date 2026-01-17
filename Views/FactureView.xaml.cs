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
    /// Logique d'interaction pour FactureView.xaml
    /// </summary>
    public partial class FactureView : UserControl
    {
        public FactureView()
        {
            InitializeComponent();
            Loaded += FactureView_Loaded;
        }

        private void FactureView_Loaded(object? sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"FactureView DataContext type: {DataContext?.GetType().FullName ?? "null"}");
        }

    }
}
