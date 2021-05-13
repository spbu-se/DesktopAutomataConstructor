using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ControlsLibrary.ViewModel.Base
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes property changed
        /// </summary>
        /// <param name="propertyName">Name of the property</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets value to the field where property storages data
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Storage of data</param>
        /// <param name="value">A new value to the property</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns></returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}