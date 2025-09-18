// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.EquipmentEditorTab
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using GLib.Lists;
using GLib.Popups;
using GLib.Widgets;
using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Core.Attributes;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.GameData.Excel;
using Ktisis.Interface.Components.Chara.Types;
using Lumina.Data;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Interface.Components.Chara;

[Transient]
public class EquipmentEditorTab
{
  private readonly IDataManager _data;
  private readonly ITextureProvider _tex;
  private readonly PopupList<ItemSheet> _itemSelectPopup;
  private readonly PopupList<Stain> _dyeSelectPopup;
  private readonly PopupList<Ktisis.GameData.Excel.Glasses> _glassesSelectPopup;
  private IEquipmentEditor _editor;
  private static readonly EquipSlot[] EquipSlots = ((IEnumerable<EquipIndex>) Enum.GetValues<EquipIndex>()).Select<EquipIndex, EquipSlot>((Func<EquipIndex, EquipSlot>) (index => index.ToEquipSlot())).ToArray<EquipSlot>();
  private static readonly Vector2 ButtonSize = new Vector2(42f, 42f);
  private EquipSlot ItemSelectSlot;
  private List<ItemSheet> ItemSelectList = new List<ItemSheet>();
  private EquipSlot DyeSelectSlot;
  private int DyeSelectIndex;
  private int GlassesSelectIndex;
  private bool _itemsRaii;
  private readonly List<ItemSheet> Items = new List<ItemSheet>();
  private readonly List<Stain> Stains = new List<Stain>();
  private readonly List<Ktisis.GameData.Excel.Glasses> Glasses = new List<Ktisis.GameData.Excel.Glasses>();
  private readonly object _equipUpdateLock = new object();
  private readonly Dictionary<EquipSlot, ItemInfo> Equipped = new Dictionary<EquipSlot, ItemInfo>();

  public IEquipmentEditor Editor
  {
    private get => this._editor;
    set
    {
      this._editor = value;
      this.InvalidateCache();
    }
  }

  public EquipmentEditorTab(IDataManager data, ITextureProvider tex)
  {
    this._data = data;
    this._tex = tex;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._itemSelectPopup = new PopupList<ItemSheet>("##ItemSelectPopup", EquipmentEditorTab.\u003C\u003EO.\u003C0\u003E__ItemSelectDrawRow ?? (EquipmentEditorTab.\u003C\u003EO.\u003C0\u003E__ItemSelectDrawRow = new ListBox<ItemSheet>.DrawItemDelegate(EquipmentEditorTab.ItemSelectDrawRow))).WithSearch(EquipmentEditorTab.\u003C\u003EO.\u003C1\u003E__ItemSelectSearchPredicate ?? (EquipmentEditorTab.\u003C\u003EO.\u003C1\u003E__ItemSelectSearchPredicate = new PopupList<ItemSheet>.SearchPredicate(EquipmentEditorTab.ItemSelectSearchPredicate)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._dyeSelectPopup = new PopupList<Stain>("##DyeSelectPopup", EquipmentEditorTab.\u003C\u003EO.\u003C2\u003E__DyeSelectDrawRow ?? (EquipmentEditorTab.\u003C\u003EO.\u003C2\u003E__DyeSelectDrawRow = new ListBox<Stain>.DrawItemDelegate(EquipmentEditorTab.DyeSelectDrawRow))).WithSearch(EquipmentEditorTab.\u003C\u003EO.\u003C3\u003E__DyeSelectSearchPredicate ?? (EquipmentEditorTab.\u003C\u003EO.\u003C3\u003E__DyeSelectSearchPredicate = new PopupList<Stain>.SearchPredicate(EquipmentEditorTab.DyeSelectSearchPredicate)));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    this._glassesSelectPopup = new PopupList<Ktisis.GameData.Excel.Glasses>("##GlassesSelectPopup", EquipmentEditorTab.\u003C\u003EO.\u003C4\u003E__GlassesSelectDrawRow ?? (EquipmentEditorTab.\u003C\u003EO.\u003C4\u003E__GlassesSelectDrawRow = new ListBox<Ktisis.GameData.Excel.Glasses>.DrawItemDelegate(EquipmentEditorTab.GlassesSelectDrawRow))).WithSearch(EquipmentEditorTab.\u003C\u003EO.\u003C5\u003E__GlassesSelectSearchPredicate ?? (EquipmentEditorTab.\u003C\u003EO.\u003C5\u003E__GlassesSelectSearchPredicate = new PopupList<Ktisis.GameData.Excel.Glasses>.SearchPredicate(EquipmentEditorTab.GlassesSelectSearchPredicate)));
  }

  public void Draw()
  {
    this.FetchData();
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.PushItemWidth(Dalamud.Bindings.ImGui.ImGui.GetWindowSize().X / 2f - ((ImGuiStylePtr) ref style).ItemSpacing.X);
    try
    {
      lock (this._equipUpdateLock)
      {
        this.DrawItemSlots(((IEnumerable<EquipSlot>) EquipmentEditorTab.EquipSlots).Take<EquipSlot>(5).Prepend<EquipSlot>(EquipSlot.MainHand));
        Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemSpacing.X);
        this.DrawItemSlots(((IEnumerable<EquipSlot>) EquipmentEditorTab.EquipSlots).Skip<EquipSlot>(5).Prepend<EquipSlot>(EquipSlot.OffHand));
      }
    }
    finally
    {
      Dalamud.Bindings.ImGui.ImGui.PopItemWidth();
    }
    this.DrawGlassesSelect();
    this.DrawPopups();
  }

  private void DrawPopups()
  {
    this.DrawItemSelectPopup();
    this.DrawDyeSelectPopup();
    this.DrawGlassesSelectPopup();
  }

  private void DrawItemSlots(IEnumerable<EquipSlot> slots)
  {
    using (ImRaii.Group())
    {
      foreach (EquipSlot slot in slots)
        this.DrawItemSlot(slot);
    }
  }

  private void DrawItemSlot(EquipSlot slot)
  {
    this.UpdateSlot(slot);
    ItemInfo info;
    if (!this.Equipped.TryGetValue(slot, out info))
      return;
    float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    this.DrawItemButton(info);
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
    using (ImRaii.Group())
    {
      EquipmentEditorTab.PrepareItemLabel(info.Item, info.ModelId, cursorPosX, x);
      ImU8String imU8String1;
      switch (info)
      {
        case WeaponInfo weaponInfo:
          int[] numArray1 = new int[3]
          {
            (int) weaponInfo.Model.Id,
            (int) weaponInfo.Model.Type,
            (int) weaponInfo.Model.Variant
          };
          imU8String1 = new ImU8String(7, 1);
          ((ImU8String) ref imU8String1).AppendLiteral("##Input");
          ((ImU8String) ref imU8String1).AppendFormatted<EquipSlot>(slot);
          ImU8String imU8String2 = imU8String1;
          Span<int> span = Span<int>.op_Implicit(numArray1);
          imU8String1 = new ImU8String();
          ImU8String imU8String3 = imU8String1;
          if (Dalamud.Bindings.ImGui.ImGui.InputInt(imU8String2, span, 0, 0, imU8String3, (ImGuiInputTextFlags) 0))
          {
            weaponInfo.SetModel((ushort) numArray1[0], (ushort) numArray1[1], (byte) numArray1[2]);
            break;
          }
          break;
        case EquipInfo equipInfo:
          int[] numArray2 = new int[2]
          {
            (int) equipInfo.Model.Id,
            (int) equipInfo.Model.Variant
          };
          ImU8String imU8String4 = new ImU8String(7, 1);
          ((ImU8String) ref imU8String4).AppendLiteral("##Input");
          ((ImU8String) ref imU8String4).AppendFormatted<EquipSlot>(slot);
          if (Dalamud.Bindings.ImGui.ImGui.InputInt(imU8String4, Span<int>.op_Implicit(numArray2), 0, 0, new ImU8String(), (ImGuiInputTextFlags) 0))
          {
            equipInfo.SetModel((ushort) numArray2[0], (byte) numArray2[1]);
            break;
          }
          break;
      }
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
      this.DrawDyeButton(info, 0);
      Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
      this.DrawDyeButton(info, 1);
      if (info.IsHideable)
      {
        imU8String1 = new ImU8String(13, 1);
        ((ImU8String) ref imU8String1).AppendLiteral("EqSetVisible_");
        ((ImU8String) ref imU8String1).AppendFormatted<EquipSlot>(slot);
        using (ImRaii.PushId(imU8String1, true))
        {
          using (ImRaii.PushColor((ImGuiCol) 21, 0U, true))
          {
            Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
            bool isVisible = info.IsVisible;
            if (Buttons.IconButtonTooltip(isVisible ? (FontAwesomeIcon) 61550 : (FontAwesomeIcon) 61552, "Toggle item visibility"))
              info.SetVisible(!isVisible);
          }
        }
      }
      if (!info.IsVisor)
        return;
      imU8String1 = new ImU8String(12, 1);
      ((ImU8String) ref imU8String1).AppendLiteral("EqSetToggle_");
      ((ImU8String) ref imU8String1).AppendFormatted<EquipSlot>(slot);
      using (ImRaii.PushId(imU8String1, true))
      {
        using (ImRaii.PushColor((ImGuiCol) 21, 0U, true))
        {
          Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
          bool isVisorToggled = info.IsVisorToggled;
          using (ImRaii.PushColor((ImGuiCol) 0, Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol) 1), isVisorToggled))
          {
            if (!Buttons.IconButtonTooltip((FontAwesomeIcon) 63226, "Toggle visor"))
              return;
            info.SetVisorToggled(!isVisorToggled);
          }
        }
      }
    }
  }

  private static void PrepareItemLabel(
    ItemSheet? item,
    ushort modelId,
    float cursorStart,
    float innerSpace)
  {
    float width = Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() - cursorStart);
    Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(width);
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit((item?.Name ?? (modelId == (ushort) 0 ? "Empty" : "Unknown")).FitToWidth(width)));
    Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(EquipmentEditorTab.CalcItemWidth(cursorStart));
  }

  private void DrawItemButton(ItemInfo info)
  {
    using (ImRaii.PushColor((ImGuiCol) 21, 0U, true))
    {
      ImU8String imU8String = new ImU8String(13, 1);
      ((ImU8String) ref imU8String).AppendLiteral("##ItemButton_");
      ((ImU8String) ref imU8String).AppendFormatted<EquipSlot>(info.Slot);
      bool flag;
      using (ImRaii.PushId(imU8String, true))
        flag = info.Texture == null ? Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit(info.Slot.ToString()), EquipmentEditorTab.ButtonSize) : Dalamud.Bindings.ImGui.ImGui.ImageButton(info.Texture.GetWrapOrEmpty().Handle, EquipmentEditorTab.ButtonSize);
      if (flag)
        this.OpenItemSelectPopup(info.Slot);
      if (!Dalamud.Bindings.ImGui.ImGui.IsItemClicked((ImGuiMouseButton) 1))
        return;
      info.Unequip();
    }
  }

  private void OpenItemSelectPopup(EquipSlot slot)
  {
    this.ItemSelectSlot = slot;
    this.ItemSelectList.Clear();
    lock (this.Items)
      this.ItemSelectList = this.Items.Where<ItemSheet>((Func<ItemSheet, bool>) (item => item.IsEquippable(slot))).ToList<ItemSheet>();
    this._itemSelectPopup.Open();
  }

  private void DrawItemSelectPopup()
  {
    ItemSheet selected;
    if (!this._itemSelectPopup.IsOpen || !this._itemSelectPopup.Draw(this.ItemSelectList, out selected))
      return;
    lock (this.Equipped)
    {
      ItemInfo itemInfo;
      if (!this.Equipped.TryGetValue(this.ItemSelectSlot, out itemInfo))
        return;
      itemInfo.SetEquipItem(selected);
    }
  }

  private static bool ItemSelectDrawRow(ItemSheet item, bool isFocus)
  {
    return Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(item.Name), isFocus, (ImGuiSelectableFlags) 0, new Vector2());
  }

  private static bool ItemSelectSearchPredicate(ItemSheet item, string query)
  {
    return item.Name.Contains(query, StringComparison.OrdinalIgnoreCase);
  }

  private static uint CalcStainColor(Stain? stain)
  {
    uint num1 = 4278190080 /*0xFF000000*/;
    if (stain.HasValue)
    {
      int num2 = (int) num1;
      Stain stain1 = stain.Value;
      int num3 = (int) (((Stain) ref stain1).Color << 8).FlipEndian();
      num1 = (uint) (num2 | num3);
    }
    return num1;
  }

  private void DrawDyeButton(ItemInfo info, int index)
  {
    Stain? stain1 = new Stain?();
    foreach (Stain stain2 in this.Stains)
    {
      if ((int) ((Stain) ref stain2).RowId == (int) info.StainIds[index])
      {
        lock (this.Stains)
          stain1 = new Stain?(stain2);
      }
    }
    uint color = EquipmentEditorTab.CalcStainColor(stain1);
    Vector4 float4 = Dalamud.Bindings.ImGui.ImGui.ColorConvertU32ToFloat4(color);
    ImU8String imU8String;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String).\u002Ector(13, 2);
    ((ImU8String) ref imU8String).AppendLiteral("##DyeSelect_");
    ((ImU8String) ref imU8String).AppendFormatted<EquipSlot>(info.Slot);
    ((ImU8String) ref imU8String).AppendLiteral("_");
    ((ImU8String) ref imU8String).AppendFormatted<int>(index);
    if (Dalamud.Bindings.ImGui.ImGui.ColorButton(imU8String, ref float4, (ImGuiColorEditFlags) 64 /*0x40*/, new Vector2()))
      this.OpenDyeSelectPopup(info.Slot, index);
    if (Dalamud.Bindings.ImGui.ImGui.IsItemClicked((ImGuiMouseButton) 1))
      info.SetStainId((byte) 0, index);
    if (!Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
      return;
    EquipmentEditorTab.DrawDyeTooltip(stain1, color, float4);
  }

  private static void DrawDyeTooltip(Stain? stain, uint color, Vector4 colorVec4)
  {
    using (ImRaii.PushColor((ImGuiCol) 0, color, ((double) colorVec4.X + (double) colorVec4.Y + (double) colorVec4.Z) / 3.0 > 0.10000000149011612))
    {
      using (ImRaii.Tooltip())
      {
        Stain valueOrDefault;
        string str1;
        if (!stain.HasValue)
        {
          str1 = (string) null;
        }
        else
        {
          valueOrDefault = stain.GetValueOrDefault();
          ReadOnlySeString name = ((Stain) ref valueOrDefault).Name;
          str1 = ((ReadOnlySeString) ref name).ExtractText();
        }
        string str2 = str1;
        Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(!StringExtensions.IsNullOrEmpty(str2) ? str2 : "No dye set."));
        int num1;
        if (!stain.HasValue)
        {
          num1 = 0;
        }
        else
        {
          valueOrDefault = stain.GetValueOrDefault();
          num1 = (int) ((Stain) ref valueOrDefault).Color;
        }
        uint num2 = (uint) num1;
        if (num2 == 0U)
          return;
        ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
        Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
        ImU8String imU8String = new ImU8String(3, 1);
        ((ImU8String) ref imU8String).AppendLiteral("(#");
        ((ImU8String) ref imU8String).AppendFormatted<uint>(num2, "X6");
        ((ImU8String) ref imU8String).AppendLiteral(")");
        Dalamud.Bindings.ImGui.ImGui.TextDisabled(imU8String);
      }
    }
  }

  private void OpenDyeSelectPopup(EquipSlot slot, int index)
  {
    this.DyeSelectSlot = slot;
    this.DyeSelectIndex = index;
    this._dyeSelectPopup.Open();
  }

  private void DrawDyeSelectPopup()
  {
    if (!this._dyeSelectPopup.IsOpen)
      return;
    lock (this.Stains)
    {
      Stain selected;
      ItemInfo itemInfo;
      if (!this._dyeSelectPopup.Draw(this.Stains, out selected) || !this.Equipped.TryGetValue(this.DyeSelectSlot, out itemInfo))
        return;
      itemInfo.SetStainId((byte) ((Stain) ref selected).RowId, this.DyeSelectIndex);
    }
  }

  private static bool DyeSelectDrawRow(Stain stain, bool isFocus)
  {
    uint background = EquipmentEditorTab.CalcStainColor(new Stain?(stain));
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float num1 = ((ImGuiStylePtr) ref style).ItemSpacing.Y / 2f;
    ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
    Vector2 cursorScreenPos = Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos();
    cursorScreenPos.X -= ((ImGuiStylePtr) ref style).WindowPadding.X + num1;
    Vector2 vector2_1 = cursorScreenPos;
    Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
    ref Vector2 local = ref contentRegionAvail;
    ImFontPtr iconFont = UiBuilder.IconFont;
    double num2 = (double) ((ImFontPtr) ref iconFont).FontSize + (double) ((ImGuiStylePtr) ref style).FramePadding.Y + (double) num1;
    local.Y = (float) num2;
    Vector2 vector2_2 = contentRegionAvail;
    Vector2 vector2_3 = vector2_1 + vector2_2;
    ((ImDrawListPtr) ref windowDrawList).AddRectFilled(cursorScreenPos, vector2_3, background);
    using (ImRaii.PushColor((ImGuiCol) 0, GuiHelpers.CalcBlackWhiteTextColor(background), true))
    {
      using (ImRaii.PushColor((ImGuiCol) 26, background, true))
      {
        using (ImRaii.PushColor((ImGuiCol) 25, background, true))
        {
          string str;
          if (((Stain) ref stain).RowId != 0U)
          {
            ReadOnlySeString name = ((Stain) ref stain).Name;
            str = ((ReadOnlySeString) ref name).ExtractText();
          }
          else
            str = "None";
          return Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(str), isFocus, (ImGuiSelectableFlags) 0, new Vector2());
        }
      }
    }
  }

  private static bool DyeSelectSearchPredicate(Stain stain, string query)
  {
    ReadOnlySeString name = ((Stain) ref stain).Name;
    return ((ReadOnlySeString) ref name).ExtractText().Contains(query, StringComparison.OrdinalIgnoreCase);
  }

  private void DrawGlassesSelect(int index = 0)
  {
    ushort glassesId = this.Editor.GetGlassesId(index);
    Ktisis.GameData.Excel.Glasses? glasses;
    lock (this.Glasses)
      glasses = new Ktisis.GameData.Excel.Glasses?(this.Glasses.FirstOrDefault<Ktisis.GameData.Excel.Glasses>((Func<Ktisis.GameData.Excel.Glasses, bool>) (x => (int) x.RowId == (int) glassesId)));
    float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
    this.DrawGlassesButton(index, glasses);
    ImGuiStylePtr style1 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style1).ItemInnerSpacing.X);
    using (ImRaii.Group())
    {
      Ktisis.GameData.Excel.Glasses valueOrDefault;
      uint? nullable1;
      if (!glasses.HasValue)
      {
        nullable1 = new uint?();
      }
      else
      {
        valueOrDefault = glasses.GetValueOrDefault();
        nullable1 = new uint?(valueOrDefault.RowId);
      }
      uint? nullable2 = nullable1;
      string str;
      if (nullable2.HasValue && nullable2.GetValueOrDefault() == 0U)
      {
        str = "None";
      }
      else
      {
        valueOrDefault = glasses.Value;
        str = valueOrDefault.Name;
      }
      Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(str));
      double num1 = (double) EquipmentEditorTab.CalcItemWidth(cursorPosX);
      double frameHeight = (double) Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
      ImGuiStylePtr style2 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
      double x = (double) ((ImGuiStylePtr) ref style2).ItemInnerSpacing.X;
      double num2 = (frameHeight + x) * 2.0;
      Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth((float) (num1 + num2));
      int id = (int) glassesId;
      ImU8String imU8String = new ImU8String(10, 1);
      ((ImU8String) ref imU8String).AppendLiteral("##Glasses_");
      ((ImU8String) ref imU8String).AppendFormatted<int>(index);
      if (!Dalamud.Bindings.ImGui.ImGui.InputInt(imU8String, ref id, 0, 0, new ImU8String(), (ImGuiInputTextFlags) 0))
        return;
      this.Editor.SetGlassesId(index, (ushort) id);
    }
  }

  private void DrawGlassesButton(int index, Ktisis.GameData.Excel.Glasses? glasses)
  {
    using (ImRaii.PushColor((ImGuiCol) 21, 0U, true))
    {
      uint? icon = glasses?.Icon;
      uint num = !icon.HasValue || icon.GetValueOrDefault() == 0U ? EquipmentEditorTab.GetFallbackIcon(EquipSlot.Glasses) : glasses.Value.Icon;
      ITextureProvider tex = this._tex;
      GameIconLookup gameIconLookup = GameIconLookup.op_Implicit(num);
      ref GameIconLookup local = ref gameIconLookup;
      if (Dalamud.Bindings.ImGui.ImGui.ImageButton(tex.GetFromGameIcon(ref local).GetWrapOrEmpty().Handle, EquipmentEditorTab.ButtonSize))
        this.OpenGlassesSelectPopup(index);
      if (!Dalamud.Bindings.ImGui.ImGui.IsItemClicked((ImGuiMouseButton) 1))
        return;
      this.Editor.SetGlassesId(index, (ushort) 0);
    }
  }

  private static bool GlassesSelectSearchPredicate(Ktisis.GameData.Excel.Glasses glasses, string query)
  {
    return glasses.Name.Contains(query, StringComparison.OrdinalIgnoreCase);
  }

  private void OpenGlassesSelectPopup(int index)
  {
    this.GlassesSelectIndex = index;
    this._glassesSelectPopup.Open();
  }

  private void DrawGlassesSelectPopup()
  {
    if (!this._glassesSelectPopup.IsOpen)
      return;
    lock (this.Glasses)
    {
      Ktisis.GameData.Excel.Glasses selected;
      if (!this._glassesSelectPopup.Draw(this.Glasses, out selected))
        return;
      this.Editor.SetGlassesId(this.GlassesSelectIndex, (ushort) selected.RowId);
    }
  }

  private static bool GlassesSelectDrawRow(Ktisis.GameData.Excel.Glasses glasses, bool isFocus)
  {
    return Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(!StringExtensions.IsNullOrEmpty(glasses.Name) ? glasses.Name : "None"), isFocus, (ImGuiSelectableFlags) 0, new Vector2());
  }

  private void FetchData()
  {
    if (this._itemsRaii)
      return;
    this._itemsRaii = true;
    this.LoadItems().ContinueWith((Action<Task>) (task =>
    {
      if (task.Exception == null)
        return;
      Ktisis.Ktisis.Log.Error($"Failed to fetch items:\n{task.Exception}", Array.Empty<object>());
    }));
  }

  private async Task LoadItems()
  {
    await Task.Yield();
    IEnumerable<Stain> collection1 = ((IEnumerable<Stain>) this._data.Excel.GetSheet<Stain>(new Language?(), (string) null)).Where<Stain>((Func<Stain, bool>) (stain =>
    {
      if (((Stain) ref stain).RowId == 0U)
        return true;
      ReadOnlySeString name = ((Stain) ref stain).Name;
      return !((ReadOnlySeString) ref name).IsEmpty;
    }));
    lock (this.Stains)
      this.Stains.AddRange(collection1);
    foreach (ItemSheet[] itemSheetArray in Enumerable.Chunk<ItemSheet>(((IEnumerable<ItemSheet>) this._data.Excel.GetSheet<ItemSheet>(new Language?(), (string) null)).Where<ItemSheet>((Func<ItemSheet, bool>) (item => item.IsEquippable())), 1000))
    {
      lock (this.Items)
        this.Items.AddRange((IEnumerable<ItemSheet>) itemSheetArray);
      lock (this._equipUpdateLock)
      {
        foreach (KeyValuePair<EquipSlot, ItemInfo> keyValuePair in this.Equipped.Where<KeyValuePair<EquipSlot, ItemInfo>>((Func<KeyValuePair<EquipSlot, ItemInfo>, bool>) (pair => !pair.Value.Item.HasValue)))
        {
          EquipSlot equipSlot;
          ItemInfo itemInfo;
          keyValuePair.Deconstruct(ref equipSlot, ref itemInfo);
          EquipSlot slot = equipSlot;
          ItemInfo info = itemInfo;
          if (((IEnumerable<ItemSheet>) itemSheetArray).Any<ItemSheet>((Func<ItemSheet, bool>) (item => item.IsEquippable(slot) && info.IsItemPredicate(item))))
            info.FlagUpdate = true;
        }
      }
    }
    IEnumerable<Ktisis.GameData.Excel.Glasses> collection2 = ((IEnumerable<Ktisis.GameData.Excel.Glasses>) this._data.Excel.GetSheet<Ktisis.GameData.Excel.Glasses>(new Language?(), (string) null)).Where<Ktisis.GameData.Excel.Glasses>((Func<Ktisis.GameData.Excel.Glasses, bool>) (x => x.RowId == 0U || !StringExtensions.IsNullOrEmpty(x.Name)));
    lock (this.Glasses)
      this.Glasses.AddRange(collection2);
  }

  private void UpdateSlot(EquipSlot slot)
  {
    ItemInfo itemInfo1;
    if (this.Equipped.TryGetValue(slot, out itemInfo1) && !itemInfo1.FlagUpdate && itemInfo1.IsCurrent())
      return;
    ItemInfo itemInfo2;
    if (slot < EquipSlot.Head)
    {
      WeaponIndex index = (WeaponIndex) slot;
      WeaponModelId weaponIndex = this.Editor.GetWeaponIndex(index);
      itemInfo2 = (ItemInfo) new WeaponInfo(this.Editor)
      {
        Index = index,
        Model = weaponIndex
      };
    }
    else
    {
      EquipIndex equipIndex1 = slot.ToEquipIndex();
      EquipmentModelId equipIndex2 = this.Editor.GetEquipIndex(equipIndex1);
      itemInfo2 = (ItemInfo) new EquipInfo(this.Editor)
      {
        Index = equipIndex1,
        Model = equipIndex2
      };
    }
    try
    {
      lock (this.Items)
      {
        foreach (ItemSheet itemSheet in this.Items)
        {
          if (itemSheet.IsEquippable(slot) && itemInfo2.IsItemPredicate(itemSheet))
          {
            itemInfo2.Item = new ItemSheet?(itemSheet);
            break;
          }
        }
      }
      ItemInfo itemInfo3 = itemInfo2;
      GameIconLookup gameIconLookup;
      ISharedImmediateTexture immediateTexture1;
      if (!itemInfo2.Item.HasValue)
      {
        immediateTexture1 = (ISharedImmediateTexture) null;
      }
      else
      {
        ITextureProvider tex = this._tex;
        gameIconLookup = GameIconLookup.op_Implicit((uint) itemInfo2.Item.Value.Icon);
        ref GameIconLookup local = ref gameIconLookup;
        immediateTexture1 = tex.GetFromGameIcon(ref local);
      }
      itemInfo3.Texture = immediateTexture1;
      ItemInfo itemInfo4 = itemInfo2;
      if (itemInfo4.Texture != null)
        return;
      ItemInfo itemInfo5 = itemInfo4;
      ITextureProvider tex1 = this._tex;
      gameIconLookup = GameIconLookup.op_Implicit(EquipmentEditorTab.GetFallbackIcon(slot));
      ref GameIconLookup local1 = ref gameIconLookup;
      ISharedImmediateTexture fromGameIcon;
      ISharedImmediateTexture immediateTexture2 = fromGameIcon = tex1.GetFromGameIcon(ref local1);
      itemInfo5.Texture = fromGameIcon;
    }
    finally
    {
      this.Equipped[slot] = itemInfo2;
    }
  }

  private void InvalidateCache()
  {
    lock (this.Equipped)
      this.Equipped.Clear();
  }

  private static float CalcItemWidth(float cursorStart)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    float x = ((ImGuiStylePtr) ref style).ItemInnerSpacing.X;
    ImFontPtr iconFont = UiBuilder.IconFont;
    return Math.Min((float) ((double) ((ImFontPtr) ref iconFont).FontSize * 4.0 * 2.0) + x, Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() - cursorStart) - x - Dalamud.Bindings.ImGui.ImGui.GetFrameHeight());
  }

  private static uint GetFallbackIcon(EquipSlot slot)
  {
    uint fallbackIcon;
    switch (slot)
    {
      case EquipSlot.MainHand:
        fallbackIcon = 60102U;
        break;
      case EquipSlot.OffHand:
        fallbackIcon = 60110U;
        break;
      case EquipSlot.Head:
        fallbackIcon = 60124U;
        break;
      case EquipSlot.Chest:
        fallbackIcon = 60125U;
        break;
      case EquipSlot.Hands:
        fallbackIcon = 60129U;
        break;
      case EquipSlot.Legs:
        fallbackIcon = 60127U;
        break;
      case EquipSlot.Feet:
        fallbackIcon = 60130U;
        break;
      case EquipSlot.Earring:
        fallbackIcon = 60133U;
        break;
      case EquipSlot.Necklace:
        fallbackIcon = 60132U;
        break;
      case EquipSlot.Bracelet:
        fallbackIcon = 60134U;
        break;
      case EquipSlot.RingLeft:
      case EquipSlot.RingRight:
        fallbackIcon = 60135U;
        break;
      case EquipSlot.Glasses:
        fallbackIcon = 60189U;
        break;
      default:
        fallbackIcon = 0U;
        break;
    }
    return fallbackIcon;
  }
}
