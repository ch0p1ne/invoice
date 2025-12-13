using invoice.ViewModels.SubViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
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

namespace invoice.CustomControls
{
    public partial class EditUserForm : UserControl
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "PasswordSecure",
                typeof(SecureString),
                typeof(EditUserForm),
                new FrameworkPropertyMetadata(
                    default(SecureString),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SecureString PasswordSecure
        {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordSecure = PasswordBox.SecurePassword;
        }

        public static readonly RoutedEvent RequestCloseEvent =
            EventManager.RegisterRoutedEvent(
                nameof(RequestClose),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(EditUserForm));

        public event RoutedEventHandler RequestClose
        {
            add => AddHandler(RequestCloseEvent, value);
            remove => RemoveHandler(RequestCloseEvent, value);
        }

        public EditUserForm()
        {
            InitializeComponent();
            Loaded += AddUserForm_Loaded;
            PasswordBox.PasswordChanged += OnPasswordChanged;
        }

        private void AddUserForm_Loaded(object sender, RoutedEventArgs e)
        {
            AttachCancelHandler(this);
        }

        private void AttachCancelHandler(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Button btn)
                {
                    if (btn.Content is string s && s.Trim().Equals("Annuler", System.StringComparison.OrdinalIgnoreCase))
                    {
                        btn.Click += CancelButton_Click;
                        return;
                    }
                }

                AttachCancelHandler(child);
            }
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            RaiseRequestClose();
        }

        public void RaiseRequestClose()
        {
            RaiseEvent(new RoutedEventArgs(RequestCloseEvent, this));
        }
    }
}
