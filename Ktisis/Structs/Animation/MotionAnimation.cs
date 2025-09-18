// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.MotionAnimation
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;
using System.Runtime.InteropServices;

using Ktisis.Structs.Animation.Clips;

namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit, Size = 96 /*0x60*/)]
public struct MotionAnimation {
	[FieldOffset(0)]
	public unsafe IntPtr* __vfTable;
	[FieldOffset(32 /*0x20*/)]
	public unsafe AnimationControl.Handle* AnimationControls;
	[FieldOffset(40)]
	public unsafe MotionControl* ParentControl;
	[FieldOffset(48 /*0x30*/)]
	public unsafe HavokAnimationClip* ParentClip;
}
