// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Actions.ActionManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Actions.Types;
using Ktisis.Editor.Actions.Input;
using Ktisis.Editor.Context.Types;

namespace Ktisis.Editor.Actions;

public class ActionManager : IActionManager, IDisposable {
	private readonly IEditorContext _ctx;

	public ActionManager(IEditorContext ctx, IInputManager input) {
		this._ctx = ctx;
		this.Input = input;
		this.History = new HistoryManager();
	}

	public IInputManager Input { get; }

	public IHistoryManager History { get; }

	public void Initialize() {
		Ktisis.Ktisis.Log.Verbose("Initializing input manager...", Array.Empty<object>());
		try {
			this.Input.Initialize();
			this.RegisterKeybinds();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to initialize input manager:\n{ex}", Array.Empty<object>());
		}
	}

	public void Dispose() {
		try {
			this.History.Clear();
			this.Input.Dispose();
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to dispose action manager:\n{ex}", Array.Empty<object>());
		} finally {
			GC.SuppressFinalize(this);
		}
	}

	private void RegisterKeybinds() {
		foreach (var action in this._ctx.Plugin.Actions.GetBindable())
			this.RegisterKeybind(action);
	}

	private void RegisterKeybind(KeyAction action) {
		this.Input.Register(action.GetKeybind(), action.Invoke, action.BindInfo.Trigger);
	}
}
