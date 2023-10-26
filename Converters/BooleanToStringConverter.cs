using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SignalBooster.Converters; 

[ValueConversion(typeof(bool), typeof(string))]
public sealed class BooleanToStringConverter : ValueConverter<bool, string> {

    /// <summary> The dependency property for <see cref="True"/>. </summary>
    public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(
        nameof(True),
        typeof(string),
        typeof(BooleanToStringConverter),
        new("True")
    );
    
    /// <summary> The string to use when the value is <see langword="true"/>. </summary>
    public string True {
        get => (string)GetValue(TrueProperty);
        set => SetValue(TrueProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="False"/>. </summary>
    public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(
        nameof(False),
        typeof(string),
        typeof(BooleanToStringConverter),
        new("False")
    );
    
    /// <summary> The string to use when the value is <see langword="false"/>. </summary>
    public string False {
        get => (string)GetValue(FalseProperty);
        set => SetValue(FalseProperty, value);
    }
    
    #region Overrides of ValueConverter<bool,string>

    /// <inheritdoc />
    protected override string Convert( bool Value, Type TargetType, object? Parameter, CultureInfo Culture ) => Value ? True : False;

    #endregion

}
