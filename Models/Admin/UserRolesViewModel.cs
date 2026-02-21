namespace MusicQuiz.Web.Models.Admin
{
    public class UserRolesViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// List of roles
        /// </summary>
        public IList<string>? Roles { get; set; }
    }
}
