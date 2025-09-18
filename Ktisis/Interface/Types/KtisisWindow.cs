// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Types.KtisisWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using Ktisis.Events;
using System;

#nullable enable
namespace Ktisis.Interface.Types;

public abstract class KtisisWindow : Window
{
  private readonly Event<Action<KtisisWindow>> _closedEvent = new Event<Action<KtisisWindow>>();

  public event KtisisWindow.ClosedDelegate Closed
  {
    add => this._closedEvent.Add(new Action<KtisisWindow>(value.Invoke));
    remove => this._closedEvent.Remove(new Action<KtisisWindow>(value.Invoke));
  }

  protected KtisisWindow(string name, ImGuiWindowFlags flags = 0, bool forceMainWindow = false)
    : base(name, flags, forceMainWindow)
  {
    this.RespectCloseHotkey = false;
  }

  public void Open() => this.IsOpen = true;

  public void Close()
  {
    try
    {
      if (this.IsOpen)
        return;
      base.OnClose();
    }
    finally
    {
      this.IsOpen = false;
    }
  }

  public virtual void OnCreate()
  {
  }

  public virtual void OnClose() => this._closedEvent.Invoke<KtisisWindow>(this);

  public delegate void ClosedDelegate(KtisisWindow window);
}
