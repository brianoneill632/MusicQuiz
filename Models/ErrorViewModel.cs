namespace MusicQuiz.Web.Models
{
    public class ErrorViewModel : BaseViewModel
    {
        /// <summary>
        /// The request ID
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Show the request ID
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}