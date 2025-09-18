// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Actions.Input.InputManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Actions.Binds;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Actions;
using Ktisis.Editor.Context.Types;
using Ktisis.Interop.Hooking;

namespace Ktisis.Editor.Actions.Input;

public class InputManager : IInputManager, IDisposable {
	private readonly IEditorContext _context;
	private readonly IKeyState _keyState;
	private readonly HookScope _scope;
	private readonly List<KeybindRegister> Keybinds = new List<KeybindRegister>();

	public InputManager(IEditorContext context, HookScope scope, IKeyState keyState) {
		this._context = context;
		this._scope = scope;
		this._keyState = keyState;
	}

	private Configuration Config => this._context.Config;

	private InputModule? Module { get; set; }

	public void Initialize() {
		this.Module = this._scope.Create<InputModule>();
		this.Module.Initialize();
		this.Module.OnKeyEvent += this.OnKeyEvent;
		this.Module.EnableAll();
	}

	public void Register(ActionKeybind keybind, KeyInvokeHandler handler, KeybindTrigger trigger) {
		this.Keybinds.Add(new KeybindRegister(keybind, handler, trigger));
	}

	public void Dispose() {
		try {
			this.Module?.Dispose();
			this.Keybinds.Clear();
			if (this.Module != null)
				this.Module.OnKeyEvent -= this.OnKeyEvent;
		} catch (Exception ex) {
			Ktisis.Ktisis.Log.Error($"Failed to dispose input manager:\n{ex}", Array.Empty<object>());
		}
		GC.SuppressFinalize(this);
	}

	private bool OnKeyEvent(VirtualKey key, VirtualKeyState state) {
		if (!this._context.IsGPosing || !this.Config.Keybinds.Enabled || IsChatInputActive())
			return false;
		KeybindTrigger keybindTrigger;
		switch (state) {
			case VirtualKeyState.Down:
				keybindTrigger = KeybindTrigger.OnDown;
				break;
			case VirtualKeyState.Held:
				keybindTrigger = KeybindTrigger.OnHeld;
				break;
			case VirtualKeyState.Released:
				keybindTrigger = KeybindTrigger.OnRelease;
				break;
			default:
				throw new Exception($"Invalid key state encountered ({state})");
		}
		var trigger = keybindTrigger;
		var activeHotkey = this.GetActiveHotkey(key, trigger);
		return activeHotkey != null && activeHotkey.Handler();
	}

	private KeybindRegister? GetActiveHotkey(VirtualKey key, KeybindTrigger trigger) {
		var activeHotkey = (KeybindRegister)null;
		var num = 0;
		foreach (var keybind in this.Keybinds) {
			var combo = keybind.Keybind.Combo;
			if (keybind.Trigger.HasFlag(trigger) && combo.Key == key && ((IEnumerable<VirtualKey>)combo.Modifiers).All((Func<VirtualKey, bool>)(mod => this._keyState[mod]))) {
				var length = combo.Modifiers.Length;
				if (activeHotkey == null || length >= num) {
					activeHotkey = keybind;
					num = length;
				}
			}
		}
		return activeHotkey;
	}

	public unsafe static bool IsChatInputActive() {
		UIModule* uiModulePtr = UIModule.Instance();
		if ((IntPtr)uiModulePtr == IntPtr.Zero)
			return false;
		RaptureAtkModule* raptureAtkModule = ((UIModule)(IntPtr)uiModulePtr).GetRaptureAtkModule();
		return (IntPtr)raptureAtkModule != IntPtr.Zero && ((AtkModule) ref raptureAtkModule ->AtkModule).IsTextInputActive();
	}

	private class KeybindRegister {
		public readonly KeyInvokeHandler Handler;
		public readonly ActionKeybind Keybind;
		public readonly KeybindTrigger Trigger;

		public KeybindRegister(ActionKeybind keybind, KeyInvokeHandler handler, KeybindTrigger trigger) {
			this.Keybind = keybind;
			this.Handler = handler;
			this.Trigger = trigger;
		}

		public bool Enabled => this.Keybind.Enabled;
	}
}
