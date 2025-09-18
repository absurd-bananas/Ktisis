// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Animation.MotionControl
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Structs.Animation.Clips;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Animation;

[StructLayout(LayoutKind.Explicit, Size = 128 /*0x80*/)]
public struct MotionControl
{
  [FieldOffset(68)]
  public uint FrameCount;
  [FieldOffset(76)]
  public float StartSpeed;
  [FieldOffset(84)]
  public float PlaySpeed;
  [FieldOffset(96 /*0x60*/)]
  public unsafe HavokAnimationClip* ParentClip;
  [FieldOffset(120)]
  public unsafe MotionAnimation* Animation;
}
