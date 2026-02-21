using MusicQuiz.Core.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicQuiz.Web.Models.Quiz
{
    /// <summary>
    /// This is the main model used for the list of questions
    /// </summary>
    public class QuestionViewModel : BaseViewModel
    {
        /// <summary>
        /// ID from database for the question. This is for the edit/delete functions
        /// </summary>
        public int QuestionID { get; set; }

        /// <summary>
        /// Gets or sets the selected topic
        /// </summary>
        [DisplayName("Topic")]
        [Required(ErrorMessage = "Please select a topic")]
        public Topic SelectedTopic { get; set; }

        /// <summary>
        /// Gets or sets the selected difficulty
        /// </summary>
        [DisplayName("Difficulty")]
        [Required(ErrorMessage = "Please select a difficulty")]
        public DifficultyLevel SelectedDifficulty { get; set; }

        /// <summary>
        /// Gets or sets the question
        /// </summary>
        [DisplayName("Question")]
        [Required(ErrorMessage = "Please enter a question")]
        public string Question { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location of the question music file
        /// </summary>
        [DisplayName("Music Path")]
        [Required(ErrorMessage = "Please enter a music path")]
        public string MusicQuestionFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location of the template music file
        /// </summary>
        [DisplayName("Reference Path")]
        [Required(ErrorMessage = "Please enter a reference path")]
        public string MusicReferenceFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location of the template music file
        /// </summary>
        public string MusicReferenceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the selectable answers
        /// </summary>
        [Required]
        public QuizSelectableAnswers Options { get; set; } = new QuizSelectableAnswers();

        /// <summary>
        /// Gets or sets the list of potential selectable options
        /// NOTE THIS IS SET UP FOR THE PURPOSE OF RANDOMISING THE LAOUT OF THE OPTIONS
        /// </summary>
        public List<string> OptionsForQuiz { get; set; } = [];

        /// <summary>
        /// Gets or sets the correct answer
        /// </summary>
        [DisplayName("Correct Answer")]
        public string CorrectAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Users answer
        /// </summary>
        [DisplayName("Users Final Answer")]
        public string UserAnswer { get; set; } = string.Empty;

        /// <summary>
        /// First answer to actually mark against
        /// </summary>
        [DisplayName("Users First Answer")]
        public string FirstAnswer { get; set; } = string.Empty;

        /// <summary>
        /// This is used when going between next & previous to stop the
        /// quesiton being submitted more than once
        /// </summary>
        public bool IsAnswered { get; set; }

        /// <summary>
        /// This variable is to store whether the user has submitted the question or not
        /// It also will display correct/incorrect if they are changing between questions
        /// </summary>
        public string? Feedback { get; set; }

        /// <summary>
        /// If is Assessment, feedback is turned off
        /// </summary>
        public bool IsAssessment { get; set; }

        /// <summary>
        /// Attempt number for the question
        /// </summary>
        [DisplayName("Attempt Number")]
        public int AttemptNumber { get; set; }

        /// <summary>
        /// EXP gained from the question, after number of attempts
        /// </summary>
        public int EXP { get; set; }

        /// <summary>
        /// List of available music file paths for dropdowns
        /// </summary>
        public List<string>? MusicFiles { get; set; }
    }
}
