using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Diplom.AppData;
using Diplom.AppData.Model;

namespace Diplom.Pages
{
    /// <summary>
    /// Страница регистрации новых пользователей в системе
    /// </summary>
    public partial class RegPage : Page
    {
        /// <summary>
        /// Конструктор страницы регистрации
        /// </summary>
        public RegPage()
        {
            InitializeComponent();

            // Загрузка списка ролей из базы данных в выпадающий список
            CmbBox.ItemsSource = TeterinEntities.GetContext()
                .Role
                .Select(x => x.RoleName)
                .ToList();
        }

        /// <summary>
        /// Обработчик нажатия кнопки регистрации
        /// </summary>
        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения всех обязательных полей
            if (string.IsNullOrWhiteSpace(SName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(FName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(PName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(Login_TextBox.Text) ||
                string.IsNullOrWhiteSpace(Pass_TextBox.Text) ||
                CmbBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Получаем выбранную роль из ComboBox
                var selectedRoleName = CmbBox.SelectedItem.ToString();

                // Поиск роли в базе данных
                var role = TeterinEntities.GetContext()
                    .Role
                    .FirstOrDefault(r => r.RoleName == selectedRoleName);

                if (role == null)
                {
                    MessageBox.Show("Роль не найдена!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на существование пользователя с таким же логином
                if (TeterinEntities.GetContext()
                    .User
                    .Any(u => u.Login == Login_TextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создание нового пользователя
                var newUser = new Diplom.AppData.Model.User
                {
                    FName = FName_TextBox.Text,       // Имя
                    SName = SName_TextBox.Text,       // Фамилия
                    PName = PName_TextBox.Text,       // Отчество
                    Login = Login_TextBox.Text,       // Логин
                    Password = Pass_TextBox.Text,     // Пароль
                    ID_Role = role.ID                 // ID роли
                };

                // Добавление пользователя в базу данных
                TeterinEntities.GetContext().User.Add(newUser);
                TeterinEntities.GetContext().SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очистка полей формы после успешной регистрации
                ClearRegistrationForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Очищает поля формы регистрации
        /// </summary>
        private void ClearRegistrationForm()
        {
            FName_TextBox.Clear();
            SName_TextBox.Clear();
            PName_TextBox.Clear();
            Login_TextBox.Clear();
            Pass_TextBox.Clear();
            CmbBox.SelectedIndex = -1;
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Назад"
        /// </summary>
        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.GoBack();
        }

        /// <summary>
        /// Обработчик ввода текста - разрешает ввод только букв
        /// </summary>
        private void LettersOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Регулярное выражение для проверки ввода только букв (русских и английских)
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Zа-яА-Я]+$"))
            {
                e.Handled = true; // Блокировка ввода недопустимых символов
            }
        }
    }
}