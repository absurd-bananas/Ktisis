// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Actors.EmoteController
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Actors;

[StructLayout(LayoutKind.Explicit)]
public struct EmoteController
{
  [FieldOffset(32 /*0x20*/)]
  public PoseModeEnum Mode;
  [FieldOffset(33)]
  public byte Pose;
  [FieldOffset(53)]
  public bool IsForceDefaultPose;
  [FieldOffset(55)]
  public bool IsDrawObjectOffset;
}
