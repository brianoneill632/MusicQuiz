using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MusicQuiz.Web.Models;

namespace MusicQuiz.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Set the IsLoggedIn and UserId values for the model
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Set the IsLoggedIn and UserId values for the model
            if (context.Controller is Controller controller && controller.ViewData.Model is BaseViewModel model)
            {
                var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
                model.IsLoggedIn = isAuthenticated;
                model.UserId = isAuthenticated ? User?.Identity?.Name ?? string.Empty : string.Empty;
            }

            //Set the active nav bar for highlighting
            SetActiveNav(context);
        }

        /// <summary>
        /// Set the active navigation value for the navbar
        /// </summary>
        /// <param name="context"></param>
        private void SetActiveNav(ActionExecutingContext context)
        {
            // Ger controllers
            var controllerName = context.ActionDescriptor.RouteValues["controller"];
            var areaName = context.ActionDescriptor.RouteValues["area"];
            var pageName = context.ActionDescriptor.RouteValues["page"];

            // Map the controller name to a corresponding active navigation value
            string activeNavValue = controllerName switch
            {
                "Quiz" => "Quiz",
                "Home" => "Home",
                "Assessment" => "Assessment",
                "Leaderboards" => "Leaderboards",
                "Privacy" => "Privacy",
                "Admin" => "Admin",
                _ => "Home"
            };

            // Added for the login/register pages
            if (areaName == "Identity")
            {
                activeNavValue = pageName switch
                {
                    "/Account/Register" => "Register",
                    "/Account/Login" => "Login",
                    _ => activeNavValue
                };
            }

            ViewData["ActiveNav"] = activeNavValue;
        }
    }
}
