// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Camera.Types.KtisisCamera
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable enable
namespace Ktisis.Editor.Camera.Types;

public class KtisisCamera(ICameraManager manager) : EditorCamera(manager), IDisposable
{
  private Ktisis.Interop.Alloc<FFXIVClientStructs.FFXIV.Client.Game.Camera>? Alloc = new Ktisis.Interop.Alloc<FFXIVClientStructs.FFXIV.Client.Game.Camera>();

  public override IntPtr Address
  {
    get
    {
      Ktisis.Interop.Alloc<FFXIVClientStructs.FFXIV.Client.Game.Camera> alloc = this.Alloc;
      return alloc == null ? IntPtr.Zero : alloc.Address;
    }
  }

  public void Dispose()
  {
    this.Alloc?.Dispose();
    this.Alloc = (Ktisis.Interop.Alloc<FFXIVClientStructs.FFXIV.Client.Game.Camera>) null;
    GC.SuppressFinalize((object) this);
  }
}
