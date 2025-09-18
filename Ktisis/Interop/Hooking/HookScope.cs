// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.HookScope
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Interop.Hooking;

public class HookScope : IHookModule, IDisposable
{
  private readonly IHookMediator _hook;
  private readonly List<HookModule> Modules = new List<HookModule>();
  private bool _init;

  public HookScope(IHookMediator hook) => this._hook = hook;

  public bool IsInit => this._init;

  public void EnableAll() => this.Modules.ForEach((Action<HookModule>) (mod => mod.EnableAll()));

  public void DisableAll() => this.Modules.ForEach((Action<HookModule>) (mod => mod.DisableAll()));

  public void SetEnabled(bool enabled)
  {
    if (enabled)
      this.EnableAll();
    else
      this.DisableAll();
  }

  public bool TryGetHook<T>(out HookWrapper<T>? result) where T : Delegate
  {
    foreach (HookModule module in this.Modules)
    {
      HookWrapper<T> result1;
      if (module.TryGetHook<T>(out result1))
      {
        result = result1;
        return true;
      }
    }
    result = (HookWrapper<T>) null;
    return false;
  }

  public T Create<T>(params object[] param) where T : HookModule
  {
    T obj = this._hook.Create<T>(param);
    this.Modules.Add((HookModule) obj);
    return obj;
  }

  public bool Initialize()
  {
    bool flag = false;
    foreach (HookModule module in this.Modules)
      flag |= module.Initialize();
    return this._init = flag;
  }

  public void Dispose()
  {
    this.Modules.ForEach((Action<HookModule>) (mod => mod.Dispose()));
    this.Modules.Clear();
  }
}
