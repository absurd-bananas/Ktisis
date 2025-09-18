// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Handlers.Output.ToggleHiResAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Attributes;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;

using KtisisPyon.Common.Utility;

namespace Ktisis.Actions.Handlers.Output;

[Action("Output_HiRes_Toggle")]
public class ToggleHiResAction(IPluginContext ctx) : KeyAction(ctx) {
	public override KeybindInfo BindInfo { get; } = new KeybindInfo {
		Trigger = KeybindTrigger.OnRelease,
		Default = new ActionKeybind {
			Enabled = true,
			Combo = new KeyCombo((VirtualKey)120)
		}
	};

	public override bool CanInvoke() => this.Context.Editor != null;

	public override bool Invoke() {
		if (!this.CanInvoke())
			return false;
		var pyon = ctx.Config.File.Pyon;
		if (pyon.DefaultSize == Size.Empty || pyon.HiResSize == Size.Empty)
			return false;
		Win32.SetWinRes(pyon);
		return true;
	}
}
