using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SignalBooster.Converters; 

[ValueConversion(typeof(double), typeof(Brush))]
public sealed class ProgressBarTriColourConverter : ValueConverter<double, Brush> {
    /// <summary> The dependency property for <see cref="Low"/>. </summary>
    public static readonly DependencyProperty LowProperty = DependencyProperty.Register(
        nameof(Low),
        typeof(Color),
        typeof(ProgressBarTriColourConverter),
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
        typeof(ProgressBarTriColourConverter),
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
        typeof(ProgressBarTriColourConverter),
        new(0.0)
    );
    
    /// <summary> The minimum value of the progress bar. </summary>
    public double Minimum {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="Mid"/>. </summary>
    public static readonly DependencyProperty MidProperty = DependencyProperty.Register(
        nameof(Mid),
        typeof(Color),
        typeof(ProgressBarTriColourConverter),
        new(Colors.Yellow)
    );
    
    /// <summary> The brush to use for the middle of the progress bar. </summary>
    public Color Mid {
        get => (Color)GetValue(MidProperty);
        set => SetValue(MidProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="MidOpacity"/>. </summary>
    public static readonly DependencyProperty MidOpacityProperty = DependencyProperty.Register(
        nameof(MidOpacity),
        typeof(double),
        typeof(ProgressBarTriColourConverter),
        new(1.0)
    );
    
    /// <summary> The opacity to use for the middle of the progress bar. </summary>
    public double MidOpacity {
        get => (double)GetValue(MidOpacityProperty);
        set => SetValue(MidOpacityProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="MidPoint"/>. </summary>
    public static readonly DependencyProperty MidPointProperty = DependencyProperty.Register(
        nameof(MidPoint),
        typeof(double),
        typeof(ProgressBarTriColourConverter),
        new(0.5)
    );
    
    /// <summary> The point at which the progress bar transitions from low to high. </summary>
    public double MidPoint {
        get => (double)GetValue(MidPointProperty);
        set => SetValue(MidPointProperty, value);
    }
    
    /// <summary> The dependency property for <see cref="High"/>. </summary>
    public static readonly DependencyProperty HighProperty = DependencyProperty.Register(
        nameof(High),
        typeof(Color),
        typeof(ProgressBarTriColourConverter),
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
        typeof(ProgressBarTriColourConverter),
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
        typeof(ProgressBarTriColourConverter),
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
        double LerpValue = InverseLerp(Minimum, Maximum, Value);
        if (LerpValue < MidPoint) {
            return new SolidColorBrush(Lerp(Low, LowOpacity, Mid, MidOpacity, InverseLerp(0, MidPoint, LerpValue)));
        }
        return new SolidColorBrush(Lerp(Mid, MidOpacity, High, HighOpacity, InverseLerp(MidPoint, 1, LerpValue)));
    }

    #endregion

}
