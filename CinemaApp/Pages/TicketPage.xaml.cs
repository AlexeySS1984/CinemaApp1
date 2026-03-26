using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CinemaApp.Pages
{
    public partial class TicketPage : Page
    {
        private readonly int _sessionId;
        private readonly int _seatRow;
        private readonly int _seatNum;
        private Sessions     _session;

        public TicketPage(int sessionId, int seatRow, int seatNum)
        {
            InitializeComponent();
            _sessionId = sessionId;
            _seatRow   = seatRow;
            _seatNum   = seatNum;
            Loaded += TicketPage_Loaded;
        }

        private void TicketPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _session = Core.Context.Sessions
                    .Include("Movies")
                    .Include("Halls")
                    .FirstOrDefault(s => s.Id == _sessionId);

                if (_session == null) { MainWindow.Instance.Navigate(new MainPage()); return; }

                MovieNameText.Text  = _session.Movies?.Title ?? "—";
                HallNameText.Text   = $"{_session.Halls?.Name} ({_session.Halls?.Classification})";
                DateTimeText.Text   = $"{_session.SessionDate:dd.MM.yyyy}  {_session.SessionTime:hh\\:mm}";
                SeatText.Text       = $"Ряд {_seatRow}, Место {_seatNum}";
                TotalPriceText.Text = $"{_session.Price:N0} ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка: место уже занято?
                bool alreadyTaken = Core.Context.Tickets.Any(t =>
                    t.SessionId == _sessionId &&
                    t.SeatRow   == _seatRow   &&
                    t.SeatNumber == _seatNum);

                if (alreadyTaken)
                {
                    MessageBox.Show("Это место только что было занято другим пользователем.\nПожалуйста, выберите другое место.",
                        "Место занято", MessageBoxButton.OK, MessageBoxImage.Warning);
                    MainWindow.Instance.Navigate(new SessionPage(_sessionId));
                    return;
                }

                var ticket = new Tickets
                {
                    UserId      = Core.CurrentUser.Id,
                    SessionId   = _sessionId,
                    SeatRow     = _seatRow,
                    SeatNumber  = _seatNum,
                    PurchaseDate = DateTime.Now
                };
                Core.Context.Tickets.Add(ticket);
                Core.Context.SaveChanges();

                MessageBox.Show($"🎉 Билет успешно оформлен!\n\n" +
                                $"Фильм:  {_session.Movies?.Title}\n" +
                                $"Зал:    {_session.Halls?.Name}\n" +
                                $"Дата:   {_session.SessionDate:dd.MM.yyyy}  {_session.SessionTime:hh\\:mm}\n" +
                                $"Место:  Ряд {_seatRow}, Место {_seatNum}",
                    "Готово!", MessageBoxButton.OK, MessageBoxImage.Information);

                Core.ResetSelection();
                MainWindow.Instance.Navigate(new MainPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения:\n{ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
            => MainWindow.Instance.Navigate(new SessionPage(_sessionId));
    }
}
