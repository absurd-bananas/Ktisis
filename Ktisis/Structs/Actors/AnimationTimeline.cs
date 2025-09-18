// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Actors.AnimationTimeline
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Structs.Animation;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Actors;

[StructLayout(LayoutKind.Explicit, Size = 496)]
public struct AnimationTimeline
{
  [FieldOffset(0)]
  public unsafe IntPtr** __vfTable;
  [FieldOffset(112 /*0x70*/)]
  public unsafe fixed ulong SchedulerTimelines[14];
  [FieldOffset(224 /*0xE0*/)]
  public unsafe fixed ushort TimelineIds[14];
  [FieldOffset(252)]
  public unsafe fixed ushort CurrentTimelineIds[14];
  [FieldOffset(280)]
  public unsafe fixed ushort PreviousTimelineIds[14];
  [FieldOffset(340)]
  public unsafe fixed float TimelineSpeeds[14];
  [FieldOffset(396)]
  public unsafe fixed float TimelineWeights[14];
  [FieldOffset(720)]
  public ushort ActionTimelineId;

  public unsafe SchedulerTimeline* GetSchedulerTimeline(int slot)
  {
    ulong num = this.SchedulerTimelines[slot];
    if (this.SchedulerTimelines[slot] == 0UL)
      return (SchedulerTimeline*) null;
    SchedulerTimeline.Handle* handlePtr = (SchedulerTimeline.Handle*) num;
    return handlePtr->Flags == 0U ? (SchedulerTimeline*) null : handlePtr->Data;
  }
}
