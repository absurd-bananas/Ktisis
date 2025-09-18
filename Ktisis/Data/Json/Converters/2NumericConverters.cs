// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.Converters.Vector4Converter
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace Ktisis.Data.Json.Converters;

internal class Vector4Converter : JsonConverter<Vector4>
{
  public Vector4Converter(JsonFileSerializer json)
  {
  }

  public virtual Vector4 Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
  {
    string[] strArray = (((Utf8JsonReader) ref reader).GetString() ?? "").Split(",", StringSplitOptions.None);
    return new Vector4(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture));
  }

  public virtual void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options)
  {
    Utf8JsonWriter utf8JsonWriter = writer;
    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
    \u003C\u003Ey__InlineArray4<object> buffer = new \u003C\u003Ey__InlineArray4<object>();
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 0) = (object) value.X;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 1) = (object) value.Y;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 2) = (object) value.Z;
    // ISSUE: reference to a compiler-generated method
    \u003CPrivateImplementationDetails\u003E.InlineArrayElementRef<\u003C\u003Ey__InlineArray4<object>, object>(ref buffer, 3) = (object) value.W;
    // ISSUE: reference to a compiler-generated method
    ReadOnlySpan<object> readOnlySpan = \u003CPrivateImplementationDetails\u003E.InlineArrayAsReadOnlySpan<\u003C\u003Ey__InlineArray4<object>, object>(in buffer, 4);
    string str = string.Format((IFormatProvider) invariantCulture, "{0}, {1}, {2}, {3}", readOnlySpan);
    utf8JsonWriter.WriteStringValue(str);
  }
}
