using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Diplom.AppData;
using Diplom.AppData.Model;
using static System.Net.Mime.MediaTypeNames;

namespace Diplom.Pages.Admin
{
    /// <summary>
    /// Главная страница администратора, содержащая функционал управления тестами и пользователями
    /// </summary>
    public partial class AdminMainPage : Page
    {
        // Список тестов, созданных текущим пользователем
        public List<Test> MyTests { get; set; }

        // Список всех тестов в системе
        public List<Test> Tests { get; set; }

        /// <summary>
        /// Конструктор страницы администратора
        /// </summary>
        public AdminMainPage()
        {
            InitializeComponent();
            DataContext = this;

            // Загрузка данных при инициализации
            using (var context = new TeterinEntities())
            {
                // Получаем тесты текущего пользователя
                MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();

                // Загружаем список студентов (пользователей с ролью 2)
                StudentTable.ItemsSource = context.User.Where(x => x.ID_Role == 2).ToList();
            }
        }

        /// <summary>
        /// Обработчик кнопки возврата на страницу авторизации
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new LogPage());
        }

        /// <summary>
        /// Обработчик кнопки редактирования теста
        /// </summary>
        private void EditTestButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var test = button?.DataContext as Test;

            if (test != null)
            {
                // Передаем ID теста для редактирования на страницу создания/редактирования теста
                var createTestPage = new CreateTest(test.ID);
                MainFrame.mainFrame.Navigate(createTestPage);
            }
        }

        /// <summary>
        /// Обработчик кнопки удаления теста
        /// </summary>
        private void DeleteTestButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Test selectedTest)
            {
                // Запрос подтверждения удаления
                var result = MessageBox.Show($"Вы уверены, что хотите удалить тест '{selectedTest.Name}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new TeterinEntities())
                    {
                        try
                        {
                            // Находим тест для удаления
                            var testToDelete = context.Test.FirstOrDefault(t => t.ID == selectedTest.ID);
                            if (testToDelete != null)
                            {
                                // Удаляем связанные результаты
                                var results = context.Result.Where(r => r.ID_Test == testToDelete.ID).ToList();
                                context.Result.RemoveRange(results);

                                // Удаляем связанные вопросы и ответы
                                var questions = context.Question.Where(q => q.ID_Test == testToDelete.ID).ToList();

                                foreach (var q in questions)
                                {
                                    var answers = context.Answer.Where(a => a.ID_Question == q.ID);
                                    context.Answer.RemoveRange(answers);
                                }

                                context.Question.RemoveRange(questions);

                                // Удаляем сам тест
                                context.Test.Remove(testToDelete);
                                context.SaveChanges();

                                MessageBox.Show("Тест удалён.");

                                // Обновление списков тестов
                                MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                                Tests = context.Test.ToList();

                                // Обновление отображения
                                MyTestListView.ItemsSource = null;
                                MyTestListView.ItemsSource = MyTests;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Обработчик кнопки добавления нового теста
        /// </summary>
        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new CreateTest());
        }

        /// <summary>
        /// Обработчик кнопки просмотра результатов теста
        /// </summary>
        private void ResultBut_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var test = button?.DataContext as Test;

            if (test != null)
            {
                // Переход на страницу результатов с передачей ID теста
                MainFrame.mainFrame.Navigate(new ResultPage(test.ID));
            }
        }

        /// <summary>
        /// Обработчик события изменения видимости страницы
        /// </summary>
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                // При повторном отображении страницы обновляем данные
                using (var context = new TeterinEntities())
                {
                    // Перезагружаем данные из базы
                    context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());

                    // Обновляем контекст данных и списки тестов
                    DataContext = null;
                    DataContext = this;
                    MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                    Tests = context.Test.ToList();
                }
            }
        }
    }
}