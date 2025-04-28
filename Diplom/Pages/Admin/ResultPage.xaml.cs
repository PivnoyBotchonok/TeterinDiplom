using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Diplom.AppData;
using Diplom.AppData.Model;
using System.Data.Entity;

namespace Diplom.Pages.Admin
{
    /// <summary>
    /// Страница для просмотра результатов прохождения конкретного теста
    /// </summary>
    public partial class ResultPage : Page
    {
        // Хранит ID теста, результаты которого отображаются
        private int _testId;

        /// <summary>
        /// Инициализирует страницу результатов для указанного теста
        /// </summary>
        /// <param name="testId">ID теста для отображения результатов</param>
        public ResultPage(int testId)
        {
            InitializeComponent();
            _testId = testId;

            // При создании страницы сразу загружаем результаты теста
            using (var context = new TeterinEntities())
            {
                // Получаем результаты теста, включая информацию о пользователях
                var results = context.Result
                    .Include(r => r.User)  // Загружаем связанные данные о пользователях
                    .Where(r => r.ID_Test == _testId)  // Фильтруем по ID теста
                    .ToList();  // Преобразуем в список

                // Устанавливаем полученные результаты как источник данных для таблицы
                ResultDataGrid.ItemsSource = results;
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Назад"
        /// Возвращает на главную страницу администратора
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            // Переход обратно на главную страницу администратора
            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }
    }
}