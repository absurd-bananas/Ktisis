// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.SceneModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Interop.Hooking;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Modules;

public abstract class SceneModule : HookModule {
	protected readonly ISceneManager Scene;

	public SceneModule(IHookMediator hook, ISceneManager scene)
		: base(hook) {
		this.Scene = scene;
	}

	protected bool CheckValid() {
		var num = this.Scene.IsValid ? 1 : 0;
		if (num != 0)
			return num != 0;
		this.DisableAll();
		Ktisis.Ktisis.Log.Warning($"Hook called from '{this.GetType().Name}' with invalid scene state, disabling.", Array.Empty<object>());
		return num != 0;
	}

	public virtual void Setup() { }

	public virtual void Update() { }
}
