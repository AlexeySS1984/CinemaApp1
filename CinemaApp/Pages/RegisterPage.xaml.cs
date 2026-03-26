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

            Reg(name, email, login, pass, pass2);
        }
        public bool Reg(string name, string email, string login, string pass, string pass2)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(pass))
            {
                ShowError("Заполните все обязательные поля."); return false;
            }
            if (pass != pass2)
            {
                ShowError("Пароли не совпадают."); return false;
            }
            if (pass.Length < 4)
            {
                ShowError("Пароль должен быть не менее 4 символов."); return false;
            }

            try
            {
                bool loginExists = Core.Context.Users.Any(u => u.Login == login);
                if (loginExists)
                {
                    ShowError("Такой логин уже занят."); return false;
                }

                var newUser = new Users
                {
                    FullName = name,
                    Email = email,
                    Login = login,
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
            return true;
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
