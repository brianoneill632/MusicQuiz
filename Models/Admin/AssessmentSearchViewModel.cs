namespace MusicQuiz.Web.Models.Admin
{
    public class AssessmentSearchViewModel
    {
        /// <summary>
        /// Academic year selected for search
        /// </summary>
        public string? SelectedYear { get; set; }

        /// <summary>
        /// Topic for search
        /// </summary>
        public int? SelectedTopic { get; set; }

        /// <summary>
        /// List of available academic years
        /// </summary>
        public List<string> AcademicYears { get; set; } = [];

        /// <summary>
        /// List of available topics
        /// </summary>
        public List<string> Topics { get; set; } = [];

        /// <summary>
        /// List of assessments
        /// </summary>
        public List<AssessmentViewModel> Assessments { get; set; } = [];

        /// <summary>
        /// Page number for filtering
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Page size for filtering
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total available assessments
        /// </summary>
        public int TotalAssessments { get; set; }
    }
}