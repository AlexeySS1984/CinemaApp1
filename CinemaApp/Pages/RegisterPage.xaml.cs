using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CinemaApp.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string name    = NameBox.Text.Trim();
            string email   = EmailBox.Text.Trim();
            string login   = LoginBox.Text.Trim();
            string pass    = PassBox.Password;
            string pass2   = Pass2Box.Password;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                ShowError("Заполните все обязательные поля."); return;
            }
            if (pass != pass2)
            {
                ShowError("Пароли не совпадают."); return;
            }
            if (pass.Length < 4)
            {
                ShowError("Пароль должен быть не менее 4 символов."); return;
            }

            try
            {
                bool loginExists = Core.Context.Users.Any(u => u.Login == login);
                if (loginExists)
                {
                    ShowError("Такой логин уже занят."); return;
                }

                var newUser = new Users
                {
                    FullName = name,
                    Email    = email,
                    Login    = login,
                    Password = pass
                };
                Core.Context.Users.Add(newUser);
                Core.Context.SaveChanges();

                Core.CurrentUser = newUser;
                MainWindow.Instance.Navigate(new MainPage());
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        private void ShowError(string msg)
        {
            ErrorText.Text       = msg;
            ErrorText.Visibility = Visibility.Visible;
        }

        private void LoginLink_Click(object sender, MouseButtonEventArgs e)
            => MainWindow.Instance.Navigate(new LoginPage());
    }
}
