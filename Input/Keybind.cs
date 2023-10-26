using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Keyboard = SignalBooster.Models.Keyboard;

namespace SignalBooster.Input; 

/// <summary> A keybind that can be activated by pressing a sequence of keys. </summary>
/// <remarks> Sequence does not matter. </remarks>
public sealed class Keybind : IDisposable {
    readonly Key[]    _Keys;
    readonly BitArray _Active;
    
    /// <summary> Raised when the keybind is activated. </summary>
    public event Action? Activated;

    bool SetKey( Key Key, bool Value ) {
        int Idx = Array.IndexOf(_Keys, Key);
        if (Idx == -1) { return false; }

        Console.WriteLine($"Keybind: {Key} {(Value ? "pressed" : "released")} / of {_Keys.Length}, {_Active.Cast<bool>().Count(B => B)} are active");
        _Active[Idx] = Value;
        return true;
    }
    
    bool IsPressed( Key Key ) {
        int Idx = Array.IndexOf(_Keys, Key);
        if (Idx == -1) { return false; }

        return _Active[Idx];
    }
    
    bool IsActive => _Active.Cast<bool>().All(B => B);

    /// <summary> Creates a new keybind. </summary>
    /// <param name="Keys"> The keys that must be pressed in order to activate the keybind. </param>
    /// <remarks> Sequence does not matter. </remarks>
    public Keybind( IEnumerable<Key> Keys ) {
        _Keys = Keys.ToArray();
        int Ln = _Keys.Length;
        _Active = new(Ln);

        Keyboard.KeyDown += Keyboard_KeyDown;
        Keyboard.KeyUp   += Keyboard_KeyUp;
    }

    void Keyboard_KeyDown( Key Key ) {
        if (SetKey(Key, true) && IsActive) {
            Console.WriteLine("Keybind activated");
            Activated?.Invoke();
        }
    }
    
    void Keyboard_KeyUp( Key Key ) {
        SetKey(Key, false);
    }

    #region IDisposable

    void ReleaseUnmanagedResources() {
        Keyboard.KeyDown -= Keyboard_KeyDown;
        Keyboard.KeyUp   -= Keyboard_KeyUp;
    }
    
    /// <inheritdoc />
    public void Dispose() {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    
    /// <inheritdoc />
    ~Keybind() {
        ReleaseUnmanagedResources();
    }

    #endregion

    #region Overrides of Object

    /// <inheritdoc />
    public override string ToString() => string.Join("+", _Keys);

    #endregion

}
