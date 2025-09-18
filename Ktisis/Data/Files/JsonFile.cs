// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Files.JsonFile
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Data.Files;

[Serializable]
public class JsonFile {
	public string FileExtension { get; set; } = ".json";

	public string TypeName { get; set; } = "Json File";

	public int FileVersion { get; set; } = 1;
}
