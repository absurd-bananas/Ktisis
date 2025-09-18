// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Vfx.Apricot.ApricotCore
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Vfx.Apricot;

[StructLayout(LayoutKind.Explicit)]
public struct ApricotCore
{
  [FieldOffset(3376)]
  public unsafe ApricotCore.DataContainer* Data;

  [StructLayout(LayoutKind.Explicit)]
  public struct DataContainer
  {
    [FieldOffset(8240)]
    public unsafe fixed byte Instances[278528];

    public unsafe InstanceContainer* GetIndex(uint index)
    {
      if (index > 2048U /*0x0800*/)
        throw new IndexOutOfRangeException($"Index {index} is out of range.");
      fixed (byte* numPtr = this.Instances)
        return (InstanceContainer*) (numPtr + (long) index * (long) sizeof (InstanceContainer));
    }

    public unsafe Span<InstanceContainer> GetInstancesSpan()
    {
      fixed (byte* numPtr = this.Instances)
        return new Span<InstanceContainer>((void*) numPtr, 136);
    }
  }
}
