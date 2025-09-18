// Decompiled with JetBrains decompiler
// Type: Ktisis.Structs.Common.PtrList`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Ktisis.Structs.Common;

[StructLayout(LayoutKind.Sequential, Size = 16 /*0x10*/)]
public struct PtrList<T> where T : unmanaged
{
  public unsafe T** Pointers;
  public ushort Capacity;
  public ushort Length;
}
