// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.JsonFileSerializer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Data.Json.Converters;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable
namespace Ktisis.Data.Json;

public class JsonFileSerializer
{
  private readonly JsonSerializerOptions Options;
  private readonly JsonSerializerOptions DeserializeOptions;

  public JsonFileSerializer()
  {
    this.Options = new JsonSerializerOptions()
    {
      WriteIndented = true,
      PropertyNameCaseInsensitive = false,
      AllowTrailingCommas = true,
      ReadCommentHandling = (JsonCommentHandling) 1,
      DefaultIgnoreCondition = (JsonIgnoreCondition) 3
    };
    this.Options.Converters.Add((JsonConverter) new JsonStringEnumConverter());
    this.Options.Converters.Add((JsonConverter) new QuaternionConverter(this));
    this.Options.Converters.Add((JsonConverter) new Vector3Converter(this));
    this.Options.Converters.Add((JsonConverter) new Vector4Converter(this));
    this.Options.Converters.Add((JsonConverter) new TransformConverter(this));
    this.DeserializeOptions = new JsonSerializerOptions(this.Options);
    this.DeserializeOptions.Converters.Add((JsonConverter) new JsonFileConverter());
  }

  public JsonConverter<T> GetConverter<T>()
  {
    return (JsonConverter<T>) this.Options.GetConverter(typeof (T));
  }

  public string Serialize(object obj) => JsonSerializer.Serialize<object>(obj, this.Options);

  public T? Deserialize<T>(string json) where T : notnull
  {
    return JsonSerializer.Deserialize<T>(json, this.DeserializeOptions);
  }
}
