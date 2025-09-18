// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Havok.CcdIkConstraint
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Numerics;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Havok;

[StructLayout(LayoutKind.Explicit)]
public struct CcdIkConstraint
{
  [FieldOffset(0)]
  public short m_startBone;
  [FieldOffset(2)]
  public short m_endBone;
  [FieldOffset(16 /*0x10*/)]
  public Vector4 m_targetMS;
}
