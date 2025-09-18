// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.Converters.JsonFileConverter
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Reflection;

using Ktisis.Data.Files;

namespace Ktisis.Data.Json.Converters;

public class JsonFileConverter : JsonConverter<JsonFile> {
	public virtual bool CanConvert(Type t) => t.BaseType == typeof(JsonFile);

	public virtual JsonFile? Read(
		ref Utf8JsonReader reader,
		Type typeToConvert,
		JsonSerializerOptions options
	) {
		var instance = (JsonFile)Activator.CreateInstance(typeToConvert);
		using (JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader)) {
			foreach (var property in typeToConvert.GetProperties()) {
				JsonElement rootElement = jsonDocument.RootElement;
				JsonElement jsonElement;
				if (!((JsonElement) ref rootElement ).TryGetProperty(property.Name, ref jsonElement))
				{
					var customAttribute = property.GetCustomAttribute<DeserializerDefaultAttribute>();
					if (customAttribute != null)
						property.SetValue(instance, customAttribute.Default);
				}
				else
				{
					try {
						object obj = JsonSerializer.Deserialize(jsonElement, property.PropertyType, options);
						if (obj != null)
							property.SetValue(instance, obj);
					} catch {
						Ktisis.Ktisis.Log.Warning($"Failed to parse {property.PropertyType.Name} value '{property.Name}' (received {((JsonElement) ref jsonElement).ValueKind} instead)", Array.Empty<object>());
					}
				}
			}
			return instance;
		}
	}

	public virtual void Write(Utf8JsonWriter writer, JsonFile value, JsonSerializerOptions options) { }
}
