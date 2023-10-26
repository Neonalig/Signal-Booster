using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SignalBooster.Converters;
using SignalBooster.Models;
using SignalBooster.Views.Controls;

namespace SignalBooster.ViewModels; 

public sealed class SettingsViewModel : ViewModelBase {
    public ObservableCollection<BoundActionView> BoundActions { get; } = new();

    static readonly FileInfo _Dest = new(Path.Combine(Environment.CurrentDirectory, "keybinds.json"));
    
    public ICommand RemoveBoundActionCommand { get; }
    public ICommand AddBoundActionCommand    { get; }

    public ICommand CancelCommand       { get; }
    public ICommand SaveCommand         { get; }
    public ICommand LoadDefaultsCommand { get; }

    public SettingsViewModel() {
        RemoveBoundActionCommand = new RelayCommand<BoundActionView>(RemoveBoundAction);
        AddBoundActionCommand    = new RelayCommand(AddBoundAction);
        
        CancelCommand       = new RelayCommand(Cancel);
        SaveCommand         = new RelayCommand(Save);
        LoadDefaultsCommand = new RelayCommand(LoadDefaultKeybinds);
        
        InitAsync();
    }
    
    void RemoveBoundAction( BoundActionView? View ) {
        if (View is null) { return; }
        BoundActions.Remove(View);
    }
    
    void AddBoundAction() {
        BoundActionView View = new() {
            DataContext = new BoundActionViewModel()
        };
        BoundActions.Add(View);
    }

    static void Cancel() {
        Window? Window = Application.Current.MainWindow;
        if (Window is not MainWindow MW) {
            Trace.TraceError("Window is not MainWindow");
            return;
        }

        MW.Back(Release: true);
    }
    
    static readonly JsonSerializerOptions _Options = new() {
        Converters = {
            new JsonStringEnumMemberConverter<Key>(),
            new JsonStringEnumMemberConverter<KeybindAction>()
        },
        WriteIndented = true
    };
    
    async void Save() {
        List<BoundAction> Actions = new();
        foreach (BoundActionView View in BoundActions) {
            if (View.DataContext is not BoundActionViewModel VM) { continue; }
            BoundAction Action = (BoundAction)VM;
            Actions.Add(Action);
        }
        KeybindsFile Keybinds = new(Actions);

        await using (FileStream Stream = _Dest.OpenWrite()) {
            await JsonSerializer.SerializeAsync(Stream, Keybinds, _Options);
        }
        
        MessageBox.Show("Keybinds have been saved.\nThe application will now restart for changes to take effect.", "Keybinds Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        Application.Current.Shutdown();
    }
    
    void AppendKeybindsFile( KeybindsFile Keybinds ) {
        foreach (BoundAction Action in Keybinds.BoundActions) {
            BoundActionView View = new() {
                DataContext = new BoundActionViewModel(Action)
            };
            BoundActions.Add(View);
        }
    }
    void LoadKeybindsFile( KeybindsFile Keybinds ) {
        Clear();
        AppendKeybindsFile(Keybinds);
    }
    
    void Clear() => BoundActions.Clear();

    void LoadDefaultKeybinds() => LoadKeybindsFile(KeybindsFile.Default);
    
    /// <summary> Gets the current keybinds file, creating it if it doesn't exist. </summary>
    /// <returns> The keybinds file. </returns>
    public static async Task<KeybindsFile> GetOrCreateKeybindsFile() {
        if (_Dest.Exists) {
            await using FileStream Stream = _Dest.OpenRead();
            try {
                return await JsonSerializer.DeserializeAsync<KeybindsFile>(Stream, _Options) ?? KeybindsFile.Default;
            } catch (JsonException Ex) {
                Console.WriteLine($"Failed to deserialize keybinds file: {Ex}");
                return KeybindsFile.Default;
            }
        }
        return KeybindsFile.Default;
    }
    
    async void InitAsync() {
        KeybindsFile Keybinds = await GetOrCreateKeybindsFile();
        LoadKeybindsFile(Keybinds);
    }
}