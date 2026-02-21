using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicQuiz.Core.Entities;
using MusicQuiz.Core.Migrations;
using MusicQuiz.Web.Models.Leaderboards;

namespace MusicQuiz.Web.Controllers
{
    public class LeaderboardsController(ApplicationDbContext context, UserManager<UserData> userManager) : BaseController
    {
        [BindProperty]
        public required LeaderboardViewModel ViewModel { get; set; }

        /// <summary>
        /// Leaderboards
        /// </summary>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string academicYear = null)
        {
            // Adjust the logic to consider the current academic year dynamically for Sept - Aug
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var currentAcademicYear = (currentMonth >= 9 && currentMonth <= 12)
                ? $"{currentYear % 100}/{(currentYear + 1) % 100}"
                : $"{(currentYear - 1) % 100}/{currentYear % 100}";

            academicYear ??= currentAcademicYear;

            // Get the current logged-in user (if any)
            var user = await userManager.GetUserAsync(User);

            // Fetch the top users for the selected academic year
            var topUsers = await GetTopUsersByAcademicYearAsync(academicYear);

            // Fetch all users in the selected academic year (needed for user rank calculation)
            var allUsers = await GetAllUsersByAcademicYearAsync(academicYear);

            // Get the user's rank, if they are in the list of all users
            int userRank = allUsers
                .OrderByDescending(u => u.EXP)
                .Select((userData, index) => new { userData, index })
                .FirstOrDefault(u => u.userData.IntID == user?.IntID)?.index + 1 ?? -1;

            // Prepare ViewModel data to be passed to the view
            ViewModel = new LeaderboardViewModel
            {
                TopUsers = topUsers,
                CurrentUser = user,
                SelectedAcademicYear = academicYear.Equals("All Time") ? academicYear : $"20{academicYear}",
                AcademicYearOptions = GetAcademicYearOptions(),
                UserRank = userRank
            };

            return View(ViewModel);
        }


        /// <summary>
        /// Fetch all users for the selected academic year (for rank calculation)
        /// </summary>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private async Task<List<UserData>> GetAllUsersByAcademicYearAsync(string academicYear)
        {
            if (academicYear == "All Time")
            {
                return await context.Users.ToListAsync();
            }
            else
            {
                return await context.Users
                    .Where(u => u.AcademicYear == academicYear)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Leaderboards (for the filter buttons)
        /// </summary>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> IndexPost(string academicYear)
        {
            return await Index(academicYear);
        }

        /// <summary>
        /// Fetch top users for the selected academic year
        /// </summary>
        /// <param name="academicYear"></param>
        /// <returns></returns>
        private async Task<List<UserData>> GetTopUsersByAcademicYearAsync(string academicYear)
        {
            if (academicYear == "All Time")
            {
                return await context.Users
                    //Leave out 'NOTLOGGED IN' & 'ADMIN'
                    .Where(u => u.IntID != 0 && u.IntID != 1)
                    .OrderByDescending(u => u.EXP )
                    .Take(10)
                    .ToListAsync();
            }
            else
            {
                return await context.Users
                    .Where(u => u.AcademicYear == academicYear && (u.IntID != 0 && u.IntID != 1))
                    .OrderByDescending(u => u.EXP)
                    .Take(10)
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Getting list of academic years, this year, the previous year and next
        /// This is more of a just-in-case rather than necesscary
        /// The users of the app will tpically be for the modue in that year.
        /// They will need to change this in the user section if they use this application
        /// for more than an academic year as this will be used for leaderboards and assessments
        /// </summary>
        /// <returns></returns>
        public List<string> GetAcademicYearOptions()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            // Adjust the logic to consider the current academic year as 24/25 for Sept - Aug
            var currentAcademicYear = (currentMonth >= 9 && currentMonth <= 12)
                ? currentYear
                : (currentMonth >= 1 && currentMonth <= 8) ? currentYear - 1 : currentYear;

            // Generate the correct academic year options
            var options = new List<string>
            {
                // Current academic year
                $"{currentAcademicYear % 100}/{(currentAcademicYear + 1) % 100}",

                //Overall leaderboard
                "All Time"
            };

            return options;
        }
    }
}
