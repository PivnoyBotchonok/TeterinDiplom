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
using Diplom.AppData;
using Diplom.AppData.Model;
using Diplom.Pages.Student;

namespace Diplom.Pages.User
{
    /// <summary>
    /// Логика взаимодействия для TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        private Test _currentTest;
        private Dictionary<Question, List<CheckBox>> _questionAnswersMap = new Dictionary<Question, List<CheckBox>>();


        public TestPage(Test test)
        {
            InitializeComponent();
            _currentTest = test;
            LoadTest();
        }

        private void LoadTest()
        {
            TestPanel.Children.Clear();
            var questions = _currentTest.Question.OrderBy(q => q.Number).ToList();

            foreach (var question in questions)
            {
                var qTextBlock = new TextBlock
                {
                    Text = $"{question.Number}. {question.Text}",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };
                TestPanel.Children.Add(qTextBlock);

                var checkBoxes = new List<CheckBox>();
                foreach (var answer in question.Answer)
                {
                    var cb = new CheckBox
                    {
                        Content = answer.Text,
                        Tag = answer,
                        Margin = new Thickness(10, 2, 0, 2)
                    };
                    TestPanel.Children.Add(cb);
                    checkBoxes.Add(cb);
                }
                _questionAnswersMap[question] = checkBoxes;
            }
        }

        private void FinishTest_Click(object sender, RoutedEventArgs e)
        {
            int correctCount = 0;
            int totalQuestions = _questionAnswersMap.Count;

            foreach (var entry in _questionAnswersMap)
            {
                var question = entry.Key;
                var checkboxes = entry.Value;

                var selected = checkboxes.Where(cb => cb.IsChecked == true)
                                         .Select(cb => cb.Tag as Answer).ToList();

                var correct = question.Answer.Where(a => a.Is_Correct).ToList();

                if (selected.Count == correct.Count &&
                    !selected.Except(correct).Any())
                {
                    correctCount++;
                }
            }

            decimal score = (decimal)correctCount / totalQuestions;

            using (var context = new TeterinEntities())
            {
                var result = new Result
                {
                    ID_User = LogClass.user.ID,
                    ID_Test = _currentTest.ID,
                    Score = score
                };
                context.Result.Add(result);
                context.SaveChanges();
            }

            MessageBox.Show($"Вы завершили тест!\nПравильных ответов: {correctCount} из {totalQuestions}\nОценка: {(score > 0.9m ? 5 : score > 0.75m ? 4 : score > 0.5m ? 3 : 2)}");
            MainFrame.mainFrame.Navigate(new StudentMainPage());
        }
    }
}
