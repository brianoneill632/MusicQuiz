using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicQuiz.Web.Models
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notify the UI that a property has changed
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Set a property and notify the UI that it has changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStore"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null!)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// The user's ID
        /// </summary>
        private string userId = string.Empty;
        public string UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        /// <summary>
        /// isLoggedIn backing field
        /// </summary>
        private bool isLoggedIn;

        /// <summary>
        /// Is the user logged in?
        /// </summary>
        public bool IsLoggedIn
        {
            get => isLoggedIn;
            set => SetProperty(ref isLoggedIn, value);
        }
    }
}