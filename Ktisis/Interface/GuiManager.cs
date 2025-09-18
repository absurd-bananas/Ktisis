// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.GuiManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using GLib.Popups;
using GLib.Popups.ImFileDialog;
using Ktisis.Core;
using Ktisis.Core.Attributes;
using Ktisis.Interface.Types;
using Ktisis.Interface.Windows;
using Ktisis.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Interface;

[Singleton]
public class GuiManager : IDisposable
{
  private readonly DIBuilder _di;
  private readonly IUiBuilder _uiBuilder;
  private readonly WindowSystem _ws = new WindowSystem("Ktisis");
  private readonly PopupManager _popup = new PopupManager();
  private readonly List<KtisisWindow> _windows = new List<KtisisWindow>();
  public readonly LocaleManager Locale;
  public readonly FileDialogManager FileDialogs;

  public GuiManager(
    DIBuilder di,
    IUiBuilder uiBuilder,
    LocaleManager locale,
    FileDialogManager dialogs)
  {
    this._di = di;
    this._uiBuilder = uiBuilder;
    this.Locale = locale;
    this.FileDialogs = dialogs;
  }

  public void Initialize()
  {
    this._uiBuilder.DisableGposeUiHide = true;
    this._uiBuilder.Draw += new Action(this.Draw);
    this._uiBuilder.OpenConfigUi += new Action(this.OnOpenConfigUi);
    this.FileDialogs.OnOpenDialog += new Action<FileDialog>(this.OnOpenDialog);
    this.FileDialogs.Initialize();
  }

  private void Draw()
  {
    this._ws.Draw();
    this._popup.Draw();
  }

  public T Add<T>(T inst) where T : KtisisWindow
  {
    this._ws.AddWindow((Window) inst);
    this._windows.Add((KtisisWindow) inst);
    inst.Closed += new KtisisWindow.ClosedDelegate(this.OnClose);
    Ktisis.Ktisis.Log.Verbose($"Added window: {((object) inst).GetType().Name} ('{inst.WindowName}')", Array.Empty<object>());
    return inst;
  }

  public T? Get<T>() where T : KtisisWindow
  {
    return (T) this._windows.Find((Predicate<KtisisWindow>) (win => win is T));
  }

  public bool Remove(KtisisWindow inst)
  {
    int num = this._windows.Remove(inst) ? 1 : 0;
    if (num == 0)
      return num != 0;
    this._ws.RemoveWindow((Window) inst);
    inst.Closed -= new KtisisWindow.ClosedDelegate(this.OnClose);
    if (inst is IDisposable disposable)
      disposable.Dispose();
    Ktisis.Ktisis.Log.Verbose($"Removed window: {((object) inst).GetType().Name} ('{inst.WindowName}')", Array.Empty<object>());
    return num != 0;
  }

  public T Create<T>(params object[] parameters) where T : KtisisWindow
  {
    T inst = this._di.Create<T>(parameters);
    inst.OnCreate();
    return this.Add<T>(inst);
  }

  public T CreatePopup<T>(params object[] parameters) where T : class, IPopup
  {
    return this.AddPopupSingleton<T>(this._di.Create<T>(parameters));
  }

  public T GetOrCreate<T>(params object[] parameters) where T : KtisisWindow
  {
    return this.Get<T>() ?? this.Create<T>(parameters);
  }

  public T AddPopup<T>(T popup) where T : class, IPopup
  {
    this._popup.Add((IPopup) popup);
    return popup;
  }

  public T AddPopupSingleton<T>(T popup) where T : class, IPopup
  {
    T popup1 = this.GetPopup<T>();
    if ((object) popup1 != null)
      this._popup.Remove((IPopup) popup1);
    return this.AddPopup<T>(popup);
  }

  public T? GetPopup<T>() where T : class, IPopup => this._popup.Get<T>();

  private void OnClose(KtisisWindow window)
  {
    Ktisis.Ktisis.Log.Verbose($"Window {((object) window).GetType().Name} ('{window.WindowName}') closed, removing...", Array.Empty<object>());
    this.Remove(window);
  }

  private void OnOpenConfigUi() => this.GetOrCreate<ConfigWindow>().Toggle();

  private void OnOpenDialog(FileDialog dialog)
  {
    foreach (FileDialog fileDialog in this._popup.GetAll<FileDialog>())
    {
      if (fileDialog.Title == dialog.Title)
        fileDialog.Close();
    }
    this.AddPopup<FileDialog>(dialog);
  }

  private void RemoveAll()
  {
    foreach (KtisisWindow inst in this._windows.ToList<KtisisWindow>())
      this.Remove(inst);
    this._windows.Clear();
  }

  public void Dispose()
  {
    this._uiBuilder.Draw -= new Action(this.Draw);
    this._uiBuilder.OpenConfigUi -= new Action(this.OnOpenConfigUi);
    this.RemoveAll();
  }
}
