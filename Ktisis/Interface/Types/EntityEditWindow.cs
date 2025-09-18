// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Types.EntityEditWindow`1
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Linq;

using Ktisis.Editor.Context.Types;
using Ktisis.Scene.Entities;

namespace Ktisis.Interface.Types;

public abstract class EntityEditWindow<T> : KtisisWindow where T : SceneEntity {
	protected readonly IEditorContext Context;

	protected EntityEditWindow(string name, IEditorContext ctx, ImGuiWindowFlags flags = 0)
		: base(name, flags) {
		this.Context = ctx;
	}

	protected T Target { get; private set; }

	public virtual void PreDraw() {
		if (this.Context.IsValid) {
			var target = this.Target;
			if (target != null && target.IsValid)
				return;
		}
		Ktisis.Ktisis.Log.Verbose($"State for {this.GetType().Name} is stale, closing...", Array.Empty<object>());
		this.Close();
	}

	public virtual void SetTarget(T target) {
		this.Target = target.IsValid ? target : throw new Exception("Attempted to set invalid target.");
	}

	protected void UpdateTarget() {
		if (this.Context.Config.Editor.UseLegacyWindowBehavior)
			return;
		var target = (T)this.Context.Selection.GetSelected().FirstOrDefault(entity => entity is T);
		if (target == null || this.Target == target)
			return;
		this.SetTarget(target);
	}
}
