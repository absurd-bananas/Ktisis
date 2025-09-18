// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.HookWrapper`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

namespace Ktisis.Interop.Hooking;

public class HookWrapper<T> : IHookWrapper, IDalamudHook, IDisposable where T : Delegate {
	private readonly Hook<T> _hook;

	public HookWrapper(Hook<T> hook) {
		this._hook = hook;
		this.Name = this.GetType().GetGenericArguments()[0].Name;
	}

	public IntPtr Address => this._hook.Address;

	public bool IsEnabled => this._hook.IsEnabled;

	public bool IsDisposed => this._hook.IsDisposed;

	public string BackendName => this._hook.BackendName;

	public string Name { get; }

	public void Enable() => this._hook.Enable();

	public void Disable() => this._hook.Disable();

	public void Dispose() {
		Ktisis.Ktisis.Log.Debug($"Disposing hook: '{this.Name}'", Array.Empty<object>());
		if (this._hook.IsEnabled)
			this._hook.Disable();
		this._hook.Dispose();
		GC.SuppressFinalize(this);
	}
}
