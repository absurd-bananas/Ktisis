// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.Clips.VfxClip
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Animation.Clips;

[StructLayout(LayoutKind.Explicit, Size = 392)]
public struct VfxClip {
	[FieldOffset(0)]
	public BaseClip Clip;
	[FieldOffset(152)]
	public unsafe Ktisis.Structs.Vfx.VfxControl* VfxControl;
}
