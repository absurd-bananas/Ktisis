// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Actions.Input.IInputManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Actions.Binds;
using Ktisis.Data.Config.Actions;

namespace Ktisis.Editor.Actions.Input;

public interface IInputManager : IDisposable {
	void Initialize();

	void Register(ActionKeybind keybind, KeyInvokeHandler handler, KeybindTrigger trigger);
}
