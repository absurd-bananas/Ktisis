// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.IHookWrapper
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Hooking;
using System;

#nullable enable
namespace Ktisis.Interop.Hooking;

public interface IHookWrapper : IDalamudHook, IDisposable
{
  string Name { get; }

  void Enable();

  void Disable();
}
