// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Lights.SceneLight
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Lights;

[StructLayout(LayoutKind.Explicit, Size = 160 /*0xA0*/)]
public struct SceneLight
{
  [FieldOffset(0)]
  public unsafe IntPtr* _vf;
  [FieldOffset(0)]
  public DrawObject DrawObject;
  [FieldOffset(80 /*0x50*/)]
  public Transform Transform;
  [FieldOffset(128 /*0x80*/)]
  public IntPtr Culling;
  [FieldOffset(136)]
  public byte Flags00;
  [FieldOffset(137)]
  public byte Flags01;
  [FieldOffset(144 /*0x90*/)]
  public unsafe Ktisis.Structs.Lights.RenderLight* RenderLight;
}
