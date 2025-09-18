// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.Clips.BaseClip
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation.Clips;

[StructLayout(LayoutKind.Explicit, Size = 152)]
public struct BaseClip
{
  [FieldOffset(0)]
  public unsafe IntPtr* __vfTable;
  [FieldOffset(0)]
  public SchedulerState SchedulerState;
  [FieldOffset(40)]
  public unsafe Ktisis.Structs.Animation.TrackController* TrackController;
  [FieldOffset(48 /*0x30*/)]
  public unsafe TimelineController* ParentTimeline;
  [FieldOffset(56)]
  public unsafe TimelineController* RootTimeline;
  [FieldOffset(72)]
  public unsafe byte* Data;
  [FieldOffset(80 /*0x50*/)]
  public float TrackStartFrame;
  [FieldOffset(84)]
  public float TrackTotalFrames;
  [FieldOffset(92)]
  public float DeltaFrames;
  [FieldOffset(100)]
  public float ClipStartFrame;
  [FieldOffset(104)]
  public float ClipTotalFrames;
  [FieldOffset(132)]
  public ClipType ClipType;
}
