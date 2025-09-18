// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.TimelineGroup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Structs.Common;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit, Size = 2680)]
public struct TimelineGroup
{
  [FieldOffset(0)]
  public unsafe IntPtr* __vfTable;
  [FieldOffset(24)]
  public unsafe Ktisis.Structs.Animation.SchedulerTimeline* SchedulerTimeline;
  [FieldOffset(32 /*0x20*/)]
  public unsafe void* Controller;
  [FieldOffset(40)]
  public ObjectUnion Object;
  [FieldOffset(2668)]
  public uint GroupType;
}
