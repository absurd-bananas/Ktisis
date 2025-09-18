// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Config.BoneCategoryEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Bones;
using Ktisis.Data.Config.Sections;
using Ktisis.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Components.Config;

[Transient]
public class BoneCategoryEditor
{
  private readonly ConfigManager _cfg;
  private readonly LocaleManager _locale;
  private readonly Dictionary<string, List<BoneCategory>> CategoryMap = new Dictionary<string, List<BoneCategory>>();
  private BoneCategory? Selected;
  private bool ColorSub;

  private CategoryConfig Config => this._cfg.File.Categories;

  public BoneCategoryEditor(ConfigManager cfg, LocaleManager locale)
  {
    this._cfg = cfg;
    this._locale = locale;
  }

  public void Setup()
  {
    this.Selected = (BoneCategory) null;
    this.BuildCategoryMap();
  }

  private void BuildCategoryMap()
  {
    this.CategoryMap.Clear();
    for (int index = -1; index < this.Config.CategoryList.Count; ++index)
    {
      string parent = index >= 0 ? this.Config.CategoryList[index].Name : (string) null;
      List<BoneCategory> list = this.Config.CategoryList.Where<BoneCategory>((Func<BoneCategory, bool>) (cat => cat.ParentCategory == parent)).ToList<BoneCategory>();
      if (list.Count > 0)
        this.CategoryMap.Add(parent ?? string.Empty, list);
    }
  }

  public void Draw()
  {
    using (ImRaii.PushStyle((ImGuiStyleVar) 1, Vector2.Zero, true))
    {
      using (ImRaii.IEndObject iendObject1 = ImRaii.Child(ImU8String.op_Implicit("##BoneCategoriesFrame"), Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail(), true))
      {
        if (!iendObject1.Success)
          return;
        using (ImRaii.PushStyle((ImGuiStyleVar) 16 /*0x10*/, new Vector2(10f, 10f), true))
        {
          using (ImRaii.IEndObject iendObject2 = ImRaii.Table(ImU8String.op_Implicit("##BoneCategoriesTable"), 2, (ImGuiTableFlags) 1))
          {
            if (!iendObject2.Success)
              return;
            Dalamud.Bindings.ImGui.ImGui.TableSetupColumn(ImU8String.op_Implicit("CategoryList"), (ImGuiTableColumnFlags) 0, 0.0f, 0U);
            Dalamud.Bindings.ImGui.ImGui.TableSetupColumn(ImU8String.op_Implicit("CategoryInfo"), (ImGuiTableColumnFlags) 0, 0.0f, 0U);
            Dalamud.Bindings.ImGui.ImGui.TableNextRow();
            this.DrawCategoryList();
            this.DrawCategoryInfo();
            ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
            Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail() with
            {
              X = 0.0f
            };
            contentRegionAvail.Y -= ((ImGuiStylePtr) ref style).ItemSpacing.Y + ((ImGuiStylePtr) ref style).CellPadding.Y;
            Dalamud.Bindings.ImGui.ImGui.Dummy(contentRegionAvail);
          }
        }
      }
    }
  }

  private void DrawCategoryList()
  {
    Dalamud.Bindings.ImGui.ImGui.TableNextColumn();
    ImFontPtr defaultFont = UiBuilder.DefaultFont;
    using (ImRaii.PushStyle((ImGuiStyleVar) 15, ((ImFontPtr) ref defaultFont).FontSize, true))
      this.DrawCategoryList(string.Empty);
  }

  private void DrawCategoryList(string key)
  {
    List<BoneCategory> boneCategoryList;
    if (!this.CategoryMap.TryGetValue(key, out boneCategoryList))
      return;
    boneCategoryList.ForEach(new Action<BoneCategory>(this.DrawListCategory));
  }

  private void DrawListCategory(BoneCategory category)
  {
    if (category.IsNsfw && !this.Config.ShowNsfwBones)
      return;
    using (ImRaii.IEndObject iendObject = this.DrawCategoryNode(category))
    {
      if (Dalamud.Bindings.ImGui.ImGui.IsItemClicked() && (double) Dalamud.Bindings.ImGui.ImGui.GetItemRectMin().X + (double) Dalamud.Bindings.ImGui.ImGui.GetTreeNodeToLabelSpacing() < (double) Dalamud.Bindings.ImGui.ImGui.GetMousePos().X)
        this.Selected = this.Selected != category ? category : (BoneCategory) null;
      if (!iendObject.Success)
        return;
      this.DrawCategoryList(category.Name);
    }
  }

  private ImRaii.IEndObject DrawCategoryNode(BoneCategory category)
  {
    using (ImRaii.PushColor((ImGuiCol) 0, category.GroupColor, true))
    {
      ImGuiTreeNodeFlags guiTreeNodeFlags = (ImGuiTreeNodeFlags) 2176;
      if (this.Selected == category)
        guiTreeNodeFlags = (ImGuiTreeNodeFlags) (guiTreeNodeFlags | 1);
      if (!this.CategoryMap.ContainsKey(category.Name))
        guiTreeNodeFlags = (ImGuiTreeNodeFlags) (guiTreeNodeFlags | 256 /*0x0100*/);
      return ImRaii.TreeNode(ImU8String.op_Implicit(this._locale.GetCategoryName(category)), guiTreeNodeFlags);
    }
  }

  private void DrawCategoryInfo()
  {
    Dalamud.Bindings.ImGui.ImGui.TableNextColumn();
    if (this.Selected == null)
      return;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._locale.Translate("config.categories.editor.color_header")));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.DrawCategoryColors(this.Selected);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.DrawCategoryOverlayOptions(this.Selected);
  }

  private void DrawCategoryOverlayOptions(BoneCategory category)
  {
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Overlay:"));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Hide with 'Pose' entity"), ref category.HideOnPoseEntity);
    if (!Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      return;
    Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Bones in this category will not be visible when the 'Pose' entity of an actor is made visible.\nThey will only be visible when you specifically set the category to be visible.\n\nThis option can also instead be assigned per group/bone from the group/bone context menu in the Workspace."));
  }

  private void DrawCategoryColors(BoneCategory category)
  {
    this.DrawSwitches(category);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    bool flag = false;
    if (!(!category.LinkedColors ? flag | BoneCategoryEditor.DrawColorEdit(this._locale.Translate("config.categories.editor.group_color"), ref category.GroupColor) | BoneCategoryEditor.DrawColorEdit(this._locale.Translate("config.categories.editor.bone_color"), ref category.BoneColor) : BoneCategoryEditor.DrawColorEdit(this._locale.Translate("config.categories.editor.group_color"), ref category.GroupColor)) || !this.ColorSub)
      return;
    this.SetColors(category, category);
  }

  private void SetColors(BoneCategory parent, BoneCategory child)
  {
    if (parent != child)
      child.GroupColor = parent.GroupColor;
    child.LinkedColors |= parent.LinkedColors;
    if (!child.LinkedColors)
      child.BoneColor = parent.BoneColor;
    List<BoneCategory> boneCategoryList;
    if (!this.CategoryMap.TryGetValue(child.Name, out boneCategoryList))
      return;
    boneCategoryList.ForEach((Action<BoneCategory>) (cat => this.SetColors(parent, cat)));
  }

  private void DrawSwitches(BoneCategory category)
  {
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._locale.Translate("config.categories.editor.subcategories")), ref this.ColorSub);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this._locale.Translate("config.categories.editor.link_colors")), ref category.LinkedColors);
  }

  private static bool DrawColorEdit(string label, ref uint color)
  {
    Vector4 float4 = Dalamud.Bindings.ImGui.ImGui.ColorConvertU32ToFloat4(color);
    int num = Dalamud.Bindings.ImGui.ImGui.ColorEdit4(ImU8String.op_Implicit(label), ref float4, (ImGuiColorEditFlags) 0) ? 1 : 0;
    if (num == 0)
      return num != 0;
    color = Dalamud.Bindings.ImGui.ImGui.ColorConvertFloat4ToU32(float4);
    return num != 0;
  }
}
