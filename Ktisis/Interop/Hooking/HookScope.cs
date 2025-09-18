// Decompiled with JetBrains decompiler
// Type: Ktisis.Interop.Hooking.HookScope
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

namespace Ktisis.Interop.Hooking;

public class HookScope : IHookModule, IDisposable {
	private readonly IHookMediator _hook;
	private readonly List<HookModule> Modules = new List<HookModule>();

	public HookScope(IHookMediator hook) {
		this._hook = hook;
	}

	public bool IsInit { get; private set; }

	public void EnableAll() => this.Modules.ForEach(mod => mod.EnableAll());

	public void DisableAll() => this.Modules.ForEach(mod => mod.DisableAll());

	public void SetEnabled(bool enabled) {
		if (enabled)
			this.EnableAll();
		else
			this.DisableAll();
	}

	public bool TryGetHook<T>(out HookWrapper<T>? result) where T : Delegate {
		foreach (var module in this.Modules) {
			HookWrapper<T> result1;
			if (module.TryGetHook(out result1)) {
				result = result1;
				return true;
			}
		}
		result = null;
		return false;
	}

	public bool Initialize() {
		var flag = false;
		foreach (var module in this.Modules)
			flag |= module.Initialize();
		return this.IsInit = flag;
	}

	public void Dispose() {
		this.Modules.ForEach(mod => mod.Dispose());
		this.Modules.Clear();
	}

	public T Create<T>(params object[] param) where T : HookModule {
		var obj = this._hook.Create<T>(param);
		this.Modules.Add(obj);
		return obj;
	}
}
