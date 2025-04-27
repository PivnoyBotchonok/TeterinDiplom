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
using Diplom.Pages.Admin;
using Diplom.Pages.Student;

namespace Diplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для LogPage.xaml
    /// </summary>
    public partial class LogPage : Page
    {
        public LogPage()
        {
            InitializeComponent();
        }

        private void EntryBut_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = TeterinEntities.GetContext().User
                    .FirstOrDefault(u => u.Login == Login.Text && u.Password == Password.Text);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Добро пожаловать, {user.SName} {user.FName}!", "Успешный вход", MessageBoxButton.OK, MessageBoxImage.Information);
                LogClass.user = user;

                // Пример перехода по ролям
                switch (user.ID_Role)
                {
                    case 1: // Администратор
                        MainFrame.mainFrame.Navigate(new AdminMainPage());
                        break;
                    case 2: // Студент
                        MainFrame.mainFrame.Navigate(new StudentMainPage());
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при входе: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new RegPage());
        }
    }
}
