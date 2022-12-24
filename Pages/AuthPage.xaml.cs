using Bredikhin.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bredikhin.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        readonly IQueryable<User> _users = new TradeEntities().User;
        string captchaCode = "";

        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (CaptchaField.Text != captchaCode)
            {
                ShowError("Неверно введена капча.");
                LoginButton.IsEnabled = false;
                DispatcherTimer timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(10),
                };
                timer.Tick += (s, ea) =>
                {
                    LoginButton.IsEnabled = true;
                    timer.Stop();
                };
                timer.Start();
                return;
            }
            User user = (from u in _users where u.UserLogin == LoginField.Text && u.UserPassword == PasswordField.Password select u).FirstOrDefault();
            if (user == null)
            {
                ShowError("Неверный логин или пароль.");
                return;
            }
            ((MainWindow)Window.GetWindow(this)).LoggedInAs.Text = $"Вы вошли как {user.UserSurname} {user.UserName} {user.UserPatronymic}";
            ((MainWindow)Window.GetWindow(this)).MainFrame.Content = new MainPage(user.UserRole);
            ((MainWindow)Window.GetWindow(this)).LoggedInAs.Visibility = Visibility.Visible;
            ((MainWindow)Window.GetWindow(this)).LogoutButton.Visibility = Visibility.Visible;
            ErrorMessage.Visibility = Visibility.Hidden;
        }

        private void LoginAsGuest(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((MainWindow)Window.GetWindow(this)).LoggedInAs.Text = $"Вы вошли как Гость";
            ((MainWindow)Window.GetWindow(this)).MainFrame.Content = new MainPage(0);
            ((MainWindow)Window.GetWindow(this)).LoggedInAs.Visibility = Visibility.Visible;
            ((MainWindow)Window.GetWindow(this)).LogoutButton.Visibility = Visibility.Visible;
            ErrorMessage.Visibility = Visibility.Hidden;
        }

        private void ShowError(string msg)
        {
            ErrorMessage.Text = msg;
            ErrorMessage.Visibility = Visibility.Visible;
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            CaptchaCanvas.Children.Clear();
            captchaCode = "";
            CaptchaContainer.Visibility = Visibility.Visible;
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                switch (random.Next(3))
                {
                    case 0:
                        captchaCode += ((char)('a' + random.Next(26))).ToString();
                        break;
                    case 1:
                        captchaCode += ((char)('A' + random.Next(26))).ToString();
                        break;
                    case 2:
                        captchaCode += random.Next(10).ToString();
                        break;
                }
                CaptchaCanvas.Children.Add(new TextBlock()
                {
                    Text = captchaCode[i].ToString(),
                    Margin = new Thickness(60 * i, CaptchaCanvas.ActualHeight / 4 + random.Next((int)-CaptchaCanvas.ActualHeight / 4, (int)CaptchaCanvas.ActualHeight / 4), 0, 0),
                    FontSize = 32,
                });
            }
            CaptchaCanvas.Children.Add(new Line()
            {
                Stroke = (Brush)FindResource("Accent"),
                StrokeThickness = 6,
                X1 = 0,
                X2 = CaptchaCanvas.ActualWidth,
                Y1 = CaptchaCanvas.ActualHeight / 2 + random.Next((int)(-CaptchaCanvas.ActualHeight / 3), (int)(CaptchaCanvas.ActualHeight / 3)),
                Y2 = CaptchaCanvas.ActualHeight / 2 + random.Next((int)(-CaptchaCanvas.ActualHeight / 3), (int)(CaptchaCanvas.ActualHeight / 3)),
            });
        }
    }
}
