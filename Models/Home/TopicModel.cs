using MusicQuiz.Core.Enums;

namespace MusicQuiz.Web.Models.Home
{
    public class TopicModel : BaseViewModel
    {
        /// <summary>
        /// The topic of the quiz.
        /// </summary>
        public Topic Topic { get; set; }

        /// <summary>
        /// The description of the topic.
        /// </summary>
        public string? Description { get; set; }
    }
}
