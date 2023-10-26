using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SignalBooster.Models;

namespace SignalBooster.ViewModels;

public sealed class BoundActionViewModel : ViewModelBase {
    KeybindAction _Action;
    /// <summary> Gets or sets the action to invoke. </summary>
    public KeybindAction Action {
        get => _Action;
        [MemberNotNull(nameof(_Action))]
        set => SetField(ref _Action, value);
    }

    /// <summary> Gets the keys that must be pressed simultaneously to invoke the action. </summary>
    public ObservableCollection<KeyViewModel> Keys { get; } = new();
    
    public ICommand AddKeyCommand { get; }

    public BoundActionViewModel() => AddKeyCommand = new RelayCommand(AddKey);
    public BoundActionViewModel( BoundAction Action ) : this(Action.Action, Action.Keys) { }
    public BoundActionViewModel( KeybindAction Action, params Key[] Keys ) : this(Action, (IEnumerable<Key>)Keys) { }
    public BoundActionViewModel( KeybindAction Action, IEnumerable<Key> Keys ) : this() {
        this.Action = Action;
        foreach (Key Key in Keys) {
            AddKey(Key);
        }
    }

    sealed class DelayedCommand : ICommand {
        public Action Invoke = () => throw new NotImplementedException();

        #region Implementation of ICommand

        /// <inheritdoc />
        public bool CanExecute( object? Parameter ) => true;
        
        /// <inheritdoc />
        public void Execute( object? Parameter ) => Invoke();
        
        #pragma warning disable CS0067 // Event is never used
        /// <inheritdoc />
        public event EventHandler? CanExecuteChanged;
        #pragma warning restore CS0067 // Event is never used

        #endregion

    }
    
    void AddKey() => AddKey(Key.Subtract);
    void AddKey( Key Key ) {
        DelayedCommand DC   = new();
        KeyViewModel   View = new(Key, DC);
        Keys.Add(View);
        DC.Invoke = Remove;
        void Remove() => Keys.Remove(View);
    }
    
    public static explicit operator BoundAction( BoundActionViewModel VM ) => new(VM.Action, VM.Keys.Select(KVM => KVM.SelectedKey).ToArray());

}
