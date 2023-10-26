using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SignalBooster.Converters;

public class JsonStringEnumMemberConverter<TEnum> : JsonConverter<TEnum> where TEnum : Enum {

    /// <summary> Gets the type to convert. </summary>
    /// <param name="TypeToConvert"> The type to convert defined in the <see cref="Read"/> method. </param>
    /// <returns> The type to convert. </returns>
    protected virtual Type GetTypeToConvert( Type TypeToConvert ) => typeof(TEnum);

    #region Overrides of JsonConverter<TEnum>

    /// <inheritdoc />
    public override bool CanConvert( Type TypeToConvert ) => GetTypeToConvert(TypeToConvert).IsAssignableFrom(TypeToConvert);

    #endregion

    string GetNameOfMember( Type TypeToConvert, TEnum Member ) {
        Type       Container  = GetTypeToConvert(TypeToConvert);
        string     MemberName = Member.ToString();
        MemberInfo MemberInfo = Container.GetMember(MemberName)[0];
        return MemberInfo.GetCustomAttribute<JsonEnumMemberNameAttribute>()?.Name ?? MemberName;
    }
    
    TEnum GetMemberFromName( Type TypeToConvert, string Name ) {
        Type        Container  = GetTypeToConvert(TypeToConvert);
        MemberInfo? MemberInfo = Container.GetMember(Name).FirstOrDefault();
        if ( MemberInfo is not null ) {
            return (TEnum)Enum.Parse(Container, MemberInfo.Name);
        }

        // Search all member [JsonEnumMemberName] attributes for a match
        foreach (MemberInfo Member in Container.GetMembers()) {
            if (Member.GetCustomAttribute<JsonEnumMemberNameAttribute>()?.Name == Name) {
                return (TEnum)Enum.Parse(Container, Member.Name);
            }
        }
        
        return (TEnum)Enum.Parse(Container, Name);
    }

    #region Overrides of JsonConverter<TEnum>

    /// <inheritdoc />
    public override TEnum Read( ref Utf8JsonReader Reader, Type TypeToConvert, JsonSerializerOptions Options ) {
        if (Reader.TokenType != JsonTokenType.String) {
            throw new JsonException();
        }

        string Name = Reader.GetString()!;
        return GetMemberFromName(TypeToConvert, Name);
    }

    /// <inheritdoc />
    public override void Write( Utf8JsonWriter Writer, TEnum Value, JsonSerializerOptions Options ) {
        Writer.WriteStringValue(GetNameOfMember(Value.GetType(), Value));
    }

    #endregion

}

public sealed class JsonStringEnumMemberConverter : JsonStringEnumMemberConverter<Enum> {

    #region Overrides of JsonStringEnumMemberConverter<Enum>

    /// <inheritdoc />
    protected override Type GetTypeToConvert( Type TypeToConvert ) => TypeToConvert;

    #endregion

}

[AttributeUsage(AttributeTargets.Field)]
public sealed class JsonEnumMemberNameAttribute : Attribute {
    /// <summary> The name of the enum member. </summary>
    public readonly string Name;

    /// <summary> Initialises a new instance of the <see cref="JsonEnumMemberNameAttribute"/> class. </summary>
    /// <param name="Name"> The name of the enum member. </param>
    public JsonEnumMemberNameAttribute( string Name ) => this.Name = Name;
}