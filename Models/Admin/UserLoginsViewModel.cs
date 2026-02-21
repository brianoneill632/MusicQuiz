namespace MusicQuiz.Web.Models.Admin
{
    public class UserLoginsViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// StudentID
        /// </summary>
        public string? StudentID { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Last logged in date
        /// </summary>
        public DateTime LastLoginDate { get; set; }

        /// <summary>
        /// Weekly login counts
        /// </summary>
        public int[] WeeklyLoginCounts { get; set; } = new int[5];
    }
}