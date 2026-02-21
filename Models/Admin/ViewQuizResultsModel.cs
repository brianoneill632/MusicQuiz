using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicQuiz.Web.Models.Admin
{
    public class ViewQuizResultsModel : PageModel
    {
        /// <summary>
        /// Filter for the topic
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Topic { get; set; }

        /// <summary>
        /// Filter for the difficulty
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Difficulty { get; set; }

        /// <summary>
        /// Filter for the month
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string Month { get; set; }

        /// <summary>
        /// List of quiz results for logged-in users
        /// </summary>
        public List<QuizResultModel> LoggedInResults { get; set; }

        /// <summary>
        /// List of quiz results for not logged-in users
        /// </summary>
        public List<QuizResultModel> NotLoggedInResults { get; set; }

        /// <summary>
        /// Data for the chart
        /// </summary>
        public List<decimal> ScoreDataLoggedIn { get; set; }

        /// <summary>
        /// List of user names for logged-in users
        /// </summary>
        public List<string> UserNamesLoggedIn { get; set; }

        /// <summary>
        /// Data for the chart
        /// </summary>
        public List<decimal> ScoreDataNotLoggedIn { get; set; }

        /// <summary>
        /// List of user names for not logged-in users
        /// </summary>
        public List<string> UserNamesNotLoggedIn { get; set; }

        /// <summary>
        /// Handles the GET request for the page
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGet()
        {
            // Fetch data from the database
            var allLoggedInResults = GetLoggedInResultsFromDatabase();
            var allNotLoggedInResults = GetNotLoggedInResultsFromDatabase();

            // Filter data based on the selected filters
            LoggedInResults = FilterResults(allLoggedInResults, Topic, Difficulty, Month);
            NotLoggedInResults = FilterResults(allNotLoggedInResults, Topic, Difficulty, Month);

            // Prepare data for the chart
            ScoreDataLoggedIn = [.. LoggedInResults.Select(r => r.UserScore)];
            UserNamesLoggedIn = [.. LoggedInResults.Select(r => $"{r.Forename} {r.Surname}")];
            ScoreDataNotLoggedIn = [.. NotLoggedInResults.Select(r => r.UserScore)];
            UserNamesNotLoggedIn = [.. NotLoggedInResults.Select(r => $"{r.Forename} {r.Surname}")];

            return Page();
        }

        /// <summary>
        /// Fetches quiz results for logged-in users from the database
        /// </summary>
        /// <returns></returns>
        private static List<QuizResultModel> GetLoggedInResultsFromDatabase()
        {
            return [];
        }

        /// <summary>
        /// Fetches quiz results for not logged-in users from the database
        /// </summary>
        /// <returns></returns>
        private static List<QuizResultModel> GetNotLoggedInResultsFromDatabase()
        {
            return [];
        }

        /// <summary>
        /// Filters the quiz results based on the selected topic, difficulty, and month
        /// </summary>
        /// <param name="results"></param>
        /// <param name="topic"></param>
        /// <param name="difficulty"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private static List<QuizResultModel> FilterResults(List<QuizResultModel> results, string topic, string difficulty, string month)
        {
            if (topic != "all")
            {
                results = [.. results.Where(r => r.SelectedTopic.ToString() == topic)];
            }

            if (difficulty != "all")
            {
                results = [.. results.Where(r => r.SelectedDifficulty.ToString() == difficulty)];
            }

            if (month != "all")
            {
                var selectedMonth = DateTime.ParseExact(month, "MM-yyyy", null);
                results = [.. results.Where(r => r.DateOfSubmission.Month == selectedMonth.Month && r.DateOfSubmission.Year == selectedMonth.Year)];
            }

            return results;
        }
    }
}
