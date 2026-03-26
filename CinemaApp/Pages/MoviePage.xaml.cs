using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace CinemaApp.Pages
{
    public partial class MoviePage : Page
    {
        private readonly int _movieId;

        public MoviePage(int movieId)
        {
            InitializeComponent();
            _movieId = movieId;
            Loaded  += MoviePage_Loaded;
        }

        private void MoviePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var movie = Core.Context.Movies.Find(_movieId);
                if (movie == null) { MainWindow.Instance.Navigate(new MainPage()); return; }

                Core.SelectedMovieId = _movieId;

                // Постер
                if (!string.IsNullOrEmpty(movie.CoverImage))
                {
                    try { PosterImage.Source = new BitmapImage(new Uri(movie.CoverImage)); }
                    catch { /* изображение недоступно */ }
                }

                // Текстовые поля
                TitleText.Text  = movie.Title;
                RatingText.Text = movie.Rating.HasValue ? movie.Rating.Value.ToString("0.0") : "—";
                AgeText.Text    = movie.AgeRating ?? "";
                DescText.Text   = movie.Description ?? "";
                DateText.Text   = movie.StartDate.HasValue
                    ? movie.StartDate.Value.ToString("dd.MM.yyyy") : "—";

                // Жанры
                var genres = Core.Context.Genres
                    .Where(mg => mg.Id == _movieId)
                    .Select(mg => mg.Name)
                    .ToList();
                GenresList.ItemsSource = genres;

                // Сеансы
                var sessions = Core.Context.Sessions
                    .Include("Halls")
                    .Where(s => s.MovieId == _movieId &&
                                s.SessionDate >= DateTime.Today)
                    .OrderBy(s => s.SessionDate)
                    .ThenBy(s => s.SessionTime)
                    .ToList();

                if (sessions.Count == 0)
                    NoSessionsText.Visibility = Visibility.Visible;
                else
                    SessionsList.ItemsSource = sessions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки фильма:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SessionRow_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is int sessionId)
            {
                if (Core.CurrentUser == null)
                {
                    MessageBox.Show("Для бронирования места необходимо войти в аккаунт.",
                        "Требуется авторизация", MessageBoxButton.OK, MessageBoxImage.Information);
                    MainWindow.Instance.Navigate(new LoginPage());
                    return;
                }
                Core.SelectedSessionId = sessionId;
                MainWindow.Instance.Navigate(new SessionPage(sessionId));
            }
        }

        private void BackBtn_Click(object sender, MouseButtonEventArgs e)
            => MainWindow.Instance.Navigate(new MainPage());
    }
}
