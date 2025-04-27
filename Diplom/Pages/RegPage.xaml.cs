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

namespace Diplom.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            CmbBox.ItemsSource = TeterinEntities.GetContext().Role.Select(x=>x.RoleName).ToList();
        }

        private void regBut_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrWhiteSpace(SName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(FName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(PName_TextBox.Text) ||
                string.IsNullOrWhiteSpace(Login_TextBox.Text) ||
                string.IsNullOrWhiteSpace(Pass_TextBox.Text) ||
                CmbBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var selectedRoleName = CmbBox.SelectedItem.ToString();

                // Получаем ID роли по названию
                var role = TeterinEntities.GetContext().Role.FirstOrDefault(r => r.RoleName == selectedRoleName);
                if (role == null)
                {
                    MessageBox.Show("Роль не найдена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Проверка на уникальность логина
                if (TeterinEntities.GetContext().User.Any(u => u.Login == Login_TextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Создаем нового пользователя
                var newUser = new Diplom.AppData.Model.User
                {
                    FName = FName_TextBox.Text,
                    SName = SName_TextBox.Text,
                    PName = PName_TextBox.Text,
                    Login = Login_TextBox.Text,
                    Password = Pass_TextBox.Text,
                    ID_Role = role.ID
                };

                // Добавляем в базу
                TeterinEntities.GetContext().User.Add(newUser);
                TeterinEntities.GetContext().SaveChanges();

                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очищаем поля после регистрации
                FName_TextBox.Clear();
                SName_TextBox.Clear();
                PName_TextBox.Clear();
                Login_TextBox.Clear();
                Pass_TextBox.Clear();
                CmbBox.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при регистрации: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.GoBack();
        }
        private void LettersOnlyTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, является ли введенный символ буквой
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Zа-яА-Я]+$"))
            {
                // Если символ не буква, отменяем ввод
                e.Handled = true;
            }
        }
    }
}
