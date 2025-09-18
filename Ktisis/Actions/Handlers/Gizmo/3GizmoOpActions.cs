// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.Gizmo.OpScaleAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;
using Ktisis.ImGuizmo;

namespace Ktisis.Actions.Handlers.Gizmo;

[Action("Gizmo_SetScaleMode")]
public class OpScaleAction(IPluginContext ctx) : GizmoOpAction(ctx) {
	protected override Operation TargetOp { get; init; } = Operation.SCALE;

	public override KeybindInfo BindInfo { get; } = new KeybindInfo {
		Trigger = KeybindTrigger.OnDown,
		Default = new ActionKeybind {
			Enabled = true,
			Combo = new KeyCombo((VirtualKey)83, (VirtualKey)17)
		}
	};
}
