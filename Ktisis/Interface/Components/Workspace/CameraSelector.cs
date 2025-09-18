// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Workspace.CameraSelector
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using GLib.Widgets;
using Ktisis.Common.Extensions;
using Ktisis.Editor.Camera;
using Ktisis.Editor.Camera.Types;
using Ktisis.Editor.Context.Types;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Components.Workspace;

public class CameraSelector
{
  private readonly IEditorContext _ctx;
  private bool _isOpen;
  private float _lastScroll;

  private ICameraManager Cameras => this._ctx.Cameras;

  public CameraSelector(IEditorContext ctx) => this._ctx = ctx;

  public void Draw()
  {
    using (ImRaii.PushId(ImU8String.op_Implicit("##CameraSelect"), true))
    {
      ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
      float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
      Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X - (float) (((double) Buttons.CalcSize() + (double) x) * 3.0));
      this.DrawSelector();
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
      if (Buttons.IconButtonTooltip((FontAwesomeIcon) 61543, "Create new camera"))
        this.Cameras.Create();
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
      if (Buttons.IconButtonTooltip((FontAwesomeIcon) 62211, "Edit camera"))
        this._ctx.Interface.OpenCameraWindow();
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
      this.DrawFreecamToggle();
    }
  }

  private void DrawFreecamToggle()
  {
    bool workCameraActive = this.Cameras.IsWorkCameraActive;
    using (ImRaii.PushColor((ImGuiCol) 21, Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol) 23), workCameraActive))
    {
      using (ImRaii.PushColor((ImGuiCol) 0, Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol) 0).SetAlpha((byte) 207), !workCameraActive))
      {
        if (!Buttons.IconButtonTooltip((FontAwesomeIcon) 61488, "Toggle work camera"))
          return;
        this.Cameras.ToggleWorkCameraMode();
      }
    }
  }

  private void DrawSelector()
  {
    using (ImRaii.Disabled(this.Cameras.IsWorkCameraActive))
    {
      EditorCamera current = this.Cameras.Current;
      bool flag = Dalamud.Bindings.ImGui.ImGui.BeginCombo(ImU8String.op_Implicit("##CameraSelectList"), ImU8String.op_Implicit(current?.Name ?? "INVALID"), (ImGuiComboFlags) 0);
      if (flag)
      {
        if (!this._isOpen && (double) this._lastScroll > 0.0)
          Dalamud.Bindings.ImGui.ImGui.SetScrollY(this._lastScroll);
        foreach (EditorCamera camera in this.Cameras.GetCameras())
        {
          if (Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(camera.Name), camera == current, (ImGuiSelectableFlags) 0, new Vector2()))
            this.Cameras.SetCurrent(camera);
        }
        this._lastScroll = Dalamud.Bindings.ImGui.ImGui.GetScrollY();
        Dalamud.Bindings.ImGui.ImGui.EndCombo();
      }
      this._isOpen = flag;
    }
  }
}
