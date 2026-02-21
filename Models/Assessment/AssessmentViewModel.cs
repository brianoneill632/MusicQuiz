using MusicQuiz.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MusicQuiz.Web.Models.Assessment
{
    public class AssessmentViewModel
    {
        /// <summary>
        /// Assessment ID from the DB
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Academic year
        /// </summary>
        [DisplayName("Academic Year")]
        public required string AcademicYear { get; set; }

        /// <summary>
        /// When the assessment will unlock
        /// </summary>
        [DisplayName("Available From")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime OpenFrom { get; set; }

        /// <summary>
        /// When the assessment is available to
        /// </summary>
        [DisplayName("Available Until")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime OpenTo { get; set; }

        /// <summary>
        /// The topic of the assessment
        /// </summary>
        [DisplayName("Topic")]
        public Topic TopicName { get; set; }

        /// <summary>
        /// Boolean for when the assessment is available
        /// </summary>
        public bool IsUnlocked { get; set; }

        /// <summary>
        /// Boolean to check if the user has already completed the assessment
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Is user verified. User needs verified for assessments
        /// </summary>
        public bool IsUserVerified { get; set; }

        /// <summary>
        /// Gets the status text for the assessment
        /// </summary>
        [DisplayName("Status")]
        public string StatusText => GetStatusText();

        /// <summary>
        /// Gets the CSS class for the status display
        /// </summary>
        public string StatusCssClass => GetStatusCssClass();

        /// <summary>
        /// Gets the icon class for the status
        /// </summary>
        public string StatusIconClass => GetStatusIconClass();

        /// <summary>
        /// Gets whether the Start button should be displayed
        /// </summary>
        public bool ShowStartButton => IsUserVerified && IsUnlocked && !IsCompleted;

        /// <summary>
        /// Check if the assessment is active based on current date
        /// </summary>
        public bool IsActive => DateTime.Now >= OpenFrom && DateTime.Now <= OpenTo;

        /// <summary>
        /// Get the appropriate status text based on the assessment state
        /// </summary>
        private string GetStatusText()
        {
            if (!IsUserVerified) return "Email verification required";
            if (IsCompleted) return "Completed";
            if (!IsActive) return "Not currently available";
            if (IsUnlocked) return "Available";
            return "Locked";
        }

        /// <summary>
        /// Get the appropriate CSS class for styling based on the assessment state
        /// </summary>
        private string GetStatusCssClass()
        {
            if (!IsUserVerified) return "alert-warning";
            if (IsCompleted) return "alert-success";
            if (IsUnlocked) return "alert-primary";
            return "alert-secondary";
        }

        /// <summary>
        /// Get the appropriate icon class based on the assessment state
        /// </summary>
        private string GetStatusIconClass()
        {
            if (!IsUserVerified) return "fa-exclamation-circle";
            if (IsCompleted) return "fa-check-circle";
            if (IsUnlocked) return "fa-play-circle";
            return "fa-lock";
        }
    }
}
