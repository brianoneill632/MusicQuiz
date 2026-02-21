using MusicQuiz.Core.Enums;

namespace MusicQuiz.Web.Models.Admin
{
    public class DeleteAssessmentViewModel
    {
        /// <summary>
        /// Assessment to be deleted
        /// </summary>
        public int AssessmentId { get; set; }

        /// <summary>
        /// Academic year to be deleted
        /// </summary>
        public string AcademicYear { get; set; } = "Unknown";

        /// <summary>
        /// Topic to be deleted
        /// </summary>
        public Topic Topic { get; set; }

        /// <summary>
        /// Open from
        /// </summary>
        public DateTime OpenFrom { get; set; }

        /// <summary>
        /// Open to
        /// </summary>
        public DateTime OpenTo { get; set; }

        /// <summary>
        /// How many people have currently taken the assessment to be deleted
        /// </summary>
        public int TakenCount { get; set; }

        /// <summary>
        /// Delete yes or no
        /// </summary>
        public bool DeleteAssociatedResults { get; set; } = false;
    }
}