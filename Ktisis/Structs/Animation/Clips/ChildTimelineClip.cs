// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.Clips.ChildTimelineClip
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation.Clips;

[StructLayout(LayoutKind.Explicit, Size = 352)]
public struct ChildTimelineClip
{
  [FieldOffset(0)]
  public BaseClip Clip;
  [FieldOffset(204)]
  public float ChildFrame;
  [FieldOffset(208 /*0xD0*/)]
  public float PrevChildFrame;
  [FieldOffset(296)]
  public unsafe SchedulerTimeline* ParentTimeline;
  [FieldOffset(304)]
  public unsafe TimelineController* ChildTimeline;
  [FieldOffset(320)]
  public unsafe byte* Data;
}
