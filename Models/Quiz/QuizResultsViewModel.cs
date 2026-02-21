namespace MusicQuiz.Web.Models.Quiz
{
    public class QuizResultsViewModel : BaseViewModel
    {
        /// <summary>
        /// Total questions
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// Correct answers
        /// </summary>
        public int CorrectAnswers { get; set; }

        /// <summary>
        /// Users score
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// List of questions
        /// </summary>
        public List<QuestionViewModel>? Questions { get; set; }

        /// <summary>
        /// Date of submission
        /// </summary>
        public DateTime DateOfSubmission { get; set; }

        /// <summary>
        /// AssessmentId
        /// </summary>
        public int? AssessmentID { get; set; }
    }
}
