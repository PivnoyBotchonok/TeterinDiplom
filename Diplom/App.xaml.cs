using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Diplom.AppData;

namespace Diplom
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (AppSettings.IsFirstRun)
            {
                ShowConnectionSettings();
            }
            else
            {
                StartMainApplication();
            }
        }

        private void ShowConnectionSettings()
        {
            var settingsWindow = new ConnectionSettingsWindow();
            if (settingsWindow.ShowDialog() == true)
            {
                AppSettings.IsFirstRun = false;
                StartMainApplication();
            }
            else
            {
                // Пользователь закрыл окно без сохранения
                Shutdown();
            }
        }

        private void StartMainApplication()
        {
            try
            {
                // Проверка подключения к БД
                using (var context = new AppData.Model.TeterinEntities())
                {
                    context.Database.Connection.Open();
                    context.Database.Connection.Close();
                }

                var mainWindow = new MainWindow();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                // Сохраняем результат диалогового окна
                var result = MessageBox.Show($"Ошибка подключения к БД: {ex.Message}\n\n" +
                                            "Хотите изменить настройки подключения?",
                                            "Ошибка подключения",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Error);

                // Проверяем результат
                if (result == MessageBoxResult.Yes)
                {
                    AppSettings.IsFirstRun = true;
                    ShowConnectionSettings();
                }
                else
                {
                    Shutdown();
                }
            }
        }
    }
}
