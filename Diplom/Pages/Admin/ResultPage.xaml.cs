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
using System.Data.Entity;

namespace Diplom.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        private int _testId;

        public ResultPage(int testId)
        {
            InitializeComponent();
            _testId = testId;

            using (var context = new TeterinEntities())
            {
                var results = context.Result
                    .Include(r => r.User)
                    .Where(r => r.ID_Test == _testId)
                    .ToList();

                ResultDataGrid.ItemsSource = results;
            }
        }

        private void backBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }
    }

}
