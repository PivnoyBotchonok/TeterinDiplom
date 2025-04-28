using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Diplom.AppData;
using Diplom.AppData.Model;
using Diplom.Pages.Admin;
using Diplom.Pages.Student;

namespace Diplom.Pages
{
    /// <summary>
    /// Страница авторизации пользователей в системе
    /// </summary>
    public partial class LogPage : Page
    {
        /// <summary>
        /// Конструктор страницы авторизации
        /// </summary>
        public LogPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик нажатия кнопки входа в систему
        /// </summary>
        private void EntryBut_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей логина и пароля
            if (string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Поиск пользователя в базе данных по логину и паролю
                var user = TeterinEntities.GetContext().User
                    .FirstOrDefault(u => u.Login == Login.Text && u.Password == Password.Text);

                // Проверка существования пользователя
                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Приветствие успешно авторизованного пользователя
                MessageBox.Show($"Добро пожаловать, {user.SName} {user.FName}!", "Успешный вход",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Сохранение данных авторизованного пользователя
                LogClass.user = user;

                // Перенаправление в зависимости от роли пользователя
                switch (user.ID_Role)
                {
                    case 1: // Администратор
                        MainFrame.mainFrame.Navigate(new AdminMainPage());
                        break;
                    case 2: // Студент
                        MainFrame.mainFrame.Navigate(new StudentMainPage());
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок при авторизации
                MessageBox.Show("Ошибка при входе: " + ex.Message, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки регистрации
        /// </summary>
        private void RegBut_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации
            MainFrame.mainFrame.Navigate(new RegPage());
        }
    }
}