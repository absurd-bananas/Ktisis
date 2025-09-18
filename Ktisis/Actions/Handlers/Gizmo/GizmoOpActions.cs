// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.Gizmo.GizmoOpAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.ImGuizmo;

namespace Ktisis.Actions.Handlers.Gizmo;

public abstract class GizmoOpAction(IPluginContext ctx) : KeyAction(ctx) {
	protected abstract Operation TargetOp { get; init; }

	public override bool Invoke() {
		if (this.Context.Editor == null || this.Context.Editor.Selection.Count == 0)
			return false;
		this.Context.Config.File.Gizmo.Operation = this.TargetOp;
		return true;
	}
}
