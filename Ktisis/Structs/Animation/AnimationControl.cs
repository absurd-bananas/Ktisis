// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.AnimationControl
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit)]
public struct AnimationControl {
	[FieldOffset(0)]
	public unsafe IntPtr* __vfTable;
	[FieldOffset(56)]
	public unsafe hkaDefaultAnimationControl* HavokControl;

	[StructLayout(LayoutKind.Explicit, Size = 40)]
	public struct Handle {
		[FieldOffset(0)]
		public ReferencedClassBase Ref;
		[FieldOffset(24)]
		public StdSet<Pointer<AnimationControl>> Set;
	}
}
