using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using SignalBooster.ViewModels;

namespace SignalBooster.Converters; 

public abstract class ValueConverter<TIn, TOut> : PropertyNotifier, IValueConverter {

    #region Implementation of IValueConverter

    /// <inheritdoc />
    object? IValueConverter.Convert( object? Value, Type TargetType, object? Parameter, CultureInfo Culture ) =>
        Value switch {
            TIn Input   => Convert(Input, TargetType, Parameter, Culture),
            TOut Output => ConvertBack(Output, TargetType, Parameter, Culture),
            null        => DependencyProperty.UnsetValue,
            _           => throw new ArgumentException($"Value must be of type {typeof(TIn)} or {typeof(TOut)}")
        };

    /// <inheritdoc />
    object? IValueConverter.ConvertBack( object? Value, Type TargetType, object? Parameter, CultureInfo Culture ) =>
        Value switch {
            TOut Output => ConvertBack(Output, TargetType, Parameter, Culture),
            TIn Input   => Convert(Input, TargetType, Parameter, Culture),
            null        => DependencyProperty.UnsetValue,
            _           => throw new ArgumentException($"Value must be of type {typeof(TIn)} or {typeof(TOut)}")
        };

    #endregion
    
    /// <inheritdoc cref="IValueConverter.Convert"/>
    protected abstract TOut Convert( TIn Value, Type TargetType, object? Parameter, CultureInfo Culture );
    
    /// <inheritdoc cref="IValueConverter.ConvertBack"/>
    protected virtual TIn ConvertBack( TOut Value, Type TargetType, object? Parameter, CultureInfo Culture ) => throw new NotSupportedException();

}
