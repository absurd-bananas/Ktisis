// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Workspace.SceneTree
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using GLib.Widgets;
using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Data.Config.Bones;
using Ktisis.Data.Config.Entity;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Types;
using Ktisis.Editor.Selection;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Components.Workspace;

public class SceneTree
{
  private readonly IEditorContext _ctx;
  private readonly SceneDragDropHandler _dragDrop;
  private float MinY;
  private float MaxY;

  public SceneTree(IEditorContext ctx)
  {
    this._ctx = ctx;
    this._dragDrop = new SceneDragDropHandler(ctx);
  }

  public void Draw(float height)
  {
    bool flag = false;
    try
    {
      flag = Dalamud.Bindings.ImGui.ImGui.BeginChildFrame(Dalamud.Bindings.ImGui.ImGui.GetID(ImU8String.op_Implicit("SceneTree_Frame")), new Vector2(-1f, height));
      if (!flag)
        return;
      this.DrawScene(height);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Error drawing scene tree:\n{ex}", Array.Empty<object>());
    }
    finally
    {
      if (flag)
        Dalamud.Bindings.ImGui.ImGui.EndChildFrame();
    }
  }

  private static float IconSpacing
  {
    get
    {
      ImFontPtr iconFont = UiBuilder.IconFont;
      return ((ImFontPtr) ref iconFont).FontSize;
    }
  }

  private void PreCalc(float height)
  {
    float scrollY = Dalamud.Bindings.ImGui.ImGui.GetScrollY();
    this.MinY = scrollY - Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
    this.MaxY = height + scrollY;
  }

  private void DrawScene(float height)
  {
    this.PreCalc(height);
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    using (ImRaii.PushStyle((ImGuiStyleVar) 13, ((ImGuiStylePtr) ref style).ItemSpacing with
    {
      Y = 5f
    }, true))
      this.IterateTree(this._ctx.Scene.Children);
  }

  private void IterateTree(IEnumerable<SceneEntity> entities)
  {
    try
    {
      Dalamud.Bindings.ImGui.ImGui.TreePush(IntPtr.Zero);
      foreach (SceneEntity entity in entities)
        this.DrawNode(entity);
    }
    finally
    {
      Dalamud.Bindings.ImGui.ImGui.TreePop();
    }
  }

  private void DrawNode(SceneEntity node)
  {
    Vector2 pos = Dalamud.Bindings.ImGui.ImGui.GetCursorPos();
    int num1 = (double) pos.Y <= (double) this.MinY ? 0 : ((double) pos.Y < (double) this.MaxY ? 1 : 0);
    string str = $"##SceneTree_{node.GetHashCode():X}";
    Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(str), node.IsSelected, (ImGuiSelectableFlags) 18, new Vector2());
    bool isHover = Dalamud.Bindings.ImGui.ImGui.IsWindowHovered();
    Vector2 itemRectSize = Dalamud.Bindings.ImGui.ImGui.GetItemRectSize();
    this._dragDrop.Handle(node);
    uint id = Dalamud.Bindings.ImGui.ImGui.GetID(ImU8String.op_Implicit(str));
    ImGuiStoragePtr stateStorage = Dalamud.Bindings.ImGui.ImGui.GetStateStorage();
    bool flag1 = ((ImGuiStoragePtr) ref stateStorage).GetBool(id);
    List<SceneEntity> list = node.Children.ToList<SceneEntity>();
    if (num1 != 0)
    {
      bool flag2 = flag1;
      SceneTree.TreeNodeFlag flag3 = list.Count != 0 ? (flag2 ? SceneTree.TreeNodeFlag.Expand : SceneTree.TreeNodeFlag.Collapse) : SceneTree.TreeNodeFlag.Leaf;
      float rightAdjust = this.DrawButtons(node, isHover);
      if (this.DrawNodeLabel(node, pos, flag3, rightAdjust))
        ((ImGuiStoragePtr) ref stateStorage).SetBool(id, flag1 = !flag1);
      if (node is ActorEntity || node is LightEntity)
      {
        FontAwesomeIcon icon = node is ActorEntity ? (FontAwesomeIcon) 61870 : (FontAwesomeIcon) 61675;
        double iconSpacing = (double) SceneTree.IconSpacing;
        ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
        double x = (double) ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
        float num2 = (float) (iconSpacing + x + (double) Icons.CalcIconSize(icon).X / 2.0);
        pos = new Vector2(pos.X + num2, pos.Y);
      }
      if (isHover && this.IsNodeHovered(pos, itemRectSize, rightAdjust))
      {
        if (Dalamud.Bindings.ImGui.ImGui.IsMouseDoubleClicked((ImGuiMouseButton) 0))
          this._ctx.Interface.OpenEditorFor(node);
        else if (Dalamud.Bindings.ImGui.ImGui.IsMouseClicked((ImGuiMouseButton) 0))
        {
          SelectMode selectMode = GuiHelpers.GetSelectMode();
          node.Select(selectMode);
        }
        else if (Dalamud.Bindings.ImGui.ImGui.IsMouseClicked((ImGuiMouseButton) 1))
          this._ctx.Interface.OpenSceneEntityMenu(node);
      }
    }
    if (!flag1)
      return;
    this.IterateTree((IEnumerable<SceneEntity>) list);
  }

  private bool DrawNodeLabel(
    SceneEntity item,
    Vector2 pos,
    SceneTree.TreeNodeFlag flag,
    float rightAdjust = 0.0f)
  {
    EntityDisplay entityDisplay = this._ctx.Config.GetEntityDisplay(item);
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(pos.X - ((ImGuiStylePtr) ref style).ItemSpacing.X);
    bool flag1 = this.DrawNodeCaret(entityDisplay.Color, flag);
    using (ImRaii.PushColor((ImGuiCol) 0, this.GetVisibleColor(item, entityDisplay.Color), true))
    {
      this.DrawNodeIcon(entityDisplay.Icon, item);
      float x = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X;
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(item.Name.FitToWidth(x - rightAdjust)));
      return flag1;
    }
  }

  private uint GetVisibleColor(SceneEntity item, uint curCol)
  {
    if ((!(item is ActorEntity actorEntity) || actorEntity.IsObjectVisible) && (!(item is LightEntity lightEntity) || lightEntity.IsObjectVisible))
      return curCol;
    float num = 0.6f;
    return (uint) ((int) (byte) (curCol >> 24 & (uint) byte.MaxValue) << 24 | (int) (byte) ((double) (byte) (curCol >> 16 /*0x10*/ & (uint) byte.MaxValue) * (double) num) << 16 /*0x10*/ | (int) (byte) ((double) (byte) (curCol >> 8 & (uint) byte.MaxValue) * (double) num) << 8) | (uint) (byte) ((double) (byte) (curCol & (uint) byte.MaxValue) * (double) num);
  }

  private bool DrawNodeCaret(uint color, SceneTree.TreeNodeFlag flag)
  {
    float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
    FontAwesomeIcon fontAwesomeIcon;
    switch (flag)
    {
      case SceneTree.TreeNodeFlag.Expand:
        fontAwesomeIcon = (FontAwesomeIcon) 61655;
        break;
      case SceneTree.TreeNodeFlag.Collapse:
        fontAwesomeIcon = (FontAwesomeIcon) 61658;
        break;
      default:
        fontAwesomeIcon = (FontAwesomeIcon) 0;
        break;
    }
    FontAwesomeIcon icon = fontAwesomeIcon;
    using (ImRaii.PushColor((ImGuiCol) 0, color.SetAlpha((byte) 207), true))
      Icons.DrawIcon(icon);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Vector2 vector2_1 = ((ImGuiStylePtr) ref style).ItemInnerSpacing;
    Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(cursorPosX + (vector2_1.X + SceneTree.IconSpacing));
    Vector2 vector2_2 = Icons.CalcIconSize(icon);
    float frameHeight = Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
    return ButtonsEx.IsClicked(new Vector2(SceneTree.IconSpacing - vector2_2.X, (float) (((double) frameHeight - (double) vector2_2.Y - (double) vector2_1.Y / 2.0) / 2.0)));
  }

  private void DrawNodeIcon(FontAwesomeIcon icon, SceneEntity item)
  {
    float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
    bool flag = icon > 0;
    float num1 = flag ? Icons.CalcIconSize(icon).X / 2f : 0.0f;
    float num2 = flag ? SceneTree.IconSpacing : 0.0f;
    Icons.DrawIcon(icon);
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, num2 - num1);
    switch (item)
    {
      case ActorEntity _:
      case LightEntity _:
        ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
        Vector2 vector2_1 = ((ImGuiStylePtr) ref style).ItemInnerSpacing;
        Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(cursorPosX + (vector2_1.X + SceneTree.IconSpacing));
        Vector2 vector2_2 = flag ? Icons.CalcIconSize(icon) : new Vector2();
        float frameHeight = Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
        if (!ButtonsEx.IsClicked(new Vector2(SceneTree.IconSpacing - vector2_2.X, (float) (((double) frameHeight - (double) vector2_2.Y - (double) vector2_1.Y / 2.0) / 2.0))))
          break;
        if (item is ActorEntity actorEntity)
          actorEntity.ToggleObjectVisibility();
        if (!(item is LightEntity lightEntity))
          break;
        lightEntity.ToggleObjectVisibility();
        break;
    }
  }

  private float DrawButtons(SceneEntity node, bool isHover)
  {
    double num1;
    float cursor = (float) (num1 = (double) Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + (double) Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
    this.DrawVisibilityButton(node, ref cursor, isHover);
    if (node is IAttachable attach)
      this.DrawAttachButton(attach, ref cursor, isHover);
    double num2 = (double) cursor;
    return (float) (num1 - num2);
  }

  private void DrawVisibilityButton(SceneEntity node, ref float cursor, bool isHover)
  {
    if (!(node is IVisibility visibility1))
      return;
    int num = this._ctx.Config.Overlay.Visible ? 1 : 0;
    bool visible = visibility1.Visible;
    uint rgba = visible ? 4026531839U /*0xEFFFFFFF*/ : 1627389951U;
    if (node is EntityPose entityPose1 && !visible && entityPose1.OverlayVisible)
      rgba = 2583691263U;
    if (num == 0)
      rgba = rgba.SetAlpha(visible ? (byte) 96 /*0x60*/ : (byte) 48 /*0x30*/);
    if (!(this.DrawButton(ref cursor, (FontAwesomeIcon) 61550, new uint?(rgba)) & isHover))
      return;
    if (node is EntityPose entityPose2)
    {
      entityPose2.OverlayVisible = !entityPose2.OverlayVisible;
      foreach (SceneEntity sceneEntity in entityPose2.Recurse())
      {
        if (sceneEntity is BoneNode && sceneEntity is IVisibility visibility2)
        {
          bool flag = entityPose2.OverlayVisible;
          if (flag)
          {
            BoneNode boneNode = sceneEntity as BoneNode;
            if (boneNode != null && boneNode.Parent is BoneNodeGroup && boneNode.Parent is BoneNodeGroup parent)
            {
              BoneCategory category = parent.Category;
              if (category != null)
              {
                if (category.HideOnPoseEntity)
                {
                  flag = false;
                }
                else
                {
                  CategoryBone categoryBone = category.Bones.FirstOrDefault<CategoryBone>((Func<CategoryBone, bool>) (x => x.Name == boneNode.Info.Name));
                  if (categoryBone != null && categoryBone.HideOnPoseEntity)
                    flag = false;
                }
              }
            }
          }
          visibility2.Visible = flag;
        }
      }
    }
    else
      visibility1.Toggle();
  }

  private void DrawAttachButton(IAttachable attach, ref float cursor, bool isHover)
  {
    if (!attach.IsAttached())
      return;
    if (this.DrawButton(ref cursor, (FontAwesomeIcon) 61633, new uint?(uint.MaxValue)) & isHover)
      this._ctx.Posing.Attachments.Detach(attach);
    if (!isHover || !Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      return;
    PartialBoneInfo parentBone = attach.GetParentBone();
    string str = parentBone != null ? this._ctx.Locale.GetBoneName(parentBone) : "UNKNOWN";
    using (ImRaii.Tooltip())
    {
      ImU8String imU8String = new ImU8String(12, 1);
      ((ImU8String) ref imU8String).AppendLiteral("Attached to ");
      ((ImU8String) ref imU8String).AppendFormatted<string>(str);
      Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
    }
  }

  private bool DrawButton(ref float cursor, FontAwesomeIcon icon, uint? color = null)
  {
    ref float local = ref cursor;
    double num1 = (double) cursor;
    double x1 = (double) Icons.CalcIconSize(icon).X;
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    double x2 = (double) ((ImGuiStylePtr) ref style).ItemSpacing.X;
    double num2 = x1 + x2;
    double num3 = num1 - num2;
    local = (float) num3;
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(cursor);
    using (ImRaii.PushColor((ImGuiCol) 0, color.GetValueOrDefault(), color.HasValue))
    {
      Icons.DrawIcon(icon);
      return ButtonsEx.IsClicked();
    }
  }

  private bool IsNodeHovered(Vector2 pos, Vector2 size, float rightAdjust)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemSpacing.X;
    Vector2 vec = Dalamud.Bindings.ImGui.ImGui.GetWindowPos() + pos.AddX(x).SubY(Dalamud.Bindings.ImGui.ImGui.GetScrollY() + 2f);
    return Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(vec, vec.Add(size.X - pos.X - x - rightAdjust, size.Y));
  }

  private enum TreeNodeFlag
  {
    Leaf,
    Expand,
    Collapse,
  }
}
