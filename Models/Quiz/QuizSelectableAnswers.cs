using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MusicQuiz.Web.Models.Quiz
{
    public class QuizSelectableAnswers : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the correct answer
        /// </summary>
        [DisplayName("Correct Answer")]
        [Required(ErrorMessage = "Please enter the correct answer")]
        public string CorrectAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first wrong answer
        /// </summary>
        [DisplayName("Wrong Answer One")]
        [Required(ErrorMessage = "Please enter the first wrong answer")]
        public string WrongAnswerOne { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second wrong answer
        /// </summary>
        [DisplayName("Wrong Answer Two")]
        [Required(ErrorMessage = "Please enter the second wrong answer")]
        public string WrongAnswerTwo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the third wrong answer
        /// </summary>
        [DisplayName("Wrong Answer Three")]
        [Required(ErrorMessage = "Please enter the third wrong answer")]
        public string WrongAnswerThree { get; set; } = string.Empty;
    }
}
