// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Ipc.MareIpcProvider
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using System;

#nullable enable
namespace Ktisis.Interop.Ipc;

public class MareIpcProvider
{
  private readonly ICallGateSubscriber<string, IGameObject, bool> _mareApplyMcdf;

  public MareIpcProvider(IDalamudPluginInterface dpi)
  {
    this._mareApplyMcdf = dpi.GetIpcSubscriber<string, IGameObject, bool>("MareSynchronos.LoadMcdf");
  }

  public bool LoadMcdfAsync(string fileName, IGameObject target)
  {
    try
    {
      return this._mareApplyMcdf.InvokeFunc(fileName, target);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error(ex, "Failed to Invoke MareSynchronos.LoadMcdfAsync IPC", Array.Empty<object>());
      return false;
    }
  }
}
