// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.History.UndoAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;

namespace Ktisis.Actions.Handlers.History;

[Action("History_Undo")]
public class UndoAction(IPluginContext ctx) : KeyAction(ctx) {
	public override KeybindInfo BindInfo { get; } = new KeybindInfo {
		Trigger = KeybindTrigger.OnDown,
		Default = new ActionKeybind {
			Enabled = true,
			Combo = new KeyCombo((VirtualKey)90, (VirtualKey)17)
		}
	};

	public override bool CanInvoke() {
		var editor = this.Context.Editor;
		if (editor != null) {
			var actions = editor.Actions;
			if (actions != null) {
				var history = actions.History;
				if (history != null)
					return history.CanUndo;
			}
		}
		return false;
	}

	public override bool Invoke() {
		if (!this.CanInvoke())
			return false;
		this.Context.Editor.Actions.History.Undo();
		return true;
	}
}
