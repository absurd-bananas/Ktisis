// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Vfx.VfxResourceInstance
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Vfx;

[StructLayout(LayoutKind.Explicit, Size = 192 /*0xC0*/)]
public struct VfxResourceInstance
{
  [FieldOffset(0)]
  public unsafe IntPtr* __vfTable;
  [FieldOffset(96 /*0x60*/)]
  public VfxResourceHandle Handle;
}
