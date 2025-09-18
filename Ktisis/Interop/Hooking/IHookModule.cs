// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.IHookModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System;

#nullable enable
namespace Ktisis.Interop.Hooking;

public interface IHookModule : IDisposable
{
  bool IsInit { get; }

  void EnableAll();

  void DisableAll();

  void SetEnabled(bool enabled);

  bool TryGetHook<T>(out HookWrapper<T>? result) where T : Delegate;

  bool Initialize();
}
