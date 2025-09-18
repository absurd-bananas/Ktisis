// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Actors.AnimationContainer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Actors;

[StructLayout(LayoutKind.Explicit, Size = 832)]
public struct AnimationContainer
{
  public const int TimelineOffset = 16 /*0x10*/;
  [FieldOffset(0)]
  public unsafe IntPtr** __vfTable;
  [FieldOffset(8)]
  public unsafe CharacterEx* Character;
  [FieldOffset(16 /*0x10*/)]
  public AnimationTimeline Timeline;
}
