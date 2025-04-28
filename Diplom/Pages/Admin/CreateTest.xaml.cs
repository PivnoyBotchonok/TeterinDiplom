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
    /// <summary>
    /// Страница создания/редактирования теста с вопросами и ответами
    /// </summary>
    public partial class CreateTest : Page
    {
        // Коллекция вопросов для текущего теста
        private ObservableCollection<Question> questions;

        // ID теста (null для нового теста)
        private int? testId;

        /// <summary>
        /// Конструктор страницы создания/редактирования теста
        /// </summary>
        /// <param name="testId">ID редактируемого теста (null для нового теста)</param>
        public CreateTest(int? testId = null)
        {
            InitializeComponent();

            // Инициализация коллекции вопросов
            questions = new ObservableCollection<Question>();
            this.testId = testId;

            // Привязка коллекции к ListView
            QuestionsListView.ItemsSource = questions;

            // Если передан ID теста - загружаем его данные для редактирования
            if (testId.HasValue)
            {
                LoadTestForEditing(testId.Value);
            }
        }

        /// <summary>
        /// Загрузка данных теста для редактирования
        /// </summary>
        /// <param name="testId">ID теста</param>
        private void LoadTestForEditing(int testId)
        {
            using (var context = new TeterinEntities())
            {
                // Загрузка теста с вопросами и ответами
                var test = context.Test.Include("Question.Answer").FirstOrDefault(t => t.ID == testId);

                if (test != null)
                {
                    // Установка контекста данных для привязки
                    this.DataContext = test;

                    // Заполнение коллекции вопросами и ответами
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

        /// <summary>
        /// Валидация данных теста перед сохранением
        /// </summary>
        /// <returns>True если данные валидны, иначе False</returns>
        private bool ValidateTestData()
        {
            // Проверка названия и описания теста
            if (string.IsNullOrWhiteSpace(TestNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(TestDescriptionTextBox.Text))
            {
                MessageBox.Show("Название и описание теста обязательны для заполнения.");
                return false;
            }

            // Проверка наличия вопросов
            if (questions.Count == 0)
            {
                MessageBox.Show("Тест должен содержать хотя бы 1 вопрос.");
                return false;
            }

            // Проверка каждого вопроса
            foreach (var question in questions)
            {
                // Проверка количества ответов
                if (question.Answer.Count < 2)
                {
                    MessageBox.Show($"Вопрос {question.Number} должен иметь хотя бы 2 ответа.");
                    return false;
                }

                // Проверка наличия правильного ответа
                if (!question.Answer.Any(a => a.Is_Correct))
                {
                    MessageBox.Show($"Вопрос {question.Number} должен содержать хотя бы 1 правильный ответ.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Сохранение нового теста в базу данных
        /// </summary>
        private void SaveTest()
        {
            using (var context = new TeterinEntities())
            {
                // Создание нового теста
                var test = new Test
                {
                    ID_User = LogClass.user.ID,
                    Name = TestNameTextBox.Text,
                    Description = TestDescriptionTextBox.Text
                };

                // Добавление теста в контекст
                context.Test.Add(test);
                context.SaveChanges(); // Сохранение для получения ID

                // Добавление вопросов и ответов
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

        /// <summary>
        /// Обработчик кнопки добавления вопроса
        /// </summary>
        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание нового вопроса с порядковым номером
            var question = new Question
            {
                Number = questions.Count + 1,
                Answer = new ObservableCollection<Answer>()
            };
            questions.Add(question);
        }

        /// <summary>
        /// Обработчик кнопки добавления ответа
        /// </summary>
        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var question = button?.DataContext as Question;

            if (question != null)
            {
                // Добавление пустого ответа к вопросу
                var answer = new Answer { Text = "", Is_Correct = false };
                question.Answer.Add(answer);
            }
        }

        /// <summary>
        /// Обработчик кнопки сохранения теста
        /// </summary>
        private void SaveTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (!ValidateTestData()) return;

            using (var context = new TeterinEntities())
            {
                if (testId.HasValue)
                {
                    // Редактирование существующего теста
                    var test = context.Test.Include("Question.Answer").FirstOrDefault(t => t.ID == testId.Value);

                    if (test != null)
                    {
                        // Удаление старых вопросов и ответов
                        var oldQuestions = test.Question.ToList();
                        foreach (var q in oldQuestions)
                        {
                            context.Answer.RemoveRange(q.Answer);
                        }
                        context.Question.RemoveRange(oldQuestions);

                        // Обновление данных теста
                        test.Name = TestNameTextBox.Text;
                        test.Description = TestDescriptionTextBox.Text;

                        // Добавление новых вопросов и ответов
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
                    // Создание нового теста
                    SaveTest();
                }
            }

            // Возврат на главную страницу администратора
            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }

        /// <summary>
        /// Обработчик кнопки возврата
        /// </summary>
        private void BackBut_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.mainFrame.Navigate(new AdminMainPage());
        }
    }
}