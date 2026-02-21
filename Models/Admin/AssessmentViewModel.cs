using MusicQuiz.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MusicQuiz.Web.Models.Admin
{
    public class AssessmentViewModel
    {
        /// <summary>
        /// academic year for the assessment
        /// </summary>
        [Required]
        public string? AcademicYear { get; set; }

        /// <summary>
        /// Open from date
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime OpenFrom { get; set; }

        /// <summary>
        /// Open to date
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime OpenTo { get; set; }

        /// <summary>
        /// Assessment Topic
        /// </summary>
        [Required]
        public Topic SelectedTopic { get; set; }

        /// <summary>
        /// List of Academic years for the admin to select
        /// </summary>
        public List<string> AcademicYearOptions { get; set; } = [];

        /// <summary>
        /// Assessment ID FOR EDIT PAGE
        /// </summary>
        public int? ID { get; set; }
    }
}
