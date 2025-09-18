// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.Converters.TransformConverter
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Common.Utility;
using System;
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace Ktisis.Data.Json.Converters;

internal class TransformConverter : JsonConverter<Transform>
{
  public TransformConverter(JsonFileSerializer json)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003Cjson\u003EP = json;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public virtual Transform Read(
    ref Utf8JsonReader reader,
    Type type,
    JsonSerializerOptions options)
  {
    Transform transform = new Transform();
    ((Utf8JsonReader) ref reader).Read();
    for (int index = 0; index < 3 && ((Utf8JsonReader) ref reader).TokenType != 2; ++index)
    {
      string str = ((Utf8JsonReader) ref reader).GetString();
      ((Utf8JsonReader) ref reader).Read();
      if (str == "Rotation")
      {
        // ISSUE: reference to a compiler-generated field
        transform.Rotation = this.\u003Cjson\u003EP.GetConverter<Quaternion>().Read(ref reader, type, options);
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        Vector3 vector3 = this.\u003Cjson\u003EP.GetConverter<Vector3>().Read(ref reader, type, options);
        if (str == "Position")
          transform.Position = vector3;
        else if (str == "Scale")
          transform.Scale = vector3;
      }
      ((Utf8JsonReader) ref reader).Read();
    }
    return transform;
  }

  public virtual void Write(Utf8JsonWriter writer, Transform value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();
    foreach (FieldInfo field in typeof (Transform).GetFields())
    {
      writer.WritePropertyName(((MemberInfo) field).Name);
      switch (field.GetValue((object) value))
      {
        case Vector3 vector3:
          // ISSUE: reference to a compiler-generated field
          this.\u003Cjson\u003EP.GetConverter<Vector3>().Write(writer, vector3, options);
          break;
        case Quaternion quaternion:
          // ISSUE: reference to a compiler-generated field
          this.\u003Cjson\u003EP.GetConverter<Quaternion>().Write(writer, quaternion, options);
          break;
      }
    }
    writer.WriteEndObject();
  }
}
