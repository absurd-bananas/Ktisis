// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.TimelineTrack
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Structs.Animation.Clips;
using Ktisis.Structs.Common;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit)]
public struct TimelineTrack
{
  [FieldOffset(0)]
  public SchedulerState SchedulerState;
  [FieldOffset(24)]
  public PtrList<BaseClip> Clips;
  [FieldOffset(40)]
  public unsafe byte* ResourceData;
}
