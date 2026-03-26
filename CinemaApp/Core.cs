using System.Collections.Generic;

namespace CinemaApp
{
    /// <summary>
    /// Статический класс для хранения контекста БД и текущего состояния приложения.
    /// </summary>
    public static class Core
    {
        // ─── Контекст Entity Framework ───────────────────────────────────────────
        // После подключения ADO.NET EDM замените CinemaDBEntities на своё имя.
        public static CinemaDBEntities Context = new CinemaDBEntities();

        // ─── Текущий авторизованный пользователь ─────────────────────────────────
        public static Users CurrentUser = null;

        // ─── Временные данные для оформления билета ──────────────────────────────
        public static int?   SelectedMovieId   = null;
        public static int?   SelectedSessionId  = null;
        public static int    SelectedSeatRow    = 0;
        public static int    SelectedSeatNumber = 0;

        /// <summary>Сброс данных выбора (вызывается при возврате на главную).</summary>
        public static void ResetSelection()
        {
            SelectedMovieId   = null;
            SelectedSessionId  = null;
            SelectedSeatRow    = 0;
            SelectedSeatNumber = 0;
        }
    }
}
