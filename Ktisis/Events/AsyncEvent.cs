// Decompiled with JetBrains decompiler
// Type: Ktisis.Events.AsyncEvent`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Core.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Events;

[Transient]
public class AsyncEvent<T> : EventBase<T> where T : Delegate
{
  public Task InvokeAsync() => this.RunInvoke((Func<object, Task>) (sub => ((Func<Task>) sub)()));

  public Task InvokeAsync<T1>(T1 a1)
  {
    return this.RunInvoke((Func<object, Task>) (sub => ((Func<T1, Task>) sub)(a1)));
  }

  public Task InvokeAsync<T1, T2>(T1 a1, T2 a2)
  {
    return this.RunInvoke((Func<object, Task>) (sub => ((Func<T1, T2, Task>) sub)(a1, a2)));
  }

  public Task InvokeAsync<T1, T2, T3>(T1 a1, T2 a2, T3 a3)
  {
    return this.RunInvoke((Func<object, Task>) (sub => ((Func<T1, T2, T3, Task>) sub)(a1, a2, a3)));
  }

  private async Task RunInvoke(Func<object, Task> selector)
  {
    AsyncEvent<T> asyncEvent = this;
    await Task.WhenAll(asyncEvent._subscribers.Select<object, Task>(selector)).ContinueWith(new Action<Task>(asyncEvent.LogException));
  }

  private void LogException(Task _task)
  {
    if (_task.Exception == null)
      return;
    Ktisis.Ktisis.Log.Error(_task.Exception.ToString(), Array.Empty<object>());
  }
}
