using Microsoft.AspNetCore.Mvc;
using MusicQuiz.Web.Models;
using System.Diagnostics;
using MusicQuiz.Core.Entities;
using Microsoft.AspNetCore.Identity;
using MusicQuiz.Application.Interfaces;
using MusicQuiz.Web.Models.Home;

namespace MusicQuiz.Web.Controllers
{
    public class PrivacyController() : BaseController
    {
        /// <summary>
        /// Privacy page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}