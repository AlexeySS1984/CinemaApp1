using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CinemaApp.Pages
{
    public partial class SessionPage : Page
    {
        private readonly int _sessionId;
        private Sessions     _session;
        private bool         _hideOccupied = false;

        // Выбранное место
        private int _selRow = -1;
        private int _selNum = -1;

        // Кнопки мест: [row][number] -> Button
        private Dictionary<(int, int), Button> _seatButtons = new Dictionary<(int, int), Button>();
        // Занятые места
        private HashSet<(int, int)> _occupiedSeats = new HashSet<(int, int)>();

        // Цвета
        private static readonly Brush Free     = new SolidColorBrush(Color.FromRgb(42,  42,  74));
        private static readonly Brush Occupied = new SolidColorBrush(Color.FromRgb(233, 69,  96));
        private static readonly Brush Selected = new SolidColorBrush(Color.FromRgb(39,  174, 96));

        public SessionPage(int sessionId)
        {
            InitializeComponent();
            _sessionId = sessionId;
            Loaded += SessionPage_Loaded;
        }

        private void SessionPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _session = Core.Context.Sessions
                    .Include("Movies")
                    .Include("Halls")
                    .FirstOrDefault(s => s.Id == _sessionId);

                if (_session == null) { MainWindow.Instance.Navigate(new MainPage()); return; }

                // Шапка
                MovieTitleText.Text  = _session.Movies?.Title ?? "";
                SessionDateText.Text = $"📅 {_session.SessionDate:dd.MM.yyyy}";
                SessionTimeText.Text = $"🕒 {_session.SessionTime:hh\\:mm}";
                HallText.Text        = $"🏛 {_session.Halls?.Name} ({_session.Halls?.Classification})";

                // Занятые места из уже купленных билетов
                var taken = Core.Context.Tickets
                    .Where(t => t.SessionId == _sessionId)
                    .Select(t => new { t.SeatRow, t.SeatNumber })
                    .ToList();

                foreach (var t in taken)
                    _occupiedSeats.Add((t.SeatRow, t.SeatNumber));

                BuildSeatGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сеанса:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Построить схему зала ─────────────────────────────────────────────────
        private void BuildSeatGrid()
        {
            SeatsGrid.Items.Clear();
            _seatButtons.Clear();

            int rows        = _session.Halls?.Rows        ?? 8;
            int seatsPerRow = _session.Halls?.SeatsPerRow ?? 10;

            for (int r = 1; r <= rows; r++)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 6) };

                // Номер ряда
                var rowLabel = new TextBlock
                {
                    Text              = r.ToString(),
                    Foreground        = Brushes.Gray,
                    Width             = 24,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment     = TextAlignment.Right,
                    Margin            = new Thickness(0, 0, 8, 0),
                    FontSize          = 11
                };
                rowPanel.Children.Add(rowLabel);

                for (int n = 1; n <= seatsPerRow; n++)
                {
                    bool isOccupied = _occupiedSeats.Contains((r, n));

                    var btn = new Button
                    {
                        Width     = 32,
                        Height    = 32,
                        Margin    = new Thickness(3),
                        Content   = n.ToString(),
                        FontSize  = 10,
                        Cursor    = Cursors.Hand,
                        Tag       = (r, n),
                        IsEnabled = !isOccupied,
                        Foreground = Brushes.White,
                        BorderThickness = new Thickness(0)
                    };

                    // Стиль кнопки
                    var template = new ControlTemplate(typeof(Button));
                    var border   = new FrameworkElementFactory(typeof(Border));
                    border.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
                    border.SetBinding(Border.BackgroundProperty,
                        new System.Windows.Data.Binding("Background") { RelativeSource = new System.Windows.Data.RelativeSource(System.Windows.Data.RelativeSourceMode.TemplatedParent) });
                    var cp = new FrameworkElementFactory(typeof(ContentPresenter));
                    cp.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    cp.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                    border.AppendChild(cp);
                    template.VisualTree = border;
                    btn.Template = template;

                    btn.Background = isOccupied ? Occupied : Free;
                    if (isOccupied) btn.Opacity = _hideOccupied ? 0 : 0.4;

                    btn.Click += SeatBtn_Click;
                    _seatButtons[(r, n)] = btn;

                    if (_hideOccupied && isOccupied)
                        btn.Visibility = Visibility.Collapsed;

                    rowPanel.Children.Add(btn);
                }

                SeatsGrid.Items.Add(new ContentControl { Content = rowPanel });
            }
        }

        // ─── Клик по месту ────────────────────────────────────────────────────────
        private void SeatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button btn)) return;

            var tag = (ValueTuple<int, int>)btn.Tag;
            int row = tag.Item1;
            int num = tag.Item2;

            if (_selRow != -1 && _selNum != -1)
            {
                if (_seatButtons.TryGetValue((_selRow, _selNum), out var prev))
                    prev.Background = Free;
            }

            _selRow = row;
            _selNum = num;
            btn.Background = Selected;

            SelectedSeatText.Text = $"Ряд {row}, Место {num}";
            PriceText.Text = $"{_session.Price:N0} ₽";
            BookBtn.IsEnabled = true;
        }
        // ─── Скрыть/показать занятые ─────────────────────────────────────────────
        private void ToggleOccupied_Click(object sender, RoutedEventArgs e)
        {
            _hideOccupied = !_hideOccupied;
            ToggleOccupiedBtn.Content = _hideOccupied ? "👁 Показать занятые" : "🚫 Скрыть занятые";

            foreach (var kv in _seatButtons)
            {
                if (_occupiedSeats.Contains(kv.Key))
                    kv.Value.Visibility = _hideOccupied ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        // ─── Оформить билет ───────────────────────────────────────────────────────
        private void BookBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selRow == -1) return;

            Core.SelectedSeatRow    = _selRow;
            Core.SelectedSeatNumber = _selNum;
            MainWindow.Instance.Navigate(new TicketPage(_sessionId, _selRow, _selNum));
        }

        private void BackBtn_Click(object sender, MouseButtonEventArgs e)
        {
            if (Core.SelectedMovieId.HasValue)
                MainWindow.Instance.Navigate(new MoviePage(Core.SelectedMovieId.Value));
            else
                MainWindow.Instance.Navigate(new MainPage());
        }
    }
}
