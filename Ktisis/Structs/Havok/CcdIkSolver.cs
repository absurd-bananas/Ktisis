// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Havok.CcdIkSolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.Havok.Common.Base.Object;
using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Havok;

[StructLayout(LayoutKind.Explicit)]
public struct CcdIkSolver
{
  [FieldOffset(0)]
  public unsafe IntPtr** _vfTable;
  [FieldOffset(0)]
  public hkReferencedObject hkRefObject;
  [FieldOffset(16 /*0x10*/)]
  public int m_iterations;
  [FieldOffset(20)]
  public float m_gain;
}
