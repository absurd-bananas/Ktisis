// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Common.ObjectUnion
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable disable
namespace Ktisis.Structs.Common;

public struct ObjectUnion
{
  public unsafe IntPtr** __vfTable;
  public IntPtr Data;

  public IntPtr GetObjectPointer()
  {
    return (this.Data & new IntPtr(1)) == IntPtr.Zero ? IntPtr.Zero : this.Data & new IntPtr(-8);
  }

  public short GetObjectIndex()
  {
    return (this.Data & new IntPtr(4)) == IntPtr.Zero ? (short) -1 : (short) (this.Data >> 3);
  }
}
