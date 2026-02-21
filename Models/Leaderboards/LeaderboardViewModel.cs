using MusicQuiz.Core.Entities;

namespace MusicQuiz.Web.Models.Leaderboards
{
    public class LeaderboardViewModel : BaseViewModel
    {
        /// <summary>
        /// Top 25 users
        /// </summary>
        public required List<UserData> TopUsers { get; set; }

        /// <summary>
        /// Current users details to be highlighted
        /// </summary>
        public UserData? CurrentUser { get; set; }

        /// <summary>
        /// Academic year to filter by
        /// </summary>
        public required string SelectedAcademicYear { get; set; }

        /// <summary>
        /// Selectable options for academic year
        /// </summary>
        public required List<string> AcademicYearOptions { get; set; }

        /// <summary>
        /// Get the users rank, to show if not in top 25 and is logged in
        /// </summary>
        public int? UserRank { get; set; }
    }
}
