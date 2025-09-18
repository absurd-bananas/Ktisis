// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.HookModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Hooking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable
namespace Ktisis.Interop.Hooking;

public abstract class HookModule : IHookModule, IDisposable
{
  private readonly IHookMediator _hook;
  private readonly List<IHookWrapper> Hooks = new List<IHookWrapper>();
  private bool _init;
  private bool IsDisposed;

  public bool IsInit => this._init && !this.IsDisposed;

  protected HookModule(IHookMediator hook) => this._hook = hook;

  public virtual void EnableAll()
  {
    this.Hooks.ForEach((Action<IHookWrapper>) (hook => hook.Enable()));
  }

  public virtual void DisableAll()
  {
    this.Hooks.ForEach((Action<IHookWrapper>) (hook => hook.Disable()));
  }

  public void SetEnabled(bool enabled)
  {
    if (enabled)
      this.EnableAll();
    else
      this.DisableAll();
  }

  public bool TryGetHook<T>(out HookWrapper<T>? result) where T : Delegate
  {
    result = (HookWrapper<T>) null;
    foreach (IHookWrapper hook in this.Hooks)
    {
      if (hook is HookWrapper<T> hookWrapper)
      {
        result = hookWrapper;
        return true;
      }
    }
    return false;
  }

  public virtual bool Initialize()
  {
    if (this.IsDisposed)
      throw new Exception("Attempted to initialize disposed module.");
    bool flag = this._hook.Init(this);
    List<IHookWrapper> list = this.GetHookWrappers().ToList<IHookWrapper>();
    if (flag)
    {
      this.Hooks.AddRange((IEnumerable<IHookWrapper>) list);
      flag &= this.OnInitialize();
    }
    if (!flag)
      this.Dispose();
    return this._init = flag;
  }

  protected virtual bool OnInitialize() => true;

  private IEnumerable<IHookWrapper> GetHookWrappers()
  {
    HookModule hookModule = this;
    FieldInfo[] fieldInfoArray = hookModule.GetType().GetFields((BindingFlags) 52);
    for (int index = 0; index < fieldInfoArray.Length; ++index)
    {
      FieldInfo field = fieldInfoArray[index];
      IHookWrapper hookFromField;
      try
      {
        hookFromField = hookModule.GetHookFromField(field);
      }
      catch (Exception ex)
      {
        Ktisis.Ktisis.Log.Error($"Failed to resolve hook for field '{((MemberInfo) field).Name}':\n{ex}", Array.Empty<object>());
        continue;
      }
      if (hookFromField != null)
        yield return hookFromField;
    }
    fieldInfoArray = (FieldInfo[]) null;
  }

  private IHookWrapper? GetHookFromField(FieldInfo field)
  {
    Type fieldType = field.FieldType;
    if (!fieldType.IsGenericType || fieldType.GetGenericTypeDefinition() != typeof (Hook<>))
      return (IHookWrapper) null;
    object obj = field.GetValue((object) this);
    if (obj == null)
      return (IHookWrapper) null;
    return (IHookWrapper) Activator.CreateInstance(typeof (HookWrapper<>).GetGenericTypeDefinition().MakeGenericType(fieldType.GenericTypeArguments), obj);
  }

  public virtual void Dispose()
  {
    if (this.IsDisposed)
      return;
    try
    {
      this.Hooks.ForEach((Action<IHookWrapper>) (hook => hook.Dispose()));
      this.Hooks.Clear();
      this._hook.Remove(this);
    }
    finally
    {
      this.IsDisposed = true;
      GC.SuppressFinalize((object) this);
    }
  }
}
