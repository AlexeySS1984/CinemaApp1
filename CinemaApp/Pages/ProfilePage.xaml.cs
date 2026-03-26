using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CinemaApp.Pages
{
    /// <summary>ViewModel для строки билета в личном кабинете.</summary>
    public class TicketViewModel
    {
        public string MovieTitle        { get; set; }
        public string DateDisplay       { get; set; }
        public string HallDisplay       { get; set; }
        public string SeatDisplay       { get; set; }
        public string PriceDisplay      { get; set; }
        public string PurchaseDateDisplay { get; set; }
    }

    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
            {
                MainWindow.Instance.Navigate(new LoginPage());
                return;
            }

            var user = Core.CurrentUser;

            // Аватар — первая буква имени
            AvatarText.Text  = (user.FullName ?? user.Login).Substring(0, 1).ToUpper();
            FullNameText.Text = user.FullName ?? user.Login;
            LoginText.Text    = $"@{user.Login}";
            EmailText.Text    = user.Email ?? "";

            LoadTickets(user.Id);
        }

        private void LoadTickets(int userId)
        {
            try
            {
                var tickets = Core.Context.Tickets
                    .Include("Sessions")
                    .Include("Sessions.Movies")
                    .Include("Sessions.Halls")
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.PurchaseDate)
                    .ToList();

                if (tickets.Count == 0)
                {
                    NoTicketsText.Visibility = Visibility.Visible;
                    return;
                }

                var vmList = tickets.Select(t => new TicketViewModel
                {
                    MovieTitle = t.Sessions?.Movies?.Title ?? "—",
                    DateDisplay = $"{t.Sessions?.SessionDate:dd.MM.yyyy}  {t.Sessions?.SessionTime:hh\\:mm}",
                    HallDisplay = $"{t.Sessions?.Halls?.Name} ({t.Sessions?.Halls?.Classification})",
                    SeatDisplay = $"Ряд {t.SeatRow}, Место {t.SeatNumber}",
                    PriceDisplay = t.Sessions?.Price != null
                        ? $"{t.Sessions.Price:N0} ₽" : "—",
                    PurchaseDateDisplay = t.PurchaseDate.HasValue
                        ? $"Куплено {t.PurchaseDate.Value:dd.MM.yyyy HH:mm}" : ""
                }).ToList();

                TicketsList.ItemsSource = vmList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки билетов:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            Core.CurrentUser = null;
            MainWindow.Instance.Navigate(new MainPage());
        }

        private void BackBtn_Click(object sender, MouseButtonEventArgs e)
            => MainWindow.Instance.Navigate(new MainPage());
    }
}
