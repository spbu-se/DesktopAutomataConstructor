using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AutomataConstructor.ViewModels.Base
{
    /// <summary>
    /// Base class for all view models
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes property changed
        /// </summary>
        /// <param name="PropertyName">Name of the property</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        /// <summary>
        /// Sets value to the field where property storages data
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="field">Storage of data</param>
        /// <param name="value">A new value to the property</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <returns></returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
    }
}
