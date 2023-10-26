using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace SignalBooster.Converters; 

[ValueConversion(typeof(Enum), typeof(string))]
public sealed class EnumToDisplayStringConverter : ValueConverter<Enum, string> {

    #region Overrides of ValueConverter<TEnum,string>

    /// <inheritdoc />
    protected override string Convert( Enum Value, Type TargetType, object? Parameter, CultureInfo Culture ) {
        Type        EnumType = Value.GetType();
        MemberInfo? Member   = EnumType.GetMember(Value.ToString()).FirstOrDefault();
        if (Member is null) {
            Trace.TraceWarning($"Failed to get member for {Value}");
        }
        return Member?.GetCustomAttribute<DisplayAttribute>()?.Name ?? Value.ToString();
    }

    #endregion

}
