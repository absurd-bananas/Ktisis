// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.TrackController
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System.Runtime.InteropServices;

using Ktisis.Structs.Common;

namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit)]
public struct TrackController {
	[FieldOffset(0)]
	public SchedulerState SchedulerState;
	[FieldOffset(40)]
	public PtrList<TimelineTrack> Tracks;
}
