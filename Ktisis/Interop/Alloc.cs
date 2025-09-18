// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Alloc`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.System.Memory;
using System;

#nullable disable
namespace Ktisis.Interop;

public class Alloc<T> : IDisposable where T : unmanaged
{
  public IntPtr Address { get; private set; }

  public unsafe T* Data => (T*) this.Address;

  public bool IsDisposed { get; private set; }

  public unsafe Alloc(ulong align = 8)
  {
    this.Address = (IntPtr) ((IMemorySpace) (IntPtr) IMemorySpace.GetDefaultSpace()).Malloc<T>(align);
  }

  public unsafe void Dispose()
  {
    if (this.IsDisposed)
      return;
    if (this.Address != IntPtr.Zero)
    {
      IMemorySpace.Free<T>(this.Data);
      this.Address = IntPtr.Zero;
    }
    this.IsDisposed = true;
    GC.SuppressFinalize((object) this);
  }

  ~Alloc() => this.Dispose();
}
