// Decompiled with JetBrains decompiler
// Type: Ktisis.Events.Event`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Core.Attributes;
using System;

#nullable enable
namespace Ktisis.Events;

[Transient]
public class Event<T> : EventBase<T> where T : Delegate
{
  private void Enumerate(Action<object> func)
  {
    foreach (object subscriber in this._subscribers)
    {
      try
      {
        func(subscriber);
      }
      catch (Exception ex)
      {
        Ktisis.Ktisis.Log.Error(ex.ToString(), Array.Empty<object>());
      }
    }
  }

  public void Invoke() => this.Enumerate((Action<object>) (sub => ((Action) sub)()));

  public void Invoke<T1>(T1 a1) => this.Enumerate((Action<object>) (sub => ((Action<T1>) sub)(a1)));

  public void Invoke<T1, T2>(T1 a1, T2 a2)
  {
    this.Enumerate((Action<object>) (sub => ((Action<T1, T2>) sub)(a1, a2)));
  }

  public void Invoke<T1, T2, T3>(T1 a1, T2 a2, T3 a3)
  {
    this.Enumerate((Action<object>) (sub => ((Action<T1, T2, T3>) sub)(a1, a2, a3)));
  }
}
