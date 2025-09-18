// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Vfx.VfxControl
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

using Ktisis.Structs.Animation;

namespace Ktisis.Structs.Vfx;

[StructLayout(LayoutKind.Explicit, Size = 240 /*0xF0*/)]
public struct VfxControl {
	[FieldOffset(0)]
	public SchedulerState State;
	[FieldOffset(40)]
	public unsafe Ktisis.Structs.Vfx.SchedulerVfx* SchedulerVfx;
}
