using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SignalBooster.Converters; 

[ValueConversion(typeof(double), typeof(Brush))]
public sealed class ProgressBarColourConverter : ValueConverter<double, Brush> {
    /// <summary> The dependency property for <see cref="Low"/>. </summary>
    public static readonly DependencyProperty LowProperty = DependencyProperty.Register(
        nameof(Low),
        typeof(Color),
        typeof(ProgressBarColourConverter),
        new(Colors.Red)
    );
    
    /// <summary> The brush to use for the lower end of the progress bar. </summary>
    public Color Low {
        get => (Color)GetValue(LowProperty);
        set => SetValue(LowProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="LowOpacity"/>. </summary>
    public static readonly DependencyProperty LowOpacityProperty = DependencyProperty.Register(
        nameof(LowOpacity),
        typeof(double),
        typeof(ProgressBarColourConverter),
        new(1.0)
    );
    
    /// <summary> The opacity to use for the lower end of the progress bar. </summary>
    public double LowOpacity {
        get => (double)GetValue(LowOpacityProperty);
        set => SetValue(LowOpacityProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="Minimum"/>. </summary>
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum),
        typeof(double),
        typeof(ProgressBarColourConverter),
        new(0.0)
    );
    
    /// <summary> The minimum value of the progress bar. </summary>
    public double Minimum {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="High"/>. </summary>
    public static readonly DependencyProperty HighProperty = DependencyProperty.Register(
        nameof(High),
        typeof(Color),
        typeof(ProgressBarColourConverter),
        new(Colors.Green)
    );
    
    /// <summary> The brush to use for the higher end of the progress bar. </summary>
    public Color High {
        get => (Color)GetValue(HighProperty);
        set => SetValue(HighProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="HighOpacity"/>. </summary>
    public static readonly DependencyProperty HighOpacityProperty = DependencyProperty.Register(
        nameof(HighOpacity),
        typeof(double),
        typeof(ProgressBarColourConverter),
        new(1.0)
    );
    
    /// <summary> The opacity to use for the higher end of the progress bar. </summary>
    public double HighOpacity {
        get => (double)GetValue(HighOpacityProperty);
        set => SetValue(HighOpacityProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="Maximum"/>. </summary>
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum),
        typeof(double),
        typeof(ProgressBarColourConverter),
        new(1.0)
    );
    
    /// <summary> The maximum value of the progress bar. </summary>
    public double Maximum {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    [Pure]
    static double InverseLerp( double A, double B, double Value ) => (Value - A) / (B - A);
    
    [Pure]
    static Color Lerp( Color A, double AOp, Color B, double BOp, double Value ) => Color.FromArgb(
        (byte)((AOp + (BOp - AOp) * Value) * 255),
        (byte)(A.R + (B.R - A.R) * Value),
        (byte)(A.G + (B.G - A.G) * Value),
        (byte)(A.B + (B.B - A.B) * Value)
    );

    #region Overrides of ValueConverter<double,Brush>

    /// <inheritdoc />
    protected override Brush Convert( double Value, Type TargetType, object? Parameter, CultureInfo Culture ) {
        double Time   = InverseLerp(Minimum, Maximum, Value);
        Color  Colour = Lerp(Low, LowOpacity, High, HighOpacity, Time);
        return new SolidColorBrush(Colour);
    }

    #endregion

}
