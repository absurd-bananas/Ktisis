// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.TimelineController
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit, Size = 128 /*0x80*/)]
public struct TimelineController {
	[FieldOffset(0)]
	public SchedulerState SchedulerState;
	[FieldOffset(24)]
	public unsafe Ktisis.Structs.Animation.TrackController* TrackController;
	[FieldOffset(32 /*0x20*/)]
	public unsafe void* Child;
	[FieldOffset(40)]
	public unsafe byte* Data;
	[FieldOffset(80 /*0x50*/)]
	public uint QueuedClipCount;
	[FieldOffset(84)]
	public uint Flags;
	[FieldOffset(88)]
	public uint Unk1;
	[FieldOffset(92)]
	public uint Unk2;
}
