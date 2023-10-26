using System;
using System.Collections.Generic;
using System.ComponentModel;
using SignalBooster.Input;
using SignalBooster.Models;
using SignalBooster.ViewModels;

namespace SignalBooster.Views.Controls; 

public partial class RollingTimerView : IDisposable {
    readonly HashSet<Keybind> _Keybinds = new();
    
    public RollingTimerView() {
        InitializeComponent();
        Unloaded += ( _, _ ) => Dispose();

        InitAsync();
        
        // // Read the keybinds.txt file and set up the keybinds
        // // Defaults: KeyPad Minus    - Reset / Start - 0x6D
        // //           KeyPad Multiply - Clear / Stop  - 0x6A
        //
        // FileInfo KeybindsFile = new("keybinds.txt");
        // if (!KeybindsFile.Exists) {
        //     Console.WriteLine("No keybinds file found");
        //     return;
        // }
        //
        // string[] Lines = File.ReadAllLines(KeybindsFile.FullName);
        // foreach (string Line in Lines) {
        //     Console.WriteLine($"Parsing keybind line: {Line}");
        //     // Format:
        //     // <Name> <Key(s)>
        //     // i.e. "reset 0x6D" or "reset Ctrl Space"
        //     string[] Parts = Line.Split(' ');
        //     int      Ln    = Parts.Length;
        //     if (Ln < 2) {
        //         Console.WriteLine($"Invalid keybind line: {Line}");
        //         continue;
        //     }
        //     
        //     string Name    = Parts[0];
        //     Key[]  Keys    = new Key[Ln - 1];
        //     bool   Success = true;
        //     for (int I = 1; I < Ln; I++) {
        //         if (!TryConvertToKeyCode(Parts[I], out Keys[I - 1])) {
        //             Console.WriteLine($"Invalid keybind line: {Line}");
        //             Success = false;
        //             break;
        //         }
        //     }
        //     if (!Success) { continue; }
        //
        //     Action? Callback = Name.ToLower() switch {
        //         "start_or_reset" => ViewModel.StartOrReset,
        //         "stop_or_clear"  => ViewModel.StopOrClear,
        //         "rollover"       => ViewModel.Rollover,
        //         "cancel"         => ViewModel.Cancel,
        //         "start"          => ViewModel.Start,
        //         "stop"           => ViewModel.Stop,
        //         "reset"          => ViewModel.Reset,
        //         "clear"          => ViewModel.Clear,
        //         _                => null
        //     };
        //     if (Callback == null) {
        //         Console.WriteLine($"Unknown keybind action name: {Name} on line: {Line}");
        //         continue;
        //     }
        //
        //     Console.WriteLine($"Registering keybind: {Name} {string.Join("+", Keys)}");
        //     Keybind Keybind = new(Keys);
        //     Keybind.Activated += Callback;
        //     _Keybinds.Add(Keybind);
        // }
        //
        // Console.WriteLine(
        //     _Keybinds.Count > 0
        //         ? $"Registered {_Keybinds.Count} keybinds [{string.Join(", ", _Keybinds)}]"
        //         : Lines.Length > 0
        //             ? "Found keybinds file, but no keybinds were registered"
        //             : "Found keybinds file, but the file was empty"
        // );
    }

    async void InitAsync() {
        KeybindsFile Keybinds = await SettingsViewModel.GetOrCreateKeybindsFile();
        foreach (BoundAction Keybind in Keybinds) {
            Action Callback = Keybind.Action switch {
                KeybindAction.StartOrReset => ViewModel.StartOrReset,
                KeybindAction.StopOrClear  => ViewModel.StopOrClear,
                KeybindAction.Rollover     => ViewModel.Rollover,
                KeybindAction.Cancel       => ViewModel.Cancel,
                KeybindAction.Start        => ViewModel.Start,
                KeybindAction.Stop         => ViewModel.Stop,
                KeybindAction.Reset        => ViewModel.Reset,
                KeybindAction.Clear        => ViewModel.Clear,
                _                          => throw new InvalidEnumArgumentException(nameof(Keybind.Action), (int)Keybind.Action, typeof(KeybindAction))
            };
            Keybind Kb = new(Keybind.Keys);
            Kb.Activated += Callback;
            _Keybinds.Add(Kb);
        }
        Console.WriteLine($"Registered {_Keybinds.Count} keybinds [{string.Join(", ", _Keybinds)}]");
    }

    #region IDisposable

    void ReleaseUnmanagedResources() {
        Console.WriteLine($"Disposing RollingTimerView, {_Keybinds.Count} keybinds");
        ViewModel.Dispose();
        foreach (Keybind Keybind in _Keybinds) {
            Keybind.Dispose();
        }
    }
    
    /// <inheritdoc />
    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    
    /// <inheritdoc />
    ~RollingTimerView() {
        ReleaseUnmanagedResources();
    }

    #endregion
}

