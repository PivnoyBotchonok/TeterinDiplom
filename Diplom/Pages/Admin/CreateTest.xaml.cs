using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Diplom.AppData.Model;

namespace Diplom.Pages.Admin
{
    /// <summary>
    /// Логика взаимодействия для CreateTest.xaml
    /// </summary>
    public partial class CreateTest : Page
    {
        public ObservableCollection<QuestionViewModel> Questions { get; set; } = new ObservableCollection<QuestionViewModel>();

        public CreateTest()
        {
            InitializeComponent();
            QuestionsListView.ItemsSource = Questions;
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            Questions.Add(new QuestionViewModel());
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is QuestionViewModel question)
            {
                question.Answers.Add(new AnswerViewModel());
            }
        }

        private void SaveTestButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = TeterinEntities.GetContext()) // Создаем один контекст для всего процесса
            {
                // Создаем новый тест
                var test = new Test
                {
                    ID_User = 1, // Можно заменить на актуального пользователя
                    Name = TestNameTextBox.Text,
                    Description = TestDescriptionTextBox.Text
                };

                // Добавляем тест в контекст
                context.Test.Add(test);

                // Сохраняем вопросы и ответы для этого теста
                foreach (var q in Questions)
                {
                    var newQuestion = new Question
                    {
                        ID_Test = test.ID,
                        Text = q.Text
                    };
                    context.Question.Add(newQuestion);

                    foreach (var a in q.Answers)
                    {
                        var newAnswer = new Answer
                        {
                            ID_Question = newQuestion.ID,
                            Text = a.Text,
                            Is_Correct = a.IsCorrect
                        };
                        context.Answer.Add(newAnswer);
                    }
                }

                // Теперь сохраняем все изменения в базе данных за один раз
                context.SaveChanges();

                MessageBox.Show("Тест успешно сохранен!");
            }
        }

        public class QuestionViewModel
        {
            public string Text { get; set; }

            // Используем ObservableCollection для автоматического обновления UI
            public ObservableCollection<AnswerViewModel> Answers { get; set; }

            public QuestionViewModel()
            {
                Answers = new ObservableCollection<AnswerViewModel>();
            }
        }

        public class AnswerViewModel
        {
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
        }
    }
}
