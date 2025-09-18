// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.Clips.HavokAnimationClip
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Animation.Clips;

[StructLayout(LayoutKind.Explicit, Size = 208 /*0xD0*/)]
public struct HavokAnimationClip {
	[FieldOffset(0)]
	public BaseClip Clip;
	[FieldOffset(152)]
	public unsafe Ktisis.Structs.Animation.MotionControl* MotionControl;
	[FieldOffset(160 /*0xA0*/)]
	public unsafe char* AnimationName;
}
