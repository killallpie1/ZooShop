using System.Windows;

namespace Bredikhin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Pages.AuthPage AuthPage = new Pages.AuthPage();
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = AuthPage;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = AuthPage;
            LoggedInAs.Visibility = Visibility.Hidden;
            LogoutButton.Visibility = Visibility.Hidden;
        }
    }
}
