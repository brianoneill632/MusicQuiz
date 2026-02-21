using MusicQuiz.Core.Enums;
using MusicQuiz.Web.Models.Quiz;

namespace MusicQuiz.Web.Models.Admin
{
    public class QuestionSearchViewModel
    {
        /// <summary>
        /// List of Topics
        /// </summary>
        public List<string>? Topics { get; set; }

        /// <summary>
        /// List of difficulties
        /// </summary>
        public List<string>? Difficulties { get; set; }

        /// <summary>
        /// Selected topic
        /// </summary>
        public Topic? SelectedTopic { get; set; }

        /// <summary>
        /// Selected difficulty
        /// </summary>
        public DifficultyLevel? SelectedDifficulty { get; set; }

        /// <summary>
        /// The list of questions
        /// </summary>
        public List<QuestionViewModel>? Questions { get; set; }

        /// <summary>
        /// The page number
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The number of questions poer page
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// Total questions
        /// </summary>
        public int TotalQuestions { get; set; }
    }
}
