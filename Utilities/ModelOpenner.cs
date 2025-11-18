using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace invoice.Utilities
{
    public class ModelOpenner
    {

        public ModelOpenner()
        {
        }
        public ModelOpenner(string message)
        {
            MessageBox.Show(message);
        }
        public ModelOpenner(string title, string message, MessageBoxButton button)
        {
            MessageBox.Show(message);
        }

        public MessageBoxResult Show(string title, string message, MessageBoxButton button)
        {
            return MessageBox.Show(message, title, button);
        }
    }
}
