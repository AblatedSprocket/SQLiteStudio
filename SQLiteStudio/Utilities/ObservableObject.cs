using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SQLiteStudio.Utilities
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected virtual void OnPropertyChanged<T>(ref T property, T value, [CallerMemberName] string propertyName = "")
        {
            property = value;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
