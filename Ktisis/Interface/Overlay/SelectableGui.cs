// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.SelectableGui
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Math;
using GLib.Widgets;
using Ktisis.Common.Extensions;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Entity;
using Ktisis.Scene.Entities;
using Ktisis.Services.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Overlay;

[Transient]
public class SelectableGui
{
  private readonly ConfigManager _cfg;
  private int ScrollIndex;
  private const int HoverPadding = 6;

  private Configuration Config => this._cfg.File;

  public SelectableGui(ConfigManager cfg) => this._cfg = cfg;

  public ISelectableFrame BeginFrame() => (ISelectableFrame) new SelectableGui.SelectableFrame();

  public bool Draw(ISelectableFrame frame, out SceneEntity? clicked, bool gizmo)
  {
    clicked = (SceneEntity) null;
    if (!this.Config.Overlay.DrawDotsGizmo && Ktisis.ImGuizmo.Gizmo.IsUsing)
      return false;
    ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
    List<IItemSelect> list = frame.GetItems().ToList<IItemSelect>();
    bool flag1 = false;
    foreach (IItemSelect itemSelect1 in list)
    {
      if (!itemSelect1.Entity.IsSelected && (double) itemSelect1.OpacityMultiplier != 0.0)
      {
        EntityDisplay entityDisplay = this.Config.GetEntityDisplay(itemSelect1.Entity);
        bool isSelected = itemSelect1.Entity.IsSelected;
        IItemSelect itemSelect2 = itemSelect1;
        bool flag2;
        switch (entityDisplay.Mode)
        {
          case DisplayMode.Dot:
            flag2 = this.DrawPrimDot(windowDrawList, itemSelect1.ScreenPos, entityDisplay, isSelected, itemSelect1.OpacityMultiplier);
            break;
          case DisplayMode.Icon:
            flag2 = this.DrawIconDot(windowDrawList, itemSelect1.ScreenPos, entityDisplay, isSelected);
            break;
          default:
            flag2 = false;
            break;
        }
        itemSelect2.IsHovered = flag2;
        flag1 |= itemSelect1.IsHovered;
      }
    }
    foreach (IItemSelect itemSelect3 in list)
    {
      if (itemSelect3.Entity.IsSelected && (double) itemSelect3.OpacityMultiplier != 0.0)
      {
        EntityDisplay entityDisplay = this.Config.GetEntityDisplay(itemSelect3.Entity);
        bool isSelected = itemSelect3.Entity.IsSelected;
        IItemSelect itemSelect4 = itemSelect3;
        bool flag3;
        switch (entityDisplay.Mode)
        {
          case DisplayMode.Dot:
            flag3 = this.DrawPrimDot(windowDrawList, itemSelect3.ScreenPos, entityDisplay, isSelected, itemSelect3.OpacityMultiplier);
            break;
          case DisplayMode.Icon:
            flag3 = this.DrawIconDot(windowDrawList, itemSelect3.ScreenPos, entityDisplay, isSelected);
            break;
          default:
            flag3 = false;
            break;
        }
        itemSelect4.IsHovered = flag3;
        flag1 |= itemSelect3.IsHovered;
      }
    }
    if (!flag1)
      return false;
    list.RemoveAll((Predicate<IItemSelect>) (item => !item.IsHovered));
    return this.DrawSelectWindow((IReadOnlyList<IItemSelect>) list, out clicked, gizmo);
  }

  private bool DrawSelectWindow(
    IReadOnlyList<IItemSelect> items,
    out SceneEntity? clicked,
    bool gizmo)
  {
    clicked = (SceneEntity) null;
    if (items.Count == 0 || gizmo && (Ktisis.ImGuizmo.Gizmo.IsUsing || Ktisis.ImGuizmo.Gizmo.IsOver))
      return false;
    bool flag = false;
    try
    {
      Dalamud.Bindings.ImGui.ImGui.SetNextWindowPos(Dalamud.Bindings.ImGui.ImGui.GetMousePos().AddX(20f));
      Dalamud.Bindings.ImGui.ImGui.SetNextWindowSize(-Vector2.One, (ImGuiCond) 1);
      flag = Dalamud.Bindings.ImGui.ImGui.Begin(ImU8String.op_Implicit("##Hover"), (ImGuiWindowFlags) 4139);
      if (flag)
        return this.DrawSelectList(items, out clicked);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Error drawing select list:\n{ex}", Array.Empty<object>());
    }
    finally
    {
      if (flag)
        Dalamud.Bindings.ImGui.ImGui.End();
    }
    return false;
  }

  private bool DrawSelectList(IReadOnlyList<IItemSelect> list, out SceneEntity? clicked)
  {
    clicked = (SceneEntity) null;
    int scrollIndex = this.ScrollIndex;
    ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
    int num = (int) ((ImGuiIOPtr) ref io).MouseWheel;
    this.ScrollIndex = scrollIndex - num;
    if (this.ScrollIndex >= list.Count)
      this.ScrollIndex = 0;
    else if (this.ScrollIndex < 0)
      this.ScrollIndex = list.Count - 1;
    Dalamud.Bindings.ImGui.ImGui.SetNextFrameWantCaptureMouse(true);
    bool flag1 = Dalamud.Bindings.ImGui.ImGui.IsMouseReleased((ImGuiMouseButton) 0);
    for (int index = 0; index < list.Count; ++index)
    {
      IItemSelect itemSelect = list[index];
      bool flag2 = index == this.ScrollIndex;
      Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(itemSelect.Name), flag2, (ImGuiSelectableFlags) 0, new Vector2());
      if (flag2 & flag1)
        clicked = itemSelect.Entity;
    }
    return clicked != null;
  }

  private bool DrawPrimDot(
    ImDrawListPtr drawList,
    Vector2 pos2d,
    EntityDisplay display,
    bool isSelect = false,
    float opacityMultiplier = 1f)
  {
    float radius = isSelect ? this.Config.Overlay.DotRadiusSelected : this.Config.Overlay.DotRadius;
    ((ImDrawListPtr) ref drawList).AddCircleFilled(pos2d, radius, this.AdjustDotAlpha(isSelect ? this.Config.Overlay.DotColorSelected : this.Config.Overlay.DotColor, opacityMultiplier), 16 /*0x10*/);
    ((ImDrawListPtr) ref drawList).AddCircle(pos2d, radius, this.AdjustDotAlpha(isSelect ? this.Config.Overlay.DotOutlineColorSelected : this.Config.Overlay.DotOutlineColor, opacityMultiplier), 16 /*0x10*/, isSelect ? this.Config.Overlay.DotOutlineSelected : this.Config.Overlay.DotOutline);
    return SelectableGui.IsHovering(pos2d, radius);
  }

  private uint AdjustDotAlpha(uint color, float opacityMultiplier)
  {
    float num = Ktisis.ImGuizmo.Gizmo.IsUsing ? this.Config.Overlay.DotOpacityUsing : this.Config.Overlay.DotOpacity;
    return color.SetAlpha(System.Math.Clamp(num * opacityMultiplier, 0.0f, 1f));
  }

  private bool DrawIconDot(
    ImDrawListPtr drawList,
    Vector2 pos2d,
    EntityDisplay display,
    bool isSelect = false)
  {
    Vector2 vector2 = Icons.CalcIconSize(display.Icon);
    ImFontPtr iconFont = UiBuilder.IconFont;
    float radius = ((ImFontPtr) ref iconFont).FontSize;
    bool flag = SelectableGui.IsHovering(pos2d, radius);
    ((ImDrawListPtr) ref drawList).AddCircleFilled(pos2d, radius, isSelect ? 2936012800U /*0xAF000000*/ : (flag ? 3388997632U /*0xCA000000*/ : 1879048192U /*0x70000000*/), 16 /*0x10*/);
    if (isSelect)
      ((ImDrawListPtr) ref drawList).AddCircle(pos2d, radius, 4293914607U, 16 /*0x10*/, 1.5f);
    Dalamud.Bindings.ImGui.ImGui.SetCursorPos(pos2d - vector2 / 2f);
    Icons.DrawIcon(display.Icon, new uint?(display.Color));
    return flag;
  }

  private static bool IsHovering(Vector2 pos2d, float radius)
  {
    return Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(pos2d.Add((float) (-(double) radius - 6.0)), pos2d.Add(radius + 6f));
  }

  private class SelectableFrame : ISelectableFrame
  {
    private readonly List<SelectableGui.ItemSelect> Items = new List<SelectableGui.ItemSelect>();

    public IEnumerable<IItemSelect> GetItems()
    {
      return (IEnumerable<IItemSelect>) this.Items.AsReadOnly();
    }

    public unsafe void AddItem(SceneEntity entity, Vector3 worldPos, float opacityMultiplier)
    {
      Camera* sceneCamera = CameraService.GetSceneCamera();
      Vector2 screenPos;
      if ((IntPtr) sceneCamera == IntPtr.Zero || !CameraService.WorldToScreen(sceneCamera, worldPos, out screenPos))
        return;
      float dist = Vector3.Distance(Vector3.op_Implicit(sceneCamera->Object.Position), worldPos);
      this.Items.Add(new SelectableGui.ItemSelect(entity, screenPos, dist, opacityMultiplier));
    }
  }

  private class ItemSelect : IItemSelect
  {
    public readonly int SortPriority;

    public string Name => this.Entity.Name;

    public SceneEntity Entity { get; }

    public Vector2 ScreenPos { get; }

    public float Distance { get; }

    public bool IsHovered { get; set; }

    public float OpacityMultiplier { get; set; }

    public ItemSelect(SceneEntity entity, Vector2 screenPos, float dist, float opacityMultiplier)
    {
      this.Entity = entity;
      this.ScreenPos = screenPos;
      this.Distance = dist;
      this.OpacityMultiplier = opacityMultiplier;
    }
  }
}
