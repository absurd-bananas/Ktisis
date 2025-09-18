// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.Select.NpcSelect
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GLib.Popups;

using Ktisis.Core.Attributes;
using Ktisis.GameData.Excel.Types;
using Ktisis.Localization;
using Ktisis.Services.Data;
using Ktisis.Structs.Characters;

namespace Ktisis.Interface.Components.Chara.Select;

[Transient]
public class NpcSelect {
	private readonly LocaleManager _locale;
	private readonly NpcService _npc;
	private readonly List<INpcBase> _npcList = new List<INpcBase>();
	private readonly PopupList<INpcBase> _popup;
	private NpcLoadState _npcLoadState;

	public NpcSelect(NpcService npc, LocaleManager locale) {
		this._npc = npc;
		this._locale = locale;
		// ISSUE: reference to a compiler-generated field
		// ISSUE: reference to a compiler-generated field
		this._popup = new PopupList<INpcBase>("##NpcImportPopup", this.DrawItem).WithSearch(
			NpcSelect.\u003C\u003EO.\u003C0\u003E__MatchQuery ?? (NpcSelect.\u003C\u003EO.\u003C0\u003E__MatchQuery = new PopupList<INpcBase>.SearchPredicate(MatchQuery)));
	}

	public INpcBase? Selected { get; set; }

	public event OnNpcSelected? OnSelected;

	public void Fetch() {
		if (this._npcLoadState == NpcLoadState.Success)
			return;
		this._npc.GetNpcList().ContinueWith((Action<Task<IEnumerable<INpcBase>>>)(task => {
			if (task.Exception != null) {
				Ktisis.Ktisis.Log.Error($"Failed to fetch NPC list:\n{task.Exception}", Array.Empty<object>());
				this._npcLoadState = NpcLoadState.Failed;
			} else {
				this._npcList.Clear();
				this._npcList.AddRange(task.Result);
				this._npcLoadState = NpcLoadState.Success;
			}
		}));
	}

	public void Draw() {
		switch (this._npcLoadState) {
			case NpcLoadState.Waiting:
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Loading NPCs..."));
				break;
			case NpcLoadState.Success:
				this.DrawSelect();
				break;
			case NpcLoadState.Failed:
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Failed to load NPCs.\nCheck your error log for more information."));
				break;
			default:
				throw new InvalidEnumArgumentException($"Invalid value: {this._npcLoadState}");
		}
	}

	private void DrawSelect() {
		var str = this.Selected != null ? this.Selected.Name : "Select...";
		if (Dalamud.Bindings.ImGui.ImGui.BeginCombo(ImU8String.op_Implicit("##NpcCombo"), ImU8String.op_Implicit(str), (ImGuiComboFlags)0)) {
			Dalamud.Bindings.ImGui.ImGui.CloseCurrentPopup();
			Dalamud.Bindings.ImGui.ImGui.EndCombo();
		}
		if (Dalamud.Bindings.ImGui.ImGui.IsItemActivated())
			this._popup.Open();
		var itemHeight = Dalamud.Bindings.ImGui.ImGui.GetFontSize() * 2f;
		INpcBase selected;
		if (!this._popup.Draw(this._npcList, out selected, itemHeight) || selected == null)
			return;
		this.Select(selected);
	}

	private void Select(INpcBase npc) {
		this.Selected = npc;
		var onSelected = this.OnSelected;
		if (onSelected == null)
			return;
		onSelected(npc);
	}

	private bool DrawItem(INpcBase npc, bool isFocus) {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		float fontSize = Dalamud.Bindings.ImGui.ImGui.GetFontSize();
		var num = Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit("##"), isFocus, (ImGuiSelectableFlags)0, VectorExtensions.WithY(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail(), fontSize * 2f)) ? 1 : 0;
		Dalamud.Bindings.ImGui.ImGui.SameLine(((ImGuiStylePtr) ref style).ItemInnerSpacing.X, 0.0f);
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(npc.Name));
		var modelId = npc.GetModelId();
		Dalamud.Bindings.ImGui.ImGui.SameLine(((ImGuiStylePtr) ref style).ItemInnerSpacing.X, 0.0f);
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(Dalamud.Bindings.ImGui.ImGui.GetCursorPosY() + fontSize);
		if (modelId == 0) {
			var customize = npc.GetCustomize();
			if (customize.HasValue && customize.Value.Tribe != 0) {
				var str1 = customize.Value.Gender == Gender.Masculine ? "♂" : "♀";
				var str2 = this._locale.Translate($"{customize.Value.Tribe}");
				ImU8String imU8String;
				// ISSUE: explicit constructor call
				((ImU8String) ref imU8String).\u002Ector(1, 2);
				((ImU8String) ref imU8String).AppendFormatted<string>(str1);
				((ImU8String) ref imU8String).AppendLiteral(" ");
				((ImU8String) ref imU8String).AppendFormatted<string>(str2);
				Dalamud.Bindings.ImGui.ImGui.TextDisabled(imU8String);
				return num != 0;
			}
			Dalamud.Bindings.ImGui.ImGui.TextDisabled(ImU8String.op_Implicit("Unknown"));
			return num != 0;
		}
		ImU8String imU8String1;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String1).\u002Ector(7, 1);
		((ImU8String) ref imU8String1).AppendLiteral("Model #");
		((ImU8String) ref imU8String1).AppendFormatted<ushort>(modelId);
		Dalamud.Bindings.ImGui.ImGui.TextDisabled(imU8String1);
		return num != 0;
	}

	private static bool MatchQuery(INpcBase npc, string query) => npc.Name.Contains(query, StringComparison.OrdinalIgnoreCase);

	private enum NpcLoadState {
		Waiting,
		Success,
		Failed
	}
}
