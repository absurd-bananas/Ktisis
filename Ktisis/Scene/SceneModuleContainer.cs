// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.SceneModuleContainer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Interop.Hooking;
using Ktisis.Scene.Modules;

namespace Ktisis.Scene;

public class SceneModuleContainer {
	private readonly HookScope _scope;
	private readonly Dictionary<Type, SceneModule> Modules = new Dictionary<Type, SceneModule>();

	public SceneModuleContainer(HookScope scope) {
		this._scope = scope;
	}

	public T GetModule<T>() where T : SceneModule => (T)this.Modules[typeof(T)];

	public bool TryGetModule<T>(out T? module) where T : SceneModule {
		module = default;
		SceneModule sceneModule;
		var num = this.Modules.TryGetValue(typeof(T), out sceneModule) ? 1 : 0;
		if (num == 0)
			return num != 0;
		module = sceneModule as T;
		return num != 0;
	}

	protected T AddModule<T>(params object[] param) where T : SceneModule {
		var obj = this._scope.Create<T>(param.Prepend(this).ToArray());
		this.Modules.Add(typeof(T), obj);
		return obj;
	}

	protected void InitializeModules() {
		foreach (var sceneModule in this.Modules.Values.Where(module => module.Initialize() && module.IsInit)) {
			try {
				sceneModule.Setup();
			} catch (Exception ex) {
				Ktisis.Ktisis.Log.Error($"Failed to setup module '{sceneModule.GetType().Name}':\n{ex}", Array.Empty<object>());
			}
		}
	}

	protected void UpdateModules() {
		foreach (var module in this.Modules) {
			Type type1;
			SceneModule sceneModule1;
			module.Deconstruct(ref type1, ref sceneModule1);
			var type2 = type1;
			var sceneModule2 = sceneModule1;
			try {
				sceneModule2.Update();
			} catch (Exception ex) {
				Ktisis.Ktisis.Log.Error($"Failed to handle update for module '{type2.Name}':\n{ex}", Array.Empty<object>());
			}
		}
	}

	protected void DisposeModules() {
		foreach (HookModule hookModule in this.Modules.Values)
			hookModule.Dispose();
		this.Modules.Clear();
	}
}
