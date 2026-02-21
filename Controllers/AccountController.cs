using Microsoft.AspNetCore.Mvc;
using MusicQuiz.Application.Services;

namespace MusicQuiz.Web.Controllers
{
    public class AccountController(UserRoleService userRoleService) : BaseController
    {
        /// <summary>
        /// Assign a role to a user if they aren't assigned to it already
        /// </summary>
        /// <param name="userEmail"></param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public async Task<IActionResult> AssignRole(string userEmail, string roleName)
        {
            await userRoleService.AssignRoleToUserAsync(userEmail, roleName);
            return RedirectToAction("Index");
        }
    }
}