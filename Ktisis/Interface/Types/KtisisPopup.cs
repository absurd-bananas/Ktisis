// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Types.KtisisPopup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using GLib.Popups;
using System;

#nullable enable
namespace Ktisis.Interface.Types;

public abstract class KtisisPopup(string id, ImGuiWindowFlags flags = 0) : IPopup
{
  private bool _isOpen;
  private bool _isOpening;
  private bool _isClosing;

  public bool IsOpen => this._isOpen || this._isOpening;

  public void Open() => this._isOpening = true;

  public void Close() => this._isClosing = true;

  public bool Draw()
  {
    if (this._isOpening)
    {
      this._isOpening = false;
      Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit(id), (ImGuiPopupFlags) 0);
    }
    this._isOpen = Dalamud.Bindings.ImGui.ImGui.IsPopupOpen(ImU8String.op_Implicit(id), (ImGuiPopupFlags) 0) && !this._isClosing;
    if (!this._isOpen)
      return false;
    using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit(id), flags))
    {
      if (!iendObject.Success)
        return false;
      try
      {
        this.OnDraw();
      }
      catch (Exception ex)
      {
        Ktisis.Ktisis.Log.Error($"Error drawing popup:\n{ex}", Array.Empty<object>());
      }
      return true;
    }
  }

  protected abstract void OnDraw();
}
