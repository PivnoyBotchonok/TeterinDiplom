using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Diplom.AppData;
using Diplom.AppData.Model;

namespace Diplom.Pages.Admin
{
    public partial class CreateTest : Page
    {
        private ObservableCollection<Question> questions;

        public CreateTest()
        {
            InitializeComponent();
            questions = new ObservableCollection<Question>();
            QuestionsListView.ItemsSource = questions;
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новый вопрос
            var question = new Question
            {
                Number = questions.Count + 1, // Нумерация вопросов
                Answer = new ObservableCollection<Answer>() // Инициализируем коллекцию ответов
            };
            questions.Add(question);
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            // Находим текущий вопрос, к которому добавляется ответ
            var button = sender as Button;
            var question = button?.DataContext as Question;

            if (question != null)
            {
                // Создаем новый ответ
                var answer = new Answer { Text = "", Is_Correct = false };
                question.Answer.Add(answer);
            }
        }

        private void SaveTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, что введено название и описание теста
            if (string.IsNullOrWhiteSpace(TestNameTextBox.Text) || string.IsNullOrWhiteSpace(TestDescriptionTextBox.Text))
            {
                MessageBox.Show("Название и описание теста обязательны для заполнения.");
                return;
            }

            // Проверяем, что есть хотя бы 1 вопрос
            if (questions.Count == 0)
            {
                MessageBox.Show("Тест должен содержать хотя бы 1 вопрос.");
                return;
            }

            // Проверяем каждый вопрос
            foreach (var question in questions)
            {
                // Проверяем, что у вопроса есть хотя бы 2 ответа
                if (question.Answer.Count < 2)
                {
                    MessageBox.Show($"Вопрос {question.Number} должен иметь хотя бы 2 ответа.");
                    return;
                }

                // Проверяем, что у вопроса есть хотя бы 1 правильный ответ
                if (!question.Answer.Any(a => a.Is_Correct))
                {
                    MessageBox.Show($"Вопрос {question.Number} должен содержать хотя бы 1 правильный ответ.");
                    return;
                }
            }

            // Создаем новый тест
            var test = new Test
            {
                ID_User = LogClass.user.ID,
                Name = TestNameTextBox.Text,
                Description = TestDescriptionTextBox.Text,
                Question = new HashSet<Question>(questions)
            };

            // Сохраняем тест в базе данных (предполагается, что контекст базы данных уже настроен)
            using (var context = new TeterinEntities()) // Замените на свой контекст
            {
                context.Test.Add(test);
                context.SaveChanges();
            }

            MessageBox.Show("Тест успешно сохранен!");
            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }

        private void BackBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.GoBack();
        }
    }
}
