using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CinemaApp.Pages
{
    public partial class MainPage : Page
    {
        private List<Movies> _allMovies = new List<Movies>();
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Core.ResetSelection();
            UpdateUserUI();
            LoadMovies();
        }

        // ─── Обновить интерфейс в зависимости от авторизации ─────────────────────
        private void UpdateUserUI()
        {
            if (Core.CurrentUser != null)
            {
                WelcomeText.Text     = $"Привет, {Core.CurrentUser.FullName ?? Core.CurrentUser.Login}!";
                LoginLogoutBtn.Content = "Выйти";
            }
            else
            {
                WelcomeText.Text     = "";
                LoginLogoutBtn.Content = "Войти";
            }
        }

        // ─── Загрузить фильмы из БД ───────────────────────────────────────────────
        private void LoadMovies()
        {
            try
            {
                _allMovies = Core.Context.Movies.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _allMovies = new List<Movies>();
            }
            ApplyFilters();
        }

        // ─── Применить поиск + сортировку ────────────────────────────────────────
        private void ApplyFilters()
        {
            if (MoviesItemsControl == null || SearchBox == null || SortCombo == null)
                return;

            var query = (_allMovies ?? new List<Movies>()).AsEnumerable();

            string search = SearchBox.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrEmpty(search))
                query = query.Where(m => m.Title != null && m.Title.ToLower().Contains(search));

            switch (SortCombo.SelectedIndex)
            {
                case 1: query = query.OrderBy(m => m.Title); break;
                case 2: query = query.OrderByDescending(m => m.Title); break;
                case 3: query = query.OrderByDescending(m => m.Rating); break;
                case 4: query = query.OrderBy(m => m.Rating); break;
            }

            MoviesItemsControl.ItemsSource = query.ToList();
        }
        // ─── События ─────────────────────────────────────────────────────────────
        // ─── Безопасная загрузка постера из URL (обходит XDG0006) ───────────────
        private void PosterImg_Loaded(object sender, RoutedEventArgs e)
        {
            var img = sender as System.Windows.Controls.Image;
            if (img == null) return;

            string url = img.Tag as string;
            if (string.IsNullOrEmpty(url)) return;

            try
            {
                var bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(url);
                bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bmp.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.IgnoreImageCache;
                bmp.EndInit();
                img.Source = bmp;
            }
            catch
            {
                // URL недоступен — оставляем заглушку
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void MovieCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is int id)
                MainWindow.Instance.Navigate(new MoviePage(id));
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
                MainWindow.Instance.Navigate(new LoginPage());
            else
                MainWindow.Instance.Navigate(new ProfilePage());
        }

        private void LoginLogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Core.CurrentUser == null)
                MainWindow.Instance.Navigate(new LoginPage());
            else
            {
                Core.CurrentUser = null;
                UpdateUserUI();
            }
        }
    }
}
