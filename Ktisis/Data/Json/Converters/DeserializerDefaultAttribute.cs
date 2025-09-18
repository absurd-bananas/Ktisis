// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Json.Converters.DeserializerDefaultAttribute
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Data.Json.Converters;

public class DeserializerDefaultAttribute(object value) : Attribute {
	public readonly object Default = value;
}
