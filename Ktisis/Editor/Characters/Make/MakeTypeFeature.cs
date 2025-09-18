// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.Make.MakeTypeFeature
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Editor.Characters.Make;

public class MakeTypeFeature {
	public CustomizeIndex Index;
	public bool IsCustomize;
	public bool IsIcon;
	public string Name = string.Empty;
	public MakeTypeParam[] Params = Array.Empty<MakeTypeParam>();
}
