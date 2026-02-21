using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MusicQuiz.Web.Models.Admin
{
    public class ManageUserRolesViewModel
    {
        /// <summary>
        /// User ID
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public required string LastName { get; set; }

        /// <summary>
        /// Stident id
        /// </summary>
        public required string StudentID { get; set; }

        /// <summary>
        /// List of roles
        /// </summary>
        public required List<IdentityRole> Roles { get; set; }

        /// <summary>
        /// User roles
        /// </summary>
        public required IList<string> UserRoles { get; set; }

        /// <summary>
        /// Selected Role
        /// </summary>
        public required string SelectedRole { get; set; }
    }
}
