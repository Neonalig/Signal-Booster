using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Windows.Input;
using SignalBooster.Converters;

namespace SignalBooster.Models;

public sealed class KeybindsFile : IList<BoundAction>, IReadOnlyList<BoundAction> {
    /// <summary> The keybinds. </summary>
    public IList<BoundAction> BoundActions { get; }

    /// <summary> Creates a new <see cref="KeybindsFile"/> with the default keybinds. </summary>
    public KeybindsFile() => BoundActions = new List<BoundAction>();
    
    /// <summary> Creates a new <see cref="KeybindsFile"/> with the given <paramref name="Keybinds"/>. </summary>
    /// <param name="Keybinds"> The keybinds to use. </param>
    public KeybindsFile( IEnumerable<BoundAction> Keybinds ) => BoundActions = new List<BoundAction>(Keybinds);

    /// <inheritdoc cref="KeybindsFile(IEnumerable{BoundAction})"/>
    public KeybindsFile( params BoundAction[] Keybinds ) => BoundActions = new List<BoundAction>(Keybinds);

    /// <summary> The default keybinds. </summary>
    public static KeybindsFile Default => new(
        new(KeybindAction.Rollover, Key.Subtract),
        new(KeybindAction.Cancel, Key.Multiply)
    );

    #region Implementation of IEnumerable

    /// <inheritdoc />
    public IEnumerator<BoundAction> GetEnumerator() => BoundActions.GetEnumerator();
    
    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)BoundActions).GetEnumerator();

    #endregion

    #region Implementation of ICollection<BoundAction>

    /// <inheritdoc />
    public void Add( BoundAction Item ) => BoundActions.Add(Item);
    
    /// <inheritdoc />
    public void Clear() => BoundActions.Clear();
    
    /// <inheritdoc />
    public bool Contains( BoundAction Item ) => BoundActions.Contains(Item);
    
    /// <inheritdoc />
    public void CopyTo( BoundAction[] Array, int ArrayIndex ) => BoundActions.CopyTo(Array, ArrayIndex);
    
    /// <inheritdoc />
    public bool Remove( BoundAction Item ) => BoundActions.Remove(Item);
    
    /// <inheritdoc cref="ICollection{T}.Count"/>
    public int Count => BoundActions.Count;
    
    /// <inheritdoc />
    bool ICollection<BoundAction>.IsReadOnly => BoundActions.IsReadOnly;

    #endregion

    #region Implementation of IList<BoundAction>

    /// <inheritdoc />
    public int IndexOf( BoundAction Item ) => BoundActions.IndexOf(Item);
    
    /// <inheritdoc />
    public void Insert( int Index, BoundAction Item ) => BoundActions.Insert(Index, Item);
    
    /// <inheritdoc />
    public void RemoveAt( int Index ) => BoundActions.RemoveAt(Index);

    /// <inheritdoc cref="IList{T}.this"/>
    public BoundAction this[ int Index ] {
        get => BoundActions[Index];
        set => BoundActions[Index] = value;
    }

    #endregion

}

/// <summary> Represents a keybind and its action. </summary>
/// <param name="Action"> The action to perform. </param>
/// <param name="Keys"> The keys to bind to. </param>
public readonly record struct BoundAction( [property: JsonPropertyName("action")] KeybindAction Action, [property: JsonPropertyName("keys")] params Key[] Keys );

public enum KeybindAction {
    [JsonEnumMemberName("start_or_reset")]
    [Display(Name = "Start or Reset")]
    StartOrReset,
    [JsonEnumMemberName("stop_or_clear")]
    [Display(Name = "Stop or Clear")]
    StopOrClear,
    [JsonEnumMemberName("rollover")]
    Rollover,
    [JsonEnumMemberName("cancel")]
    Cancel,
    [JsonEnumMemberName("start")]
    Start,
    [JsonEnumMemberName("stop")]
    Stop,
    [JsonEnumMemberName("reset")]
    Reset,
    [JsonEnumMemberName("clear")]
    Clear
}