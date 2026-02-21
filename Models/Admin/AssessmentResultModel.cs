using MusicQuiz.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MusicQuiz.Web.Models.Admin
{
    /// <summary>
    /// Model for assessment results in the admin view
    /// </summary>
    public class AssessmentResultModel
    {
        /// <summary>
        /// Primary ID of the result
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User ID from the AspNetUsers table
        /// </summary>
        public string UserID { get; set; } = string.Empty;

        /// <summary>
        /// Student's first name
        /// </summary>
        public string Forename { get; set; } = string.Empty;

        /// <summary>
        /// Student's last name
        /// </summary>
        public string Surname { get; set; } = string.Empty;

        /// <summary>
        /// Student ID number
        /// </summary>
        public string StudentID { get; set; } = string.Empty;

        /// <summary>
        /// Academic year of the student
        /// </summary>
        public string StudentAcademicYear { get; set; } = string.Empty;

        /// <summary>
        /// Score achieved (percentage)
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:0.00}%")]
        public decimal UserScore { get; set; }

        /// <summary>
        /// Date the assessment was submitted
        /// </summary>
        public DateTime DateOfSubmission { get; set; }

        /// <summary>
        /// Selected topic for the assessment
        /// </summary>
        public Topic SelectedTopic { get; set; }

        /// <summary>
        /// Selected difficulty level
        /// </summary>
        public DifficultyLevel SelectedDifficulty { get; set; }

        /// <summary>
        /// ID of the assessment
        /// </summary>
        public int? AssessmentId { get; set; }

        /// <summary>
        /// Academic year of the assessment
        /// </summary>
        public string AssessmentAcademicYear { get; set; } = string.Empty;

        /// <summary>
        /// Student's experience points
        /// </summary>
        public int EXP { get; set; }

        /// <summary>
        /// Student's level based on experience
        /// </summary>
        public int Level { get; set; }
    }
}
