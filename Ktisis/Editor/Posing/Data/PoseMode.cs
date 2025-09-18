// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.PoseMode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

namespace Ktisis.Editor.Posing.Data;

[Flags]
public enum PoseMode {
	None = 0,
	Body = 1,
	Face = 2,
	BodyFace = Face | Body, // 0x00000003
	Weapons = 4,
	All = Weapons | BodyFace // 0x00000007
}
