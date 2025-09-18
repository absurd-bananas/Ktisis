// Decompiled with JetBrains decompiler
// Type: Ktisis.Ktisis
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Ktisis.Core;
using Ktisis.ImGuizmo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

#nullable enable
namespace Ktisis;

public sealed class Ktisis : IDalamudPlugin, IDisposable
{
  private readonly ServiceProvider _services;

  public static IPluginLog Log { get; private set; }

  public Ktisis(IPluginLog logger, IDalamudPluginInterface dpi)
  {
    Ktisis.Ktisis.Log = logger;
    this._services = new ServiceComposer().AddFromAttributes().AddDalamudServices(dpi).AddSingleton<IPluginLog>(logger).BuildProvider();
    this._services.GetRequiredService<PluginContext>().Initialize();
  }

  public static string GetVersion() => Assembly.GetCallingAssembly().GetName().Version.ToString(3);

  public void Dispose()
  {
    try
    {
      this._services.Dispose();
      Gizmo.Unload();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Error occurred during disposal:\n{ex}", Array.Empty<object>());
    }
  }
}
