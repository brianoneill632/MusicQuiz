using MusicQuiz.Core.Enums;

namespace MusicQuiz.Web.Models.Home
{
    public class DifficultyModel : BaseViewModel
    {
        /// <summary>
        /// Difficulty Level
        /// </summary>
        public DifficultyLevel DifficultyLevel { get; set; }

        /// <summary>
        /// Difficulty description
        /// </summary>
        public string? Description { get; set; }
    }
}
