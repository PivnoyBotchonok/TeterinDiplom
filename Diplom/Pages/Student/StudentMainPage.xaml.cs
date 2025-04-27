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
using Diplom.Pages.User;

namespace Diplom.Pages.Student
{
    /// <summary>
    /// Логика взаимодействия для StudentMainPage.xaml
    /// </summary>
    public partial class StudentMainPage : Page
    {
        public List<Test> Tests { get; set; }
        public StudentMainPage()
        {
            InitializeComponent();
            DataContext = this;
            Tests = TeterinEntities.GetContext().Test.ToList();
        }

        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.GoBack();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, по которой кликнули
            Button button = sender as Button;

            // Получаем тест из DataContext кнопки (это будет текущий элемент списка)
            Test selectedTest = button.DataContext as Test;

            if (selectedTest != null)
            {
                // Переход на страницу теста, передаём ID
                MainFrame.mainFrame.Navigate(new TestPage(selectedTest.ID));
            }
        }
    }
}
