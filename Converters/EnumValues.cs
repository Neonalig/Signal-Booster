using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SignalBooster.Converters; 

public class EnumValues<TEnum> : ObservableCollection<TEnum> where TEnum : Enum {
    protected virtual Type? GetEnumType() => typeof(TEnum);

    // ReSharper disable once MemberCanBeProtected.Global
    public EnumValues() => Populate();

    protected void Populate() {
        Type? EnumType = GetEnumType();
        Clear();
        if (EnumType is not null) {
            HashSet<TEnum> Values = new();
            foreach (object Member in Enum.GetValues(EnumType)) {
                TEnum Value = (TEnum)Member;
                if (Values.Add(Value)) {
                    Add(Value);
                }
            }
        }
    }
}

public sealed class EnumValues : EnumValues<Enum> {
    Type? _EnumType = null;
    /// <summary> Gets or sets the type of the enum to get values for. </summary>
    public Type EnumType {
        get => _EnumType ?? throw new InvalidOperationException("EnumType is null.");
        set {
            if (_EnumType == value) { return; }
            _EnumType = value;
            OnPropertyChanged(new(nameof(EnumType)));
            Populate();
        }
    }

    #region Overrides of EnumValues<Enum>

    /// <inheritdoc />
    protected override Type? GetEnumType() => _EnumType;

    #endregion

}
