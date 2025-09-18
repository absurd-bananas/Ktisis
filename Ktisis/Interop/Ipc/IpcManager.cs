// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Ipc.IpcManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;
using Ktisis.Core.Attributes;
using System;
using System.Linq;

#nullable enable
namespace Ktisis.Interop.Ipc;

[Singleton]
public class IpcManager
{
  private readonly IDalamudPluginInterface _dpi;

  public IpcManager(IDalamudPluginInterface dpi) => this._dpi = dpi;

  public bool IsAnyMcdfActive
  {
    get => this.IsPenumbraActive || this.IsCustomizeActive || this.IsGlamourerActive;
  }

  public bool IsPenumbraActive => this.GetPluginInstalled("Penumbra");

  public PenumbraIpcProvider GetPenumbraIpc() => new PenumbraIpcProvider(this._dpi);

  public bool IsCustomizeActive => this.GetPluginInstalled("CustomizePlus");

  public CustomizeIpcProvider GetCustomizeIpc() => new CustomizeIpcProvider(this._dpi);

  public bool IsGlamourerActive => this.GetPluginInstalled("Glamourer");

  public GlamourerIpcProvider GetGlamourerIpc() => new GlamourerIpcProvider(this._dpi);

  private bool GetPluginInstalled(string internalName)
  {
    return this._dpi.InstalledPlugins.Any<IExposedPlugin>((Func<IExposedPlugin, bool>) (p =>
    {
      if (!p.IsLoaded)
        return false;
      return p.InternalName == internalName || p.Name == internalName;
    }));
  }
}
