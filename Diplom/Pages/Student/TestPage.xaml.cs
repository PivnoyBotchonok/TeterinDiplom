using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Diplom.AppData.Model;

namespace Diplom.Pages.User
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        private int _testId;
        public Test SelectedTest { get; set; }

        public TestPage(int testId)
        {
            InitializeComponent();
            _testId = testId;
            DataContext = this;
            LoadTest();
        }

        private void LoadTest()
        {
            using (var context = TeterinEntities.GetContext())
            {
                // Загрузка теста с вопросами и ответами
                SelectedTest = context.Test.Include("Question.Answer").FirstOrDefault(t => t.ID == _testId);

                if (SelectedTest == null)
                {
                    MessageBox.Show("Тест не найден!");
                    NavigationService?.GoBack();
                }
                else
                {
                    // Присваиваем номер каждому вопросу
                    int questionNumber = 1;
                    foreach (var question in SelectedTest.Question)
                    {
                        question.Number = questionNumber++; // Присваиваем номер
                    }
                }
            }
        }


        private void ResultBut_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTest == null || SelectedTest.Question == null) return;

            int totalQuestions = SelectedTest.Question.Count;
            int correctAnswers = 0;

            foreach (var question in SelectedTest.Question)
            {
                var selected = question.Answer.FirstOrDefault(a => a.Is_Correct);
                if (selected != null && selected.Is_Correct)
                {
                    correctAnswers++;
                }
            }

            double percent = (double)correctAnswers / totalQuestions * 100;

            string resultMessage = $"Результат: {correctAnswers} из {totalQuestions} правильных ответов!\n" +
                                   $"Процент: {percent:0.##}%";

            MessageBox.Show(resultMessage, "Результат теста", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
