using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Diplom.AppData;
using Diplom.AppData.Model;
using System.Data.Entity;
using Diplom.Pages.User;

namespace Diplom.Pages.Student
{
    /// <summary>
    /// Главная страница студента, отображающая доступные тесты и результаты
    /// </summary>
    public partial class StudentMainPage : Page
    {
        // Список доступных тестов для студента
        public List<Test> Tests { get; set; }

        /// <summary>
        /// Конструктор страницы студента
        /// </summary>
        public StudentMainPage()
        {
            InitializeComponent();

            // Установка контекста данных для привязок XAML
            DataContext = this;

            // Загрузка списка всех тестов из базы данных
            Tests = TeterinEntities.GetContext().Test.ToList();

            // Загрузка результатов текущего пользователя с информацией о тестах
            MyResult.ItemsSource = TeterinEntities.GetContext()
                .Result
                .Include(x => x.Test) // Включаем данные о тестах
                .Where(x => x.ID_User == LogClass.user.ID) // Фильтруем по текущему пользователю
                .ToList();
        }

        /// <summary>
        /// Обработчик кнопки "Выход" - возврат на страницу авторизации
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new LogPage());
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Пройти тест"
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, по которой кликнули
            Button button = sender as Button;

            // Получаем тест из DataContext кнопки
            Test selectedTest = button.DataContext as Test;

            if (selectedTest != null)
            {
                // Переход на страницу прохождения теста с передачей выбранного теста
                MainFrame.mainFrame.Navigate(new TestPage(selectedTest));
            }
            else
            {
                MessageBox.Show("Ошибка: не удалось определить выбранный тест");
            }
        }
    }
}