using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SignalBooster.ViewModels;

public class PropertyNotifier : DependencyObject, INotifyPropertyChanged {

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary> Raise the <see cref="PropertyChanged"/> event for the given property name. </summary>
    /// <param name="PropertyName"> The name of the property that changed. </param>
    protected virtual void OnPropertyChanged( [CallerMemberName] string? PropertyName = null ) => PropertyChanged?.Invoke(this, new(PropertyName));

    /// <summary> Set the given field to the given value, and raise the <see cref="PropertyChanged"/> event for the given property name. </summary>
    /// <typeparam name="T"> The type of the field. </typeparam>
    /// <param name="Field"> The field to set. </param>
    /// <param name="Value"> The value to set the field to. </param>
    /// <param name="PropertyName"> The name of the property that changed. </param>
    /// <returns> <see langword="true"/> if the field was changed, <see langword="false"/> otherwise. </returns>
    protected bool SetField<T>( ref T Field, T Value, [CallerMemberName] string? PropertyName = null ) {
        if (EqualityComparer<T>.Default.Equals(Field, Value)) { return false; }

        Field = Value;
        OnPropertyChanged(PropertyName);
        return true;
    }
}
