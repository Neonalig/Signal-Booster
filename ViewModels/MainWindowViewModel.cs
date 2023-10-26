using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SignalBooster.Views.Controls;

namespace SignalBooster.ViewModels;

public class MainWindowViewModel : ViewModelBase {
    public ICommand OpenSettingsCommand { get; }

    readonly Dictionary<Type, object> _OpenViews   = new();
    readonly Stack<Type>              _ViewHistory = new();
    void OpenView( Type Type, Func<object> Activator ) {
        if (!_OpenViews.TryGetValue(Type, out object? View)) {
            View = Activator();
            _OpenViews.Add(Type, View);
        }
        CurrentView = View;
    }
    void OpenView<TView>( Func<TView>? Activator = null ) where TView : class => OpenView(typeof(TView), Activator ?? System.Activator.CreateInstance<TView>);

    object? _CurrentView;
    public object CurrentView {
        get => _CurrentView ?? throw new NullReferenceException();
        private set {
            object? LastView = _CurrentView;
            if (LastView != null && LastView != value) {
                _ViewHistory.Push(LastView.GetType());
            }

            if (SetField(ref _CurrentView, value)) {
                Console.WriteLine($"Current view is now {value.GetType()} (history: {string.Join(", ", _ViewHistory)})");
                OnPropertyChanged(nameof(CanOpenSettings));
            } else {
                Console.WriteLine($"Current view is already {value.GetType()} (history: {string.Join(", ", _ViewHistory)})");
            }
        }
    }
    
    /// <summary> Whether the current view is not the settings view. </summary>
    public bool CanOpenSettings => _CurrentView is not SettingsView;

    public MainWindowViewModel() {
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        CurrentView         = new RollingTimerView();
        _OpenViews.Add(typeof(RollingTimerView), CurrentView);
    }

    void OpenSettings() => OpenView<SettingsView>();

    /// <summary> Pops the last view from the history and sets it as the current view. </summary>
    /// <param name="Release"> Whether to release the view from memory. </param>
    /// <exception cref="KeyNotFoundException"> Thrown when the view type is not found in _OpenViews. </exception>
    public void Back( bool Release = false ) {
        if (_CurrentView == null) { return; }

        if (Release) {
            _OpenViews.Remove(_CurrentView.GetType());
        }

        if (!_ViewHistory.TryPop(out Type? ViewType)) {
            Console.WriteLine("No view history");
            return;
        }

        if (_OpenViews.TryGetValue(ViewType, out object? View)) {
            Console.WriteLine($"Setting view to {ViewType}");
            CurrentView = View;
        } else {
            throw new KeyNotFoundException($"View of type {ViewType} not found in _OpenViews");
        }
    }
}
