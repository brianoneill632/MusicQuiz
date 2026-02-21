using Microsoft.AspNetCore.Mvc;
using MusicQuiz.Web.Models;
using System.Diagnostics;
using MusicQuiz.Core.Entities;
using Microsoft.AspNetCore.Identity;
using MusicQuiz.Application.Interfaces;
using MusicQuiz.Web.Models.Home;

namespace MusicQuiz.Web.Controllers
{
    public class HomeController(UserManager<UserData> userManager, IResultsService resultsService) : BaseController
    {
        /// <summary>
        /// View homepage
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            ClearQuizSession();

            var user = await userManager.GetUserAsync(User);
            UsersPracticeQuizResults? latestQuizResult = null;

            if (user != null)
            {
                latestQuizResult = await resultsService.GetMostRecentQuizResultAsync(user.Id);
            }

            var model = new MusicQuizViewModel
            {
                LatestAttemptDate = latestQuizResult?.DateOfSubmission,
                LatestUserScore = latestQuizResult?.UserScore
            };

            return View(model);
        }

        /// <summary>
        /// View the privacy page
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
    }
}