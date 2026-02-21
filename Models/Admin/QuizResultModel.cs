using MusicQuiz.Core.Enums;

namespace MusicQuiz.Web.Models.Admin
{
    public class QuizResultModel : BaseViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// User score
        /// </summary>
        public decimal UserScore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateOfSubmission { get; set; }

        /// <summary>
        /// Selected Topic
        /// </summary>
        public Topic SelectedTopic { get; set; }

        /// <summary>
        /// Selected Difficulty
        /// </summary>
        public DifficultyLevel SelectedDifficulty { get; set; }

        /// <summary>
        /// Assessment ID
        /// </summary>
        public int? AssessmentId { get; set; }

        /// <summary>
        /// Forename
        /// </summary>
        public string? Forename { get; set; }

        /// <summary>
        /// Surname
        /// </summary>
        public string? Surname { get; set; }
    }
}
