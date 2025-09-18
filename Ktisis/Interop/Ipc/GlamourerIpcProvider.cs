// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Ipc.GlamourerIpcProvider
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;

#nullable enable
namespace Ktisis.Interop.Ipc;

public class GlamourerIpcProvider
{
  private readonly Glamourer.Api.IpcSubscribers.ApplyState _applyState;

  public GlamourerIpcProvider(IDalamudPluginInterface dpi)
  {
    this._applyState = new Glamourer.Api.IpcSubscribers.ApplyState(dpi);
  }

  public void ApplyState(string state, int index)
  {
    int num = (int) this._applyState.Invoke(state, index);
  }
}
