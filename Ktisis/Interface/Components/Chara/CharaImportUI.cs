// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.CharaImportUI
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Core.Attributes;
using Ktisis.Data.Files;
using Ktisis.Editor.Characters;
using Ktisis.Editor.Context.Types;
using Ktisis.GameData.Excel.Types;
using Ktisis.Interface.Components.Chara.Select;
using Ktisis.Interface.Components.Files;
using Ktisis.Scene.Entities.Game;
using System;

#nullable enable
namespace Ktisis.Interface.Components.Chara;

[Transient]
public class CharaImportUI
{
  public Action<CharaImportUI>? OnNpcSelected;
  private readonly NpcSelect _npcs;
  private readonly FileSelect<CharaFile> _select;
  private CharaImportUI.LoadMethod _method;

  public IEditorContext Context { set; private get; }

  public CharaImportUI(NpcSelect npcs, FileSelect<CharaFile> select)
  {
    this._npcs = npcs;
    this._npcs.OnSelected += new Ktisis.Interface.Components.Chara.Select.OnNpcSelected(this.OnNpcSelect);
    this._select = select;
    this._select.OnOpenDialog += new FileSelect<CharaFile>.OpenDialogHandler(this.OnFileDialogOpen);
  }

  public void Initialize() => this._npcs.Fetch();

  private void OnNpcSelect(INpcBase _)
  {
    if (!this.Context.Config.File.ImportNpcApplyOnSelect)
      return;
    Action<CharaImportUI> onNpcSelected = this.OnNpcSelected;
    if (onNpcSelected == null)
      return;
    onNpcSelected(this);
  }

  private void OnFileDialogOpen(FileSelect<CharaFile> sender)
  {
    this.Context.Interface.OpenCharaFile(new Action<string, CharaFile>(sender.SetFile));
  }

  public bool HasSelection
  {
    get
    {
      bool hasSelection;
      switch (this._method)
      {
        case CharaImportUI.LoadMethod.File:
          hasSelection = this._select.IsFileOpened;
          break;
        case CharaImportUI.LoadMethod.Npc:
          hasSelection = this._npcs.Selected != null;
          break;
        default:
          hasSelection = false;
          break;
      }
      return hasSelection;
    }
  }

  public void ApplyTo(ActorEntity actor)
  {
    switch (this._method)
    {
      case CharaImportUI.LoadMethod.File:
        this.ApplyCharaFile(actor);
        break;
      case CharaImportUI.LoadMethod.Npc:
        this.ApplyNpc(actor);
        break;
      default:
        throw new ArgumentOutOfRangeException(this._method.ToString());
    }
  }

  private void ApplyCharaFile(ActorEntity actor)
  {
    CharaFile file = this._select.Selected?.File;
    if (file == null)
      return;
    this.Context.Characters.ApplyCharaFile(actor, file, this.Context.Config.File.ImportCharaModes);
  }

  public void ApplyNpc(ActorEntity actor)
  {
    INpcBase selected = this._npcs.Selected;
    if (selected == null)
      return;
    this.Context.Characters.ApplyNpc(actor, selected, this.Context.Config.File.ImportCharaModes);
  }

  public void DrawImport()
  {
    switch (this._method)
    {
      case CharaImportUI.LoadMethod.File:
        this._select.Draw();
        break;
      case CharaImportUI.LoadMethod.Npc:
        this._npcs.Draw();
        Dalamud.Bindings.ImGui.ImGui.Spacing();
        Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Apply on selection"), ref this.Context.Config.File.ImportNpcApplyOnSelect);
        break;
      default:
        throw new ArgumentOutOfRangeException(this._method.ToString());
    }
  }

  public void DrawSimpleImport()
  {
  }

  public void DrawLoadMethods(float cursorY = -1f)
  {
    int num = (double) cursorY > -1.0 ? 1 : 0;
    if (num != 0)
      Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(cursorY);
    this.DrawMethodRadio("File", CharaImportUI.LoadMethod.File);
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
    if (num != 0)
      Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(cursorY);
    this.DrawMethodRadio("NPC", CharaImportUI.LoadMethod.Npc);
  }

  private void DrawMethodRadio(string label, CharaImportUI.LoadMethod method)
  {
    if (!Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit(label), this._method == method))
      return;
    this._method = method;
  }

  public void DrawModesSelect()
  {
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Appearance"));
    this.DrawModeSwitch("Body", SaveModes.AppearanceBody);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    this.DrawModeSwitch("Face", SaveModes.AppearanceFace);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    this.DrawModeSwitch("Hair", SaveModes.AppearanceHair);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Equipment"));
    this.DrawModeSwitch("Gear", SaveModes.EquipmentGear);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    this.DrawModeSwitch("Accessories", SaveModes.EquipmentAccessories);
    Dalamud.Bindings.ImGui.ImGui.SameLine();
    this.DrawModeSwitch("Weapons", SaveModes.EquipmentWeapons);
  }

  private void DrawModeSwitch(string label, SaveModes mode)
  {
    bool flag = this.Context.Config.File.ImportCharaModes.HasFlag((Enum) mode);
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(20, 2);
    ((ImU8String) ref imU8String).AppendFormatted<string>(label);
    ((ImU8String) ref imU8String).AppendLiteral("##CharaImportDialog_");
    ((ImU8String) ref imU8String).AppendFormatted<SaveModes>(mode);
    if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(imU8String, ref flag))
      return;
    this.Context.Config.File.ImportCharaModes ^= mode;
  }

  private enum LoadMethod
  {
    File,
    Npc,
  }
}
