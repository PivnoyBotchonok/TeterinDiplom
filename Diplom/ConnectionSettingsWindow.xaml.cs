using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

namespace Diplom
{
    public partial class ConnectionSettingsWindow : Window
    {
        public ConnectionSettingsWindow()
        {
            InitializeComponent();
            AuthTypeComboBox.SelectedIndex = 0; // Выбрать Windows по умолчанию

            // Загрузка текущих настроек
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            try
            {
                // Получаем текущую строку подключения
                var entityConnectionString = ConfigurationManager.ConnectionStrings["TeterinEntities"]?.ConnectionString;

                if (!string.IsNullOrEmpty(entityConnectionString))
                {
                    // Разбираем строку подключения Entity Framework
                    var entityBuilder = new EntityConnectionStringBuilder(entityConnectionString);
                    var sqlBuilder = new SqlConnectionStringBuilder(entityBuilder.ProviderConnectionString);

                    // Заполняем поля формы
                    ServerTextBox.Text = sqlBuilder.DataSource;

                    // Определяем тип аутентификации
                    if (sqlBuilder.IntegratedSecurity)
                    {
                        AuthTypeComboBox.SelectedIndex = 0; // Windows
                    }
                    else
                    {
                        AuthTypeComboBox.SelectedIndex = 1; // SQL
                        UsernameTextBox.Text = sqlBuilder.UserID;
                        // Пароль нельзя получить из строки подключения
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки настроек: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AuthTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (ComboBoxItem)AuthTypeComboBox.SelectedItem;
            SqlAuthPanel.Visibility = selected.Tag.ToString() == "SQL"
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Формируем базовую строку подключения SQL
                var sqlConnectionString = GenerateSqlConnectionString();

                // Проверка подключения
                using (var connection = new SqlConnection(sqlConnectionString))
                {
                    connection.Open();
                }

                // Формируем строку подключения Entity Framework
                var efConnectionString = GenerateEntityFrameworkConnectionString(sqlConnectionString);

                // Сохраняем в конфиг
                SaveConnectionStringToConfig(efConnectionString);

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private string GenerateSqlConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ServerTextBox.Text,
                InitialCatalog = "Diplom_Teterin", // Ваша база данных
                IntegratedSecurity = ((ComboBoxItem)AuthTypeComboBox.SelectedItem).Tag.ToString() == "Windows",
                MultipleActiveResultSets = true,
                Encrypt = true,
                TrustServerCertificate = true
            };

            if (!builder.IntegratedSecurity)
            {
                builder.UserID = UsernameTextBox.Text;
                builder.Password = PasswordBox.Password;
            }

            return builder.ToString();
        }

        private string GenerateEntityFrameworkConnectionString(string sqlConnectionString)
        {
            // Формируем строку подключения Entity Framework
            var entityBuilder = new EntityConnectionStringBuilder
            {
                Metadata = "res://*/AppData.Model.Model1.csdl|res://*/AppData.Model.Model1.ssdl|res://*/AppData.Model.Model1.msl",
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlConnectionString
            };

            return entityBuilder.ToString();
        }

        private void SaveConnectionStringToConfig(string connectionString)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                // Обновляем строку подключения
                var connectionSettings = config.ConnectionStrings.ConnectionStrings["TeterinEntities"];

                if (connectionSettings != null)
                {
                    connectionSettings.ConnectionString = connectionString;
                }
                else
                {
                    config.ConnectionStrings.ConnectionStrings.Add(
                        new ConnectionStringSettings(
                            "TeterinEntities",
                            connectionString,
                            "System.Data.EntityClient"));
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("connectionStrings");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}\n\n" +
                                "Убедитесь, что приложение имеет права на запись в конфигурационный файл.",
                                "Ошибка конфигурации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}