using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MusicQuiz.Application.Interfaces;
using MusicQuiz.Core.Entities;
using MusicQuiz.Core.Enums;
using MusicQuiz.Core.Migrations;
using MusicQuiz.Web.Models.Home;
using MusicQuiz.Web.Models.Quiz;
using System.Text.Json;

namespace MusicQuiz.Web.Controllers
{
    public class QuizController(ApplicationDbContext context,
        IResultsService resultsService, UserManager<UserData> userManager) : BaseController
    {
        /// <summary>
        /// Get user ID
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetUserIdAsync()
        {
            var user = await userManager.GetUserAsync(User);
            return user?.Id ?? "0";
        }

        /// <summary>
        /// Index action method for the Quiz
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            ClearQuizSession();

            var quiz = new MusicQuizViewModel();
            return View(quiz);
        }

        /// <summary>
        /// Clear the quiz session
        /// </summary>
        private void ClearQuizSession()
        {
            HttpContext.Session.Remove("QuizQuestions");
            HttpContext.Session.Remove("CurrentQuestionIndex");
            HttpContext.Session.Remove("Score");
            HttpContext.Session.Remove("CorrectAnswers");
        }

        /// <summary>
        /// Select music topic
        /// </summary>
        /// <param name="selectedTopic"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SelectDifficulty(string selectedTopic)
        {
            if (string.IsNullOrEmpty(selectedTopic))
            {
                // Handle the case where selectedTopic is null or empty
                return BadRequest("Selected topic is required.");
            }

            // Store the selected topic in TempData
            TempData["SelectedTopic"] = selectedTopic;
            return RedirectToAction("SelectDifficulty");
        }

        /// <summary>
        /// Select difficulty
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SelectDifficulty(MusicQuizViewModel model)
        {
            if (TempData["SelectedTopic"] != null)
            {
                var selectedTopicString = TempData["SelectedTopic"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrEmpty(selectedTopicString) && Enum.TryParse(typeof(Topic), selectedTopicString, out var selectedTopic))
                {
                    model.SelectedTopic = (Topic)selectedTopic;
                }
                else
                {
                    return BadRequest("Invalid topic selected.");
                }
            }

            return View(model);
        }

        /// <summary>
        /// starting quiz
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Start(MusicQuizViewModel model)
        {
            if (model.SelectedTopic == default || model.SelectedDifficulty == default)
            {
                return BadRequest("Selected topic and difficulty are required.");
            }

            var questions = context.QuizQuestions
                .Where(q => q.TopicId == (int)model.SelectedTopic && q.DifficultyId == (int)model.SelectedDifficulty)
                .ToList();

            if (questions.Count == 0)
            {
                return View("NoQuestions");
            }

            // Select 10 random questions
            var random = new Random();

            var selectedQuestions = questions.OrderBy(q => random.Next()).Take((int)QuestionQuantity.Quiz).ToList();

            var questionViewModels = selectedQuestions.Select(q =>
            {
                //List options
                var options = new List<string> { q.CorrectAnswer, q.WrongAnswerOne, q.WrongAnswerTwo, q.WrongAnswerThree };

                //Randomize order so users cannot predict answer locations
                options = [.. options.OrderBy(x => random.Next())];

                return new QuestionViewModel
                {
                    SelectedTopic = model.SelectedTopic,
                    SelectedDifficulty = model.SelectedDifficulty,
                    Question = q.Question,
                    MusicQuestionFilePath = q.QuestionMusicFilePath,
                    MusicReferenceFilePath = q.ReferenceMusicFilePath,
                    MusicReferenceName = Path.GetFileNameWithoutExtension(q.ReferenceMusicFilePath),
                    OptionsForQuiz = options,
                    CorrectAnswer = q.CorrectAnswer,
                    AttemptNumber = 0,
                    EXP = 0
                };
            }).ToList();

            HttpContext.Session.SetString("QuizQuestions", JsonSerializer.Serialize(questionViewModels));
            HttpContext.Session.SetInt32("CurrentQuestionIndex", 0);

            return RedirectToAction("ShowQuestion");
        }

        /// <summary>
        /// Displays the current question.
        /// </summary>
        /// <returns></returns>
        public IActionResult ShowQuestion()
        {
            var questionsJson = HttpContext.Session.GetString("QuizQuestions");
            var questions = questionsJson != null ? JsonSerializer.Deserialize<List<QuestionViewModel>>(questionsJson) ?? [] : [];
            var currentIndex = HttpContext.Session.GetInt32("CurrentQuestionIndex") ?? 0;
            var answer = questions[currentIndex].UserAnswer;
            var scoreString = HttpContext.Session.GetString("Score");
            decimal score = 0;
            if (!string.IsNullOrEmpty(scoreString))
            {
                _ = decimal.TryParse(scoreString, out score);
            }

            if (questions == null || questions.Count == 0)
            {
                return View("NoQuestions");
            }

            // Update question statuses for all questions
            var questionStatuses = questions.Select(q =>
            {
                if (!q.IsAnswered)
                {
                    return "unanswered";
                }
                else if (q.FirstAnswer == q.CorrectAnswer && q.AttemptNumber == 1)
                {
                    return "correct-first";
                }
                else if (q.UserAnswer == q.CorrectAnswer && q.AttemptNumber > 1 && q.AttemptNumber < 4)
                {
                    return "correct-multiple";
                }
                else
                {
                    return "incorrect";
                }
            }).ToList();

            ViewBag.QuestionStatuses = questionStatuses;

            var model = questions[currentIndex];
            ViewBag.CurrentQuestionIndex = currentIndex;
            ViewBag.TotalQuestions = questions.Count;
            ViewBag.Score = score;
            ViewBag.userAnswer = answer;
            ViewBag.Feedback = questions[currentIndex].Feedback;

            return View(model);
        }

        /// <summary>
        /// Show the next question
        /// </summary>
        /// <param name="selectedOption"></param>
        /// <param name="attemptNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult NextQuestion(string selectedOption, int attemptNumber)
        {
            var questionsJson = HttpContext.Session.GetString("QuizQuestions");
            var questions = questionsJson != null ? JsonSerializer.Deserialize<List<QuestionViewModel>>(questionsJson) : [];
            var currentIndex = HttpContext.Session.GetInt32("CurrentQuestionIndex") ?? 0;

            if (questions == null || questions.Count == 0)
            {
                return View("NoQuestions");
            }

            SaveUserAnswer(questions, currentIndex, selectedOption, attemptNumber);

            currentIndex++;
            if (currentIndex >= questions.Count)
            {
                // Finish the quiz
                return RedirectToAction("QuizResults");
            }

            HttpContext.Session.SetInt32("CurrentQuestionIndex", currentIndex);

            return RedirectToAction("ShowQuestion");
        }

        /// <summary>
        /// Save user answer to the HTTPcontext
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="currentIndex"></param>
        /// <param name="selectedOption"></param>
        /// <param name="attemptNumber"></param>
        private void SaveUserAnswer(List<QuestionViewModel> questions, int currentIndex, string selectedOption, int attemptNumber)
        {
            if (string.IsNullOrEmpty(selectedOption))
            {
                selectedOption = "Not answered";
            }

            var currentQuestion = questions[currentIndex];

            if (string.IsNullOrEmpty(currentQuestion.FirstAnswer))
            {
                currentQuestion.FirstAnswer = selectedOption;
            }

            var correctAnswers = HttpContext.Session.GetInt32("CorrectAnswers") ?? 0;

            // Check if the previous answer was correct
            var wasPreviouslyCorrect = currentQuestion.UserAnswer == currentQuestion.CorrectAnswer;

            currentQuestion.AttemptNumber = attemptNumber;
            currentQuestion.UserAnswer = selectedOption;

            if (currentQuestion.IsAnswered)
            {
                if (selectedOption != currentQuestion.CorrectAnswer)
                {
                    currentQuestion.Feedback = "Try again. ✘";

                    if (wasPreviouslyCorrect)
                    {
                        correctAnswers--;
                    }
                }
                else
                {
                    currentQuestion.Feedback = "Correct! ✔";
                    if (attemptNumber < 4 && !wasPreviouslyCorrect)
                    {
                        correctAnswers++;
                    }
                }
            }
            else
            {
                if (selectedOption == currentQuestion.CorrectAnswer)
                {
                    currentQuestion.Feedback = "Correct! ✔";
                    correctAnswers++;
                }
                else
                {
                    currentQuestion.Feedback = "Try again. ✘";
                    attemptNumber++;
                }
                currentQuestion.IsAnswered = true;
            }

            // Save changes to session
            //WORKING OUT SCORE
            decimal score;

            if (currentIndex == 0)
            {
                score = 0;
            }
            else
            {
                var scoreString = HttpContext.Session.GetString("Score");
                score = scoreString != null ? decimal.Parse(scoreString) : 0;
            }

            currentQuestion.AttemptNumber = attemptNumber;

            decimal scoreForQuestion;

            switch (attemptNumber)
            {
                case 1:
                    currentQuestion.EXP = (int)MusicQuiz.Core.Enums.EXP.One;
                    scoreForQuestion = Math.Round((decimal)100 / questions.Count, 2);
                    break;
                case 2:
                    currentQuestion.EXP = (int)MusicQuiz.Core.Enums.EXP.Two;
                    scoreForQuestion = Math.Round(((decimal)100 / questions.Count) / 2, 2);
                    break;
                case 3:
                    currentQuestion.EXP = (int)MusicQuiz.Core.Enums.EXP.Three;
                    scoreForQuestion = Math.Round(((decimal)100 / questions.Count) / 3, 2);
                    break;
                default:
                    currentQuestion.EXP = (int)MusicQuiz.Core.Enums.EXP.Default;
                    scoreForQuestion = 0;
                    break;
            }

            score += currentQuestion.UserAnswer == currentQuestion.CorrectAnswer ? scoreForQuestion : 0;

            HttpContext.Session.SetString("Score", score.ToString());
            HttpContext.Session.SetInt32("CorrectAnswers", correctAnswers);
            HttpContext.Session.SetString("QuizQuestions", JsonSerializer.Serialize(questions));
        }

        /// <summary>
        /// Return to previous question in the list
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PreviousQuestion()
        {
            var questionsJson = HttpContext.Session.GetString("QuizQuestions");
            var questions = questionsJson != null ? JsonSerializer.Deserialize<List<QuestionViewModel>>(questionsJson) : [];
            var currentIndex = HttpContext.Session.GetInt32("CurrentQuestionIndex") ?? 0;

            if (questions == null || questions.Count == 0)
            {
                return View("NoQuestions");
            }

            // Move to the previous question
            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            HttpContext.Session.SetInt32("CurrentQuestionIndex", currentIndex);

            ViewBag.Feedback = questions[currentIndex].Feedback;
            ViewBag.FirstUserAnswer = questions[currentIndex].FirstAnswer;

            return RedirectToAction("ShowQuestion");
        }

        /// <summary>
        /// Display the quiz results
        /// </summary>
        /// <returns></returns>
        public IActionResult QuizResults()
        {
            var questionsJson = HttpContext.Session.GetString("QuizQuestions");
            var questions = questionsJson != null ? JsonSerializer.Deserialize<List<QuestionViewModel>>(questionsJson) ?? [] : [];

            var scoreString = HttpContext.Session.GetString("Score");

            var rightFirstTime = 0;
            var rightSecondTime = 0;
            var rightThirdTime = 0;
            var rightFourthTime = 0;
            var incorrectAnswers = 0;

            decimal score = 0;

            decimal exp = 0;

            if (!string.IsNullOrEmpty(scoreString))
            {
                score = decimal.TryParse(scoreString, out var parsedScore) ? parsedScore : 0;
            }

            // Loop through all the questions and calculate the counts
            foreach (var question in questions)
            {
                if (question.UserAnswer == question.CorrectAnswer && question.AttemptNumber == 1)
                {
                    rightFirstTime++;
                }
                else if (question.UserAnswer == question.CorrectAnswer && question.AttemptNumber > 1)
                {
                    switch (question.AttemptNumber)
                    {
                        case 2:
                            rightSecondTime++;
                            break;
                        case 3:
                            rightThirdTime++;
                            break;
                        case 4:
                            rightFourthTime++;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    incorrectAnswers++;
                }

                exp += question.EXP;
            }

            var model = new QuizResultsViewModel
            {
                TotalQuestions = questions.Count,
                Score = score,
                Questions = questions,
                DateOfSubmission = DateTime.Now
            };

            //ViewBag to populate JS
            ViewBag.Score = score;
            ViewBag.TotalQuestions = questions.Count;
            ViewBag.RightFirstTime = rightFirstTime;
            ViewBag.RightSecondTime = rightSecondTime;
            ViewBag.RightThirdTime = rightThirdTime;
            ViewBag.RightFourthTime = rightFourthTime;
            ViewBag.IncorrectAnswers = incorrectAnswers;
            ViewBag.DateOfSubmission = model.DateOfSubmission.ToString("dd/MM/yyyy HH:mm:ss");

            var userID = GetUserIdAsync().Result;

            if (questions.Count != 0)
            {
                decimal percentage = score;

                var firstQuestion = questions.First();
                resultsService.SaveQuizResults(percentage, model.DateOfSubmission, (int)firstQuestion.SelectedDifficulty, (int)firstQuestion.SelectedTopic, userID);

                // Update user's EXP
                var user = userManager.FindByIdAsync(userID).Result;

                if (user != null)
                {
                    int expGained = CalculateExpGained(exp, (int)firstQuestion.SelectedDifficulty);
                    user.EXP += expGained;
                    userManager.UpdateAsync(user).Wait();
                }
            }

            return View(model);
        }

        /// <summary>
        /// Calculate the EXP gained depending on the difficulty level
        /// </summary>
        /// <param name="expGained"></param>
        /// <param name="difficultyLevel"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static int CalculateExpGained(decimal exp, int difficultyLevel)
        {

            switch (difficultyLevel)
            {
                case (int)DifficultyLevel.Easy:
                    // No change to expGained
                    break;
                case (int)DifficultyLevel.Medium:
                    //Double exp for medium difficulty
                    exp *= 2;
                    break;
                case (int)DifficultyLevel.Hard:
                    //Triple exp for hard difficulty
                    exp *= 3;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(difficultyLevel), difficultyLevel, null);
            }
            return (int)exp;
        }
    }
}
