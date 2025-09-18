// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.Types.CameraFlags
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

namespace Ktisis.Editor.Camera.Types;

[Flags]
public enum CameraFlags {
	None = 0,
	DefaultCamera = 1,
	NoCollide = 2,
	Delimit = 4,
	Orthographic = 8
}
