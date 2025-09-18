// Decompiled with JetBrains decompiler
// Type: Ktisis.Events.EventBase`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Events;

public abstract class EventBase<T> : IDisposable where T : Delegate
{
  protected readonly HashSet<object> _subscribers = new HashSet<object>();

  public bool Add(T subscriber)
  {
    lock (this._subscribers)
      return this._subscribers.Add((object) subscriber);
  }

  public bool Remove(T subscriber)
  {
    lock (this._subscribers)
      return this._subscribers.Remove((object) subscriber);
  }

  public void Dispose() => this._subscribers.Clear();
}
