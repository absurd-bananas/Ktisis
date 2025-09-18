// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Actions.InputConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

namespace Ktisis.Data.Config.Actions;

public class InputConfig {
	public bool Enabled;
	public Dictionary<string, ActionKeybind> Keybinds = new Dictionary<string, ActionKeybind>();

	public ActionKeybind GetOrSetDefault(string name, ActionKeybind defaultValue) {
		ActionKeybind orSetDefault;
		if (this.Keybinds.TryGetValue(name, out orSetDefault))
			return orSetDefault;
		this.Keybinds.Add(name, defaultValue);
		return defaultValue;
	}
}
