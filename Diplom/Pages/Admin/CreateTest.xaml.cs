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
        private int? testId;

        // Конструктор, принимающий ID теста для редактирования
        public CreateTest(int? testId = null)
        {
            InitializeComponent();
            questions = new ObservableCollection<Question>();
            this.testId = testId;
            QuestionsListView.ItemsSource = questions;

            // Если передан ID теста, загружаем тест для редактирования
            if (testId.HasValue)
            {
                LoadTestForEditing(testId.Value);
            }
        }

        private void LoadTestForEditing(int testId)
        {
            using (var context = new TeterinEntities()) // Замените на ваш контекст
            {
                // Находим тест по ID
                var test = context.Test.Include("Question.Answer").FirstOrDefault(t => t.ID == testId);

                if (test != null)
                {
                    // Устанавливаем DataContext для привязки
                    this.DataContext = test;

                    // Загружаем вопросы и ответы в коллекцию
                    foreach (var question in test.Question)
                    {
                        questions.Add(new Question
                        {
                            Number = question.Number,
                            Text = question.Text,
                            Answer = new ObservableCollection<Answer>(question.Answer)
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Тест не найден.");
                }
            }
        }

        // Проверка данных перед сохранением
        private bool ValidateTestData()
        {
            if (string.IsNullOrWhiteSpace(TestNameTextBox.Text) || string.IsNullOrWhiteSpace(TestDescriptionTextBox.Text))
            {
                MessageBox.Show("Название и описание теста обязательны для заполнения.");
                return false;
            }

            if (questions.Count == 0)
            {
                MessageBox.Show("Тест должен содержать хотя бы 1 вопрос.");
                return false;
            }

            foreach (var question in questions)
            {
                if (question.Answer.Count < 2)
                {
                    MessageBox.Show($"Вопрос {question.Number} должен иметь хотя бы 2 ответа.");
                    return false;
                }

                if (!question.Answer.Any(a => a.Is_Correct))
                {
                    MessageBox.Show($"Вопрос {question.Number} должен содержать хотя бы 1 правильный ответ.");
                    return false;
                }
            }

            return true;
        }

        // Сохранение нового теста в базу данных
        private void SaveTest()
        {
            using (var context = new TeterinEntities())
            {
                var test = new Test
                {
                    ID_User = LogClass.user.ID,
                    Name = TestNameTextBox.Text,
                    Description = TestDescriptionTextBox.Text
                };

                context.Test.Add(test);
                context.SaveChanges(); // Сначала сохраняем сам тест, чтобы получить его ID

                foreach (var question in questions)
                {
                    var newQuestion = new Question
                    {
                        Text = question.Text,
                        Number = question.Number,
                        ID_Test = test.ID,
                        Answer = new HashSet<Answer>(question.Answer.Select(a => new Answer
                        {
                            Text = a.Text,
                            Is_Correct = a.Is_Correct
                        }))
                    };

                    context.Question.Add(newQuestion);
                }

                context.SaveChanges();
                MessageBox.Show("Тест успешно сохранен!");
                MainFrame.mainFrame.Navigate(new AdminMainPage());
            }
        }
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            var question = new Question
            {
                Number = questions.Count + 1,
                Answer = new ObservableCollection<Answer>()
            };
            questions.Add(question);
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var question = button?.DataContext as Question;

            if (question != null)
            {
                var answer = new Answer { Text = "", Is_Correct = false };
                question.Answer.Add(answer);
            }
        }

        private void SaveTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка данных
            if (!ValidateTestData()) return;

            using (var context = new TeterinEntities())
            {
                if (testId.HasValue)
                {
                    // Загружаем тест из текущего контекста (не передаем старый объект!)
                    var test = context.Test.Include("Question.Answer").FirstOrDefault(t => t.ID == testId.Value);

                    if (test != null)
                    {
                        // Удаляем старые вопросы и ответы из контекста
                        var oldQuestions = test.Question.ToList();
                        foreach (var q in oldQuestions)
                        {
                            context.Answer.RemoveRange(q.Answer);
                        }
                        context.Question.RemoveRange(oldQuestions);

                        // Обновляем поля теста
                        test.Name = TestNameTextBox.Text;
                        test.Description = TestDescriptionTextBox.Text;

                        // Добавляем новые вопросы и ответы
                        foreach (var question in questions)
                        {
                            var newQuestion = new Question
                            {
                                Text = question.Text,
                                Number = question.Number,
                                ID_Test = test.ID,
                                Answer = new HashSet<Answer>(question.Answer.Select(a => new Answer
                                {
                                    Text = a.Text,
                                    Is_Correct = a.Is_Correct
                                }))
                            };

                            context.Question.Add(newQuestion);
                        }

                        context.SaveChanges();
                        MessageBox.Show("Тест успешно обновлен!");
                    }
                    else
                    {
                        MessageBox.Show("Тест не найден.");
                    }
                }
                else
                {
                    SaveTest();
                }
            }

            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }

        private void BackBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }
    }
}
