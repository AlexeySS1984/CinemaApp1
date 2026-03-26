using System.Windows;

namespace CinemaApp
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            // Стартуем с главной страницы
            MainFrame.Navigate(new Pages.MainPage());
        }

        /// <summary>Навигация из любого места приложения.</summary>
        public void Navigate(System.Windows.Controls.Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}
