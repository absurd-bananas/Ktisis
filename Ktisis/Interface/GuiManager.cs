// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.GuiManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using GLib.Popups;
using GLib.Popups.ImFileDialog;

using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Interface.Types;
using Ktisis.Interface.Windows;
using Ktisis.Localization;

namespace Ktisis.Interface;

[Singleton]
public class GuiManager : IDisposable {
	private readonly DIBuilder _di;
	private readonly PopupManager _popup = new PopupManager();
	private readonly IUiBuilder _uiBuilder;
	private readonly List<KtisisWindow> _windows = new List<KtisisWindow>();
	private readonly WindowSystem _ws = new WindowSystem("Ktisis");
	public readonly FileDialogManager FileDialogs;
	public readonly LocaleManager Locale;

	public GuiManager(
		DIBuilder di,
		IUiBuilder uiBuilder,
		LocaleManager locale,
		FileDialogManager dialogs
	) {
		this._di = di;
		this._uiBuilder = uiBuilder;
		this.Locale = locale;
		this.FileDialogs = dialogs;
	}

	public void Dispose() {
		this._uiBuilder.Draw -= new Action(this.Draw);
		this._uiBuilder.OpenConfigUi -= new Action(this.OnOpenConfigUi);
		this.RemoveAll();
	}

	public void Initialize() {
		this._uiBuilder.DisableGposeUiHide = true;
		this._uiBuilder.Draw += new Action(this.Draw);
		this._uiBuilder.OpenConfigUi += new Action(this.OnOpenConfigUi);
		this.FileDialogs.OnOpenDialog += this.OnOpenDialog;
		this.FileDialogs.Initialize();
	}

	private void Draw() {
		this._ws.Draw();
		this._popup.Draw();
	}

	public T Add<T>(T inst) where T : KtisisWindow {
		this._ws.AddWindow((Window)inst);
		this._windows.Add(inst);
		inst.Closed += this.OnClose;
		Ktisis.Ktisis.Log.Verbose($"Added window: {inst.GetType().Name} ('{inst.WindowName}')", Array.Empty<object>());
		return inst;
	}

	public T? Get<T>() where T : KtisisWindow {
		return (T)this._windows.Find(win => win is T);
	}

	public bool Remove(KtisisWindow inst) {
		var num = this._windows.Remove(inst) ? 1 : 0;
		if (num == 0)
			return num != 0;
		this._ws.RemoveWindow((Window)inst);
		inst.Closed -= this.OnClose;
		if (inst is IDisposable disposable)
			disposable.Dispose();
		Ktisis.Ktisis.Log.Verbose($"Removed window: {inst.GetType().Name} ('{inst.WindowName}')", Array.Empty<object>());
		return num != 0;
	}

	public T Create<T>(params object[] parameters) where T : KtisisWindow {
		var inst = this._di.Create<T>(parameters);
		inst.OnCreate();
		return this.Add<T>(inst);
	}

	public T CreatePopup<T>(params object[] parameters) where T : class, IPopup => this.AddPopupSingleton(this._di.Create<T>(parameters));

	public T GetOrCreate<T>(params object[] parameters) where T : KtisisWindow => this.Get<T>() ?? this.Create<T>(parameters);

	public T AddPopup<T>(T popup) where T : class, IPopup {
		this._popup.Add(popup);
		return popup;
	}

	public T AddPopupSingleton<T>(T popup) where T : class, IPopup {
		var popup1 = this.GetPopup<T>();
		if (popup1 != null)
			this._popup.Remove(popup1);
		return this.AddPopup(popup);
	}

	public T? GetPopup<T>() where T : class, IPopup => this._popup.Get<T>();

	private void OnClose(KtisisWindow window) {
		Ktisis.Ktisis.Log.Verbose($"Window {window.GetType().Name} ('{window.WindowName}') closed, removing...", Array.Empty<object>());
		this.Remove(window);
	}

	private void OnOpenConfigUi() => this.GetOrCreate<ConfigWindow>().Toggle();

	private void OnOpenDialog(FileDialog dialog) {
		foreach (var fileDialog in this._popup.GetAll<FileDialog>()) {
			if (fileDialog.Title == dialog.Title)
				fileDialog.Close();
		}
		this.AddPopup(dialog);
	}

	private void RemoveAll() {
		foreach (var inst in this._windows.ToList())
			this.Remove(inst);
		this._windows.Clear();
	}
}
