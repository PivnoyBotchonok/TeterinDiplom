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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        public List<Test> MyTests { get; set; }
        public List<Test> Tests { get; set; }
        public AdminMainPage()
        {
            InitializeComponent();
            DataContext = this;
            using (var context = new TeterinEntities()) 
            {
                MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                StudentTable.ItemsSource = context.User.Where(x=>x.ID_Role == 2).ToList();
            }
        }

        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new LogPage());
        }

        private void EditTestButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var test = button?.DataContext as Test;

            if (test != null)
            {
                // Передаем ID теста для редактирования
                var createTestPage = new CreateTest(test.ID); // Передаем ID теста
                MainFrame.mainFrame.Navigate(createTestPage);
            }
        }
        private void DeleteTestButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Test selectedTest)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить тест '{selectedTest.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new TeterinEntities())
                    {
                        try
                        {
                            var testToDelete = context.Test.FirstOrDefault(t => t.ID == selectedTest.ID);
                            if (testToDelete != null)
                            {
                                var results = context.Result.Where(r => r.ID_Test == testToDelete.ID).ToList();
                                context.Result.RemoveRange(results);

                                var questions = context.Question.Where(q => q.ID_Test == testToDelete.ID).ToList();

                                foreach (var q in questions)
                                {
                                    var answers = context.Answer.Where(a => a.ID_Question == q.ID);
                                    context.Answer.RemoveRange(answers);
                                }

                                context.Question.RemoveRange(questions);
                                context.Test.Remove(testToDelete);
                                context.SaveChanges();

                                MessageBox.Show("Тест удалён.");

                                // 🔄 Обновление списка тестов
                                MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                                Tests = context.Test.ToList();

                                // 🔁 Обновляем привязку
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


        private void AddTestButton_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new CreateTest());
        }

        private void ResultBut_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var test = button?.DataContext as Test;

            if (test != null)
            {
                MainFrame.mainFrame.Navigate(new ResultPage(test.ID));
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                using (var context = new TeterinEntities())
                {
                    context.ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                    DataContext = null;
                    DataContext = this;
                    MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                    Tests = context.Test.ToList();
                }
            }
        }
    }
}
