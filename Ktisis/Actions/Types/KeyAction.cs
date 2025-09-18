// Decompiled with JetBrains decompiler
// Type: Ktisis.Actions.Types.KeyAction
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Actions.Binds;
using Ktisis.Core.Types;
using Ktisis.Data.Config.Actions;

namespace Ktisis.Actions.Types;

public abstract class KeyAction(IPluginContext ctx) : ActionBase(ctx), IKeybind {
	public abstract KeybindInfo BindInfo { get; }

	public ActionKeybind GetKeybind() => this.Context.Config.File.Keybinds.GetOrSetDefault(this.GetName(), this.BindInfo.Default);
}
