namespace MusicQuiz.Web.Models.Admin
{
    public class UserListViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public required string Email { get; set; }
    }
}
