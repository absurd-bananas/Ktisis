// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.ConfigWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using GLib.Widgets;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Context;
using Ktisis.Interface.Components.Config;
using Ktisis.Interface.Types;
using Ktisis.Localization;
using Ktisis.Services.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Windows;

public class ConfigWindow : KtisisWindow
{
  private readonly ConfigManager _cfg;
  private readonly ContextManager _context;
  private readonly FormatService _format;
  private readonly ActionKeybindEditor _keybinds;
  private readonly BoneCategoryEditor _boneCategories;
  private readonly GizmoStyleEditor _gizmoStyle;
  public readonly LocaleManager Locale;
  private int resW;
  private int resH;

  private Configuration Config => this._cfg.File;

  public ConfigWindow(
    ConfigManager cfg,
    ContextManager context,
    FormatService format,
    ActionKeybindEditor keybinds,
    BoneCategoryEditor boneCategories,
    GizmoStyleEditor gizmoStyle,
    LocaleManager locale)
    : base("Ktisis Settings")
  {
    this._cfg = cfg;
    this._context = context;
    this._format = format;
    this._keybinds = keybinds;
    this._boneCategories = boneCategories;
    this._gizmoStyle = gizmoStyle;
    this.Locale = locale;
  }

  public virtual void OnOpen()
  {
    this._keybinds.Setup();
    this._boneCategories.Setup();
  }

  public virtual void Draw()
  {
    using (ImRaii.IEndObject iendObject = ImRaii.TabBar(ImU8String.op_Implicit("##ConfigTabs")))
    {
      if (!iendObject.Success)
        return;
      ConfigWindow.DrawTab(this.Locale.Translate("config.categories.title"), new Action(this.DrawCategoriesTab));
      ConfigWindow.DrawTab(this.Locale.Translate("config.gizmo.title"), new Action(this.DrawGizmoTab));
      ConfigWindow.DrawTab(this.Locale.Translate("config.overlay.title"), new Action(this.DrawOverlayTab));
      ConfigWindow.DrawTab(this.Locale.Translate("config.workspace.title"), new Action(this.DrawWorkspaceTab));
      ConfigWindow.DrawTab(this.Locale.Translate("config.autosave.title"), new Action(this.DrawAutoSaveTab));
      ConfigWindow.DrawTab(this.Locale.Translate("config.input.title"), new Action(this.DrawInputTab));
      ConfigWindow.DrawTab("Output", new Action(this.DrawOutputTab));
    }
  }

  private void DrawHint(string localeHandle)
  {
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    Icons.DrawIcon((FontAwesomeIcon) 61529);
    if (!Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      return;
    using (ImRaii.Tooltip())
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this.Locale.Translate(localeHandle)));
  }

  private static void DrawTab(string name, Action handler)
  {
    using (ImRaii.IEndObject iendObject = ImRaii.TabItem(ImU8String.op_Implicit(name)))
    {
      if (!iendObject.Success)
        return;
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      handler();
    }
  }

  private void DrawCategoriesTab()
  {
    int num1 = 0 | (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.categories.allow_nsfw")), ref this.Config.Categories.ShowNsfwBones) ? 1 : 0);
    this.DrawHint("config.categories.hint_nsfw");
    int num2 = Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.categories.show_all_viera_ears")), ref this.Config.Categories.ShowAllVieraEars) ? 1 : 0;
    int num3 = num1 | num2;
    this.DrawHint("config.categories.hint_viera_ears");
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.categories.show_friendly_bone_names")), ref this.Config.Categories.ShowFriendlyBoneNames);
    this.DrawHint("config.categories.hint_friendly_bones");
    if (num3 != 0)
      this.RefreshScene();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this.Locale.Translate("config.categories.header")));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this._boneCategories.Draw();
  }

  private void DrawGizmoTab()
  {
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.gizmo.flip")), ref this.Config.Gizmo.AllowAxisFlip);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.gizmo.raySnap")), ref this.Config.Gizmo.AllowRaySnap);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this.Locale.Translate("config.gizmo.header")));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this._gizmoStyle.Draw();
  }

  private void DrawOverlayTab()
  {
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.lines.draw")), ref this.Config.Overlay.DrawLines);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.lines.draw_gizmo")), ref this.Config.Overlay.DrawLinesGizmo);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.dots.draw_gizmo")), ref this.Config.Overlay.DrawDotsGizmo);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.references.draw_title")), ref this.Config.Overlay.DrawReferenceTitle);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.dots.radius")), ref this.Config.Overlay.DotRadius, 0.1f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Dot outline"), ref this.Config.Overlay.DotOutline, 0.1f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Dot radius (selected)"), ref this.Config.Overlay.DotRadiusSelected, 0.1f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Dot outline (selected)"), ref this.Config.Overlay.DotOutlineSelected, 0.1f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Dot opacity"), ref this.Config.Overlay.DotOpacity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Dot opacity while using gizmo"), ref this.Config.Overlay.DotOpacityUsing, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.lines.thick")), ref this.Config.Overlay.LineThickness, 0.1f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.lines.opacity")), ref this.Config.Overlay.LineOpacity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit(this.Locale.Translate("config.overlay.lines.opacity_gizmo")), ref this.Config.Overlay.LineOpacityUsing, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Active actor opacity multiplier"), ref this.Config.Overlay.ActiveActorOpacityMultiplier, 0.0f, 2f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Inactive actor opacity multiplier"), ref this.Config.Overlay.InactiveActorOpacityMultiplier, 0.0f, 2f, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Active State Type:"));
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit("Selection##activeState1"), this.Config.Overlay.ActiveStateType == OverlayConfig.ActiveState.Selection))
      this.Config.Overlay.ActiveStateType = OverlayConfig.ActiveState.Selection;
    if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Actor is considered 'active' when selected in the Workspace."));
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit("Target##activeState2"), this.Config.Overlay.ActiveStateType == OverlayConfig.ActiveState.Target))
      this.Config.Overlay.ActiveStateType = OverlayConfig.ActiveState.Target;
    if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Actor is considered 'active' when targeted."));
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit("Both##activeState3"), this.Config.Overlay.ActiveStateType == OverlayConfig.ActiveState.Both))
      this.Config.Overlay.ActiveStateType = OverlayConfig.ActiveState.Both;
    if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Actor is considered 'active' when either selected or targeted."));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Overlay Colors"));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.ColorPicker4("Dot color", "colDot", ref this.Config.Overlay.DotColor);
    this.ColorPicker4("Dot outline color", "colDotOutline", ref this.Config.Overlay.DotOutlineColor);
    this.ColorPicker4("Dot color (selected)", "colDotSelected", ref this.Config.Overlay.DotColorSelected);
    this.ColorPicker4("Dot outline color (selected)", "colDotOutlineSelected", ref this.Config.Overlay.DotOutlineColorSelected);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.ColorPicker4("Default line color", "colDefaultLine", ref this.Config.Overlay.DefaultLineColor);
    this.DrawHint("Group/Bone line colors on the Categories tab will override this.");
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Color selected bone parent lines"), ref this.Config.Overlay.ColorSelectedBoneParentLine);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    this.ColorPicker4("", "colBoneParentLine", ref this.Config.Overlay.SelectedBoneParentLineColor);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Color selected bone descendant lines"), ref this.Config.Overlay.ColorSelectedBoneDescendantLine);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    this.ColorPicker4("", "colBoneDescLine", ref this.Config.Overlay.SelectedBoneDescendantLineColor);
  }

  private bool ColorPicker4(string label, string id, ref uint value)
  {
    Vector4 float4 = Dalamud.Bindings.ImGui.ImGui.ColorConvertU32ToFloat4(value);
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(8, 1);
    ((ImU8String) ref imU8String).AppendLiteral("##");
    ((ImU8String) ref imU8String).AppendFormatted<string>(id);
    ((ImU8String) ref imU8String).AppendLiteral("Button");
    if (Dalamud.Bindings.ImGui.ImGui.ColorButton(imU8String, ref float4, (ImGuiColorEditFlags) 32 /*0x20*/, new Vector2()))
      Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit(id), (ImGuiPopupFlags) 0);
    if (!string.IsNullOrWhiteSpace(label))
    {
      Dalamud.Bindings.ImGui.ImGui.SameLine();
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(label));
    }
    bool flag = false;
    if (Dalamud.Bindings.ImGui.ImGui.BeginPopup(ImU8String.op_Implicit(id), (ImGuiWindowFlags) 0))
    {
      Dalamud.Bindings.ImGui.ImGui.SetColorEditOptions((ImGuiColorEditFlags) 177209344 /*0x0A900000*/);
      // ISSUE: explicit constructor call
      ((ImU8String) ref imU8String).\u002Ector(2, 2);
      ((ImU8String) ref imU8String).AppendFormatted<string>(label);
      ((ImU8String) ref imU8String).AppendLiteral("##");
      ((ImU8String) ref imU8String).AppendFormatted<string>(id);
      flag = Dalamud.Bindings.ImGui.ImGui.ColorPicker4(imU8String, ref float4, (ImGuiColorEditFlags) 181404034);
      Dalamud.Bindings.ImGui.ImGui.EndPopup();
    }
    value = Dalamud.Bindings.ImGui.ImGui.ColorConvertFloat4ToU32(float4);
    return flag;
  }

  private void DrawWorkspaceTab()
  {
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.workspace.init")), ref this.Config.Editor.OpenOnEnterGPose);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.workspace.editOnSelect")), ref this.Config.Editor.ToggleEditorOnSelect);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.workspace.legacyWindows")), ref this.Config.Editor.UseLegacyWindowBehavior);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.workspace.legacyPoseTabs")), ref this.Config.Editor.UseLegacyPoseViewTabs);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.workspace.legacyLightEditor")), ref this.Config.Editor.UseLegacyLightEditor);
  }

  private void DrawInputTab()
  {
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.input.enable")), ref this.Config.Keybinds.Enabled);
    this.DrawHint("Left-click: Edit binding\nRight-click: Remove binding\nBackspace: Cancel editing");
    if (!this.Config.Keybinds.Enabled)
      return;
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this.Locale.Translate("config.input.help")));
    this._keybinds.Draw();
  }

  private void DrawAutoSaveTab()
  {
    AutoSaveConfig autoSave = this.Config.AutoSave;
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.enable")), ref autoSave.Enabled);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.disconnect")), ref autoSave.OnDisconnect);
    Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.clear")), ref autoSave.ClearOnExit);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.SliderInt(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.interval")), ref autoSave.Interval, 10, 600, ImU8String.op_Implicit("%d s"), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.SliderInt(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.count")), ref autoSave.Count, 1, 20, new ImU8String(), (ImGuiSliderFlags) 0);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.path")), ref autoSave.FilePath, 256 /*0x0100*/, (ImGuiInputTextFlags) 0, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate) null);
    Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit(this.Locale.Translate("config.autosave.dir")), ref autoSave.FolderFormat, 256 /*0x0100*/, (ImGuiInputTextFlags) 0, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate) null);
    using (ImRaii.PushColor((ImGuiCol) 0, Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol) 1), true))
    {
      ImU8String imU8String = new ImU8String(21, 1);
      ((ImU8String) ref imU8String).AppendLiteral("Example folder name: ");
      ((ImU8String) ref imU8String).AppendFormatted<string>(this._format.Replace(autoSave.FolderFormat));
      Dalamud.Bindings.ImGui.ImGui.TextUnformatted(imU8String);
    }
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.DrawAutoSaveFormatting();
  }

  private void DrawAutoSaveFormatting()
  {
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(20, 0);
    ((ImU8String) ref imU8String).AppendLiteral("##AutoSaveFormatters");
    using (ImRaii.IEndObject iendObject = ImRaii.Table(imU8String, 2, (ImGuiTableFlags) 2107264))
    {
      if (!iendObject.Success)
        return;
      Dalamud.Bindings.ImGui.ImGui.TableSetupScrollFreeze(0, 1);
      Dalamud.Bindings.ImGui.ImGui.TableSetupColumn(ImU8String.op_Implicit("Formatter"), (ImGuiTableColumnFlags) 0, 0.0f, 0U);
      Dalamud.Bindings.ImGui.ImGui.TableSetupColumn(ImU8String.op_Implicit("Example Value"), (ImGuiTableColumnFlags) 0, 0.0f, 0U);
      Dalamud.Bindings.ImGui.ImGui.TableHeadersRow();
      foreach (KeyValuePair<string, string> replacement in this._format.GetReplacements())
      {
        string str1;
        string str2;
        replacement.Deconstruct(ref str1, ref str2);
        string str3 = str1;
        string str4 = str2;
        Dalamud.Bindings.ImGui.ImGui.TableNextRow();
        Dalamud.Bindings.ImGui.ImGui.TableNextColumn();
        Dalamud.Bindings.ImGui.ImGui.TextUnformatted(ImU8String.op_Implicit(str3));
        Dalamud.Bindings.ImGui.ImGui.TableNextColumn();
        Dalamud.Bindings.ImGui.ImGui.TextUnformatted(ImU8String.op_Implicit(str4));
      }
    }
  }

  private void DrawOutputTab()
  {
    PyonConfig pyon = this.Config.Pyon;
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Hi-Res Output"));
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    Icons.DrawIcon((FontAwesomeIcon) 61529);
    if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("The below selected resolution will be toggled when pressing\nthe 'Toggle hi-res mode' keybind set on the Input tab (Default F9).\n\nWhen hi-res is toggled, press your Reshade/Gshade screenshot key to save the image at this resolution.\n\nThen press the toggle key again to revert back to your original resolution."));
    if (pyon.Resolutions.Count == 0)
    {
      pyon.Resolutions.Add(new Size(1280 /*0x0500*/, 768 /*0x0300*/));
      pyon.Resolutions.Add(new Size(1600, 900));
      pyon.Resolutions.Add(new Size(1920, 1080));
      pyon.Resolutions.Add(new Size(1920, 1200));
      pyon.Resolutions.Add(new Size(2560 /*0x0A00*/, 1440));
      pyon.Resolutions.Add(new Size(2560 /*0x0A00*/, 1600));
      pyon.Resolutions.Add(new Size(3200, 1800));
      pyon.Resolutions.Add(new Size(3440, 1440));
      pyon.Resolutions.Add(new Size(3840 /*0x0F00*/, 2160));
      pyon.Resolutions.Add(new Size(3840 /*0x0F00*/, 2400));
      pyon.Resolutions.Add(new Size(4096 /*0x1000*/, 2160));
      pyon.Resolutions.Add(new Size(5120, 2880));
      pyon.Resolutions.Add(new Size(7680, 4320));
      pyon.HiResSize = new Size(3840 /*0x0F00*/, 2160);
      this.resW = 3840 /*0x0F00*/;
      this.resH = 2160;
    }
    int index1 = 0;
    Size resolution;
    for (int index2 = 0; index2 < pyon.Resolutions.Count; ++index2)
    {
      if (pyon.Resolutions[index2].Width == pyon.HiResSize.Width && pyon.Resolutions[index2].Height == pyon.HiResSize.Height)
      {
        index1 = index2;
        if (this.resW == 0)
        {
          resolution = pyon.Resolutions[index2];
          this.resW = resolution.Width;
          resolution = pyon.Resolutions[index2];
          this.resH = resolution.Height;
          break;
        }
        break;
      }
    }
    string[] array = pyon.Resolutions.Select<Size, string>((Func<Size, string>) (r => $"{r.Width} x {r.Height}")).ToArray<string>();
    Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(210f);
    if (Dalamud.Bindings.ImGui.ImGui.Combo(ImU8String.op_Implicit("Resolution"), ref index1, ReadOnlySpan<string>.op_Implicit(array), array.Length))
    {
      pyon.HiResSize = pyon.Resolutions[index1];
      this.resW = pyon.HiResSize.Width;
      this.resH = pyon.HiResSize.Height;
    }
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Add/Remove Resolution"));
    Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(70f);
    if (Dalamud.Bindings.ImGui.ImGui.DragInt(ImU8String.op_Implicit("##w"), ref this.resW, 1f, 1040, 9999, new ImU8String(), (ImGuiSliderFlags) 0))
    {
      if (this.resW < 1040)
        this.resW = 1040;
      if (this.resW > 9999)
        this.resW = 9999;
    }
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 2f);
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("x"));
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 2f);
    Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(70f);
    if (Dalamud.Bindings.ImGui.ImGui.DragInt(ImU8String.op_Implicit("##h"), ref this.resH, 1f, 768 /*0x0300*/, 9999, new ImU8String(), (ImGuiSliderFlags) 0))
    {
      if (this.resH < 768 /*0x0300*/)
        this.resH = 768 /*0x0300*/;
      if (this.resH > 9999)
        this.resH = 9999;
    }
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    if (Buttons.IconButton((FontAwesomeIcon) 61543, new Vector2?(new Vector2(24f, 24f))) && pyon.Resolutions.Find((Predicate<Size>) (x => x.Width == this.resW && x.Height == this.resH)) == Size.Empty)
    {
      pyon.Resolutions.Add(new Size(this.resW, this.resH));
      pyon.Resolutions.Sort((Comparison<Size>) ((a, b) => (a.Width + a.Height).CompareTo(b.Width + b.Height)));
      pyon.HiResSize.Width = this.resW;
      pyon.HiResSize.Height = this.resH;
    }
    if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Add the specified width/height to the Resolution list."));
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, 2f);
    Dalamud.Bindings.ImGui.ImGui.BeginDisabled(pyon.Resolutions.Count <= 1);
    if (Buttons.IconButton((FontAwesomeIcon) 61544, new Vector2?(new Vector2(24f, 24f))))
    {
      Size? nullable = new Size?(pyon.Resolutions.Find((Predicate<Size>) (x => x.Width == this.resW && x.Height == this.resH)));
      if (nullable.HasValue)
      {
        pyon.Resolutions.Remove(nullable ?? Size.Empty);
        resolution = pyon.Resolutions[0];
        this.resW = resolution.Width;
        resolution = pyon.Resolutions[0];
        this.resH = resolution.Height;
      }
    }
    Dalamud.Bindings.ImGui.ImGui.EndDisabled();
    if (!Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      return;
    Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Remove the specified width/height from the Resolution list."));
  }

  private void RefreshScene() => this._context.Current?.Scene.Refresh();

  public override void OnClose()
  {
    base.OnClose();
    this._cfg.Save();
  }
}
