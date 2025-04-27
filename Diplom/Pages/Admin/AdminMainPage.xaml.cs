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
            MyTests = TeterinEntities.GetContext().Test.Where(x=>x.ID_User == LogClass.user.ID).ToList();
            Tests = TeterinEntities.GetContext().Test.ToList();
        }

        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.GoBack();
        }

        private void EditTestButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Test selectedTest)
            {
                MainFrame.mainFrame.Navigate(new CreateTest());
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

                                // Обновляем списки
                                MyTests = context.Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                                Tests = context.Test.ToList();
                                DataContext = null;
                                DataContext = this;
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

        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                using (var context = new TeterinEntities())
                {
                    TeterinEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(entry => entry.Reload());
                    DataContext = null;
                    DataContext = this;
                    MyTests = TeterinEntities.GetContext().Test.Where(x => x.ID_User == LogClass.user.ID).ToList();
                    Tests = TeterinEntities.GetContext().Test.ToList();
                }
            }
        }
    }
}
