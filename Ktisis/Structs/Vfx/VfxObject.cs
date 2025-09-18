// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Vfx.VfxObject
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Vfx;

[StructLayout(LayoutKind.Explicit, Size = 832)]
public struct VfxObject
{
  [FieldOffset(0)]
  public Object Object;
  [FieldOffset(608)]
  public Vector4 Color;
  [FieldOffset(672)]
  public unsafe VfxResourceInstance* ResourceInstance;
}
