using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicQuiz.Core.Migrations;
using MusicQuiz.Web.Models.Assessment;
using MusicQuiz.Core.Enums;
using MusicQuiz.Web.Models.Quiz;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MusicQuiz.Core.Entities;
using MusicQuiz.Application.Interfaces;

namespace MusicQuiz.Web.Controllers
{
    [Authorize]
    public class AssessmentController(ApplicationDbContext context, UserManager<UserData> userManager, IResultsService resultsService, IAssessmentService assessmentService) : BaseController
    {
        /// <summary>
        /// Assessment Index page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var userID = await GetUserIdAsync();
            var user = await userManager.GetUserAsync(User);
            var isUserVerified = user != null && await userManager.IsEmailConfirmedAsync(user);
            var userAcademicYear = await GetCurrentUserAcademicYearAsync();

            string year = userAcademicYear;

            // Fetch assessments matching the user's academic year.
            var assessments = await context.Assessments
                .Where(a => a.AcademicYear == year)
                .OrderBy(a => a.OpenFrom)
                .ToListAsync();

            // Fetch completed assessments for this user
            var completedAssessments = await context.UsersPracticeQuizResults
                .Where(q => q.UserID == userID)
                .Select(q => q.AssessmentId)
                .ToListAsync();

            // Create the ViewModel with filtered assessments and completion check.
            var viewModel = assessments.Select(a => new AssessmentViewModel
            {
                ID = a.ID,
                AcademicYear = a.AcademicYear,
                OpenFrom = a.OpenFrom,
                OpenTo = a.OpenTo,
                TopicName = (Topic)a.TopicId,
                // Lock assessments if user is not verified, regardless of dates
                IsUnlocked = isUserVerified && DateTime.Now >= a.OpenFrom && DateTime.Now <= a.OpenTo,
                IsCompleted = completedAssessments.Contains(a.ID),
                IsUserVerified = isUserVerified
            }).ToList();

            // Set notification for unverified users
            if (!isUserVerified)
            {
                ViewBag.VerificationMessage = "Your email address has not been verified. Please check your email for a verification link to access assessments.";
            }

            return View(viewModel);
        }

        /// <summary>
        /// Start the assessment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> StartAssessment(int id)
        {
            // Check if user is verified
            var user = await userManager.GetUserAsync(User);
            var isUserVerified = user != null && await userManager.IsEmailConfirmedAsync(user);

            if (!isUserVerified)
            {
                TempData["ErrorMessage"] = "You must verify your email address before accessing assessments. Please check your email for a verification link.";
                return RedirectToAction("Index");
            }

            // Find the assessment by ID.
            var assessment = await context.Assessments.FindAsync(id);

            if (assessment == null || DateTime.Now < assessment.OpenFrom || DateTime.Now > assessment.OpenTo)
            {
                TempData["ErrorMessage"] = "You cannot start this assessment at this time.";
                return RedirectToAction("Index");
            }

            // Rest of the method remains unchanged...
            HttpContext.Session.SetInt32("AssessmentID", id);

            var questions = context.QuizQuestions
                .Where(q => q.TopicId == (int)assessment.TopicId && q.DifficultyId == (int)DifficultyLevel.Hard)
                .ToList();

            if (questions.Count == 0)
            {
                return View("NoQuestions");
            }

            // Select 10 random questions
            var random = new Random();
            var selectedQuestions = questions.OrderBy(q => random.Next()).Take((int)QuestionQuantity.Assessment).ToList();

            var questionViewModels = selectedQuestions.Select(q =>
            {
                var options = new List<string> { q.CorrectAnswer, q.WrongAnswerOne, q.WrongAnswerTwo, q.WrongAnswerThree };
                options = [.. options.OrderBy(x => random.Next())];

                return new QuestionViewModel
                {
                    SelectedTopic = (Topic)assessment.TopicId,
                    SelectedDifficulty = DifficultyLevel.Hard,
                    Question = q.Question,
                    MusicQuestionFilePath = q.QuestionMusicFilePath,
                    MusicReferenceFilePath = q.ReferenceMusicFilePath,
                    MusicReferenceName = Path.GetFileNameWithoutExtension(q.ReferenceMusicFilePath),
                    OptionsForQuiz = options,
                    CorrectAnswer = q.CorrectAnswer
                };
            }).ToList();

            HttpContext.Session.SetString("QuizQuestions", JsonSerializer.Serialize(questionViewModels));
            HttpContext.Session.SetInt32("CurrentQuestionIndex", 0);

            return RedirectToAction("ShowQuestion");
        }

        /// <summary>
        /// Fetch the current user's academic year to fetch assessment data.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task<string> GetCurrentUserAcademicYearAsync()
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
            {
                throw new InvalidOperationException("User is not logged in.");
            }

            // Query the database for the current user's academic year.
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);

            return user == null ? throw new InvalidOperationException("User not found in the database.") : user.AcademicYear;
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
            var assessmentID = HttpContext.Session.GetInt32("AssessmentID") ?? 0;

            if (questions == null || questions.Count == 0)
            {
                return View("NoQuestions");
            }

            var model = questions[currentIndex];
            ViewBag.CurrentQuestionIndex = currentIndex;
            ViewBag.TotalQuestions = questions.Count;
            ViewBag.AssessmentID = assessmentID;

            return View(model);
        }

        /// <summary>
        /// Show the next question. Finish & Save and return home
        /// </summary>
        /// <param name="selectedOption"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult NextQuestion(string selectedOption)
        {
            var questionsJson = HttpContext.Session.GetString("QuizQuestions");
            var questions = questionsJson != null ? JsonSerializer.Deserialize<List<QuestionViewModel>>(questionsJson) : [];
            var currentIndex = HttpContext.Session.GetInt32("CurrentQuestionIndex") ?? 0;
            var assessmentID = HttpContext.Session.GetInt32("AssessmentID") ?? 0;

            if (questions == null || questions.Count == 0)
            {
                return View("NoQuestions");
            }

            SaveUserAnswer(questions, currentIndex, selectedOption);

            currentIndex++;
            if (currentIndex >= questions.Count)
            {
                return RedirectToAction("QuizFinished", new { assessmentID });
            }

            HttpContext.Session.SetInt32("CurrentQuestionIndex", currentIndex);

            return RedirectToAction("ShowQuestion");
        }

        /// <summary>
        /// Finish the assessment
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QuizFinished()
        {
            var assessmentID = HttpContext.Session.GetInt32("AssessmentID") ?? 0;

            var assessment = await assessmentService.GetAssessmentByIdAsync(assessmentID);

            if (assessment == null)
            {
                TempData["ErrorMessage"] = "Assessment not found.";
                return RedirectToAction("Index");
            }

            var questionsJson = HttpContext.Session.GetString("QuizQuestions");
            var questions = questionsJson != null ? JsonSerializer.Deserialize<List<QuestionViewModel>>(questionsJson) ?? [] : [];

            var userID = await GetUserIdAsync();

            decimal percentage = 0;

            if (questions.Count != 0)
            {
                percentage = CalculatePercentage(questions);
            }

            resultsService.SaveAssessmentResults(percentage, DateTime.Now, (int)DifficultyLevel.Hard, assessment.TopicId, userID, assessmentID);

            return View("QuizFinished");
        }

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
        /// Save user answer to the HTTPcontext
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="currentIndex"></param>
        /// <param name="selectedOption"></param>
        private void SaveUserAnswer(List<QuestionViewModel> questions, int currentIndex, string selectedOption)
        {
            // Set selectedOption to "Not answered" if the user doesn't choose a radio button
            if (string.IsNullOrEmpty(selectedOption))
            {
                selectedOption = "Not answered";
            }

            var currentQuestion = questions[currentIndex];
            var previousAnswer = currentQuestion.UserAnswer;
            var correctAnswers = HttpContext.Session.GetInt32("CorrectAnswers") ?? 0;

            if (currentQuestion.IsAnswered)
            {
                if (previousAnswer == currentQuestion.CorrectAnswer && selectedOption != currentQuestion.CorrectAnswer)
                {
                    correctAnswers--;
                }
                else if (previousAnswer != currentQuestion.CorrectAnswer && selectedOption == currentQuestion.CorrectAnswer)
                {
                    correctAnswers++;
                }
            }
            else
            {
                if (selectedOption == currentQuestion.CorrectAnswer)
                {
                    correctAnswers++;
                }

                currentQuestion.IsAnswered = true;
            }

            currentQuestion.UserAnswer = selectedOption;

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
            _ = HttpContext.Session.GetInt32("AssessmentID") ?? 0;

            if (questions == null || questions.Count == 0)
            {
                return View("NoQuestions");
            }

            currentIndex--;
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            HttpContext.Session.SetInt32("CurrentQuestionIndex", currentIndex);

            return RedirectToAction("ShowQuestion");
        }

        /// <summary>
        /// Calculate the percentage of users assessment
        /// </summary>
        /// <param name="questions"></param>
        /// <returns></returns>
        private static decimal CalculatePercentage(List<QuestionViewModel> questions)
        {
            decimal correctAnswers = 0;
            foreach (var question in questions)
            {
                if (question.UserAnswer == question.CorrectAnswer)
                {
                    correctAnswers++;
                }
            }

            decimal percentage = (correctAnswers / questions.Count) * 100;

            return percentage;
        }
    }
}