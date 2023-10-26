using System.Windows.Input;

namespace SignalBooster.ViewModels;

public sealed class KeyViewModel : ViewModelBase {
    Key _SelectedKey;

    /// <summary> Gets or sets the key. </summary>
    public Key SelectedKey {
        get => _SelectedKey;
        set => SetField(ref _SelectedKey, value);
    }
    
    public ICommand RemoveKeyCommand { get; }

    public KeyViewModel(ICommand Remove) : this(Key.Subtract, Remove) { }
    public KeyViewModel( Key SelectedKey, ICommand Remove ) {
        _SelectedKey     = SelectedKey;
        RemoveKeyCommand = Remove;
    }
}

