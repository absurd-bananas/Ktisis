// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.Converters.Vector3Converter
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

internal class Vector3Converter : JsonConverter<Vector3>
{
  public Vector3Converter(JsonFileSerializer json)
  {
  }

  public virtual Vector3 Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
  {
    string[] strArray = (((Utf8JsonReader) ref reader).GetString() ?? "").Split(",", StringSplitOptions.None);
    return new Vector3(float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture));
  }

  public virtual void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, {1}, {2}", (object) value.X, (object) value.Y, (object) value.Z));
  }
}
