// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.SchedulerTimeline
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Structs.Common;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit, Size = 628)]
public struct SchedulerTimeline
{
  [FieldOffset(0)]
  public TimelineController Controller;
  [FieldOffset(144 /*0x90*/)]
  public unsafe Ktisis.Structs.Animation.TimelineGroup* TimelineGroup;
  [FieldOffset(152)]
  public unsafe FFXIVClientStructs.FFXIV.Client.System.Scheduler.Resource.SchedulerResource* SchedulerResource;
  [FieldOffset(168)]
  public unsafe char* FilePath1;
  [FieldOffset(176 /*0xB0*/)]
  public unsafe char* FilePath2;
  [FieldOffset(216)]
  public ObjectUnion UnkObject1;
  [FieldOffset(240 /*0xF0*/)]
  public ObjectUnion UnkObject2;
  [FieldOffset(368)]
  public unsafe byte* UnkData;
  [FieldOffset(384)]
  public unsafe SchedulerTimeline.Handle* TimelineHandle;
  [FieldOffset(396)]
  public uint ObjectIndex;
  [FieldOffset(400)]
  public uint TargetIndex;
  [FieldOffset(548)]
  public unsafe fixed char FilePathBuffer[40];

  [StructLayout(LayoutKind.Sequential, Size = 16 /*0x10*/)]
  public struct Handle
  {
    public unsafe SchedulerTimeline* Data;
    public uint Flags;
  }
}
