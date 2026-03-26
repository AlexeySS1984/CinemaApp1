using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CinemaApp.Pages
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login    = LoginBox.Text.Trim();
            string password = PassBox.Password;
            Auth(login, password);
        }

        public bool Auth(string login,  string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowError("Заполните все поля.");
                return false;
            }

            try
            {
                var user = Core.Context.Users
                    .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user == null)
                {
                    ShowError("Неверный логин или пароль.");
                    return false;
                }

                Core.CurrentUser = user;
                MainWindow.Instance.Navigate(new MainPage());
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка подключения к БД:\n{ex.Message}");
            }
            return true;
        }

        private void ShowError(string msg)
        {
            ErrorText.Text       = msg;
            ErrorText.Visibility = Visibility.Visible;
        }

        private void RegisterLink_Click(object sender, MouseButtonEventArgs e)
            => MainWindow.Instance.Navigate(new RegisterPage());

        private void BackLink_Click(object sender, MouseButtonEventArgs e)
            => MainWindow.Instance.Navigate(new MainPage());
    }
}
