// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Config.ActionKeybindEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Actions;
using Ktisis.Actions.Binds;
using Ktisis.Actions.Types;
using Ktisis.Common.Utility;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config.Actions;
using Ktisis.Localization;

namespace Ktisis.Interface.Components.Config;

[Transient]
public class ActionKeybindEditor {
	private readonly static Vector2 CellPadding = new Vector2(8f, 8f);
	private readonly ActionService _actions;
	private readonly LocaleManager _locale;
	private readonly List<KeyAction> Actions = new List<KeyAction>();
	private readonly List<VirtualKey> KeysHandled = new List<VirtualKey>();
	private ActionKeybind? Editing;
	private KeyCombo? KeyCombo;

	public ActionKeybindEditor(ActionService actions, LocaleManager locale) {
		this._actions = actions;
		this._locale = locale;
	}

	public void Setup() {
		var bindable = this._actions.GetBindable();
		this.Actions.Clear();
		this.Actions.AddRange(bindable);
		this.SetEditing(null);
	}

	public void Draw() {
		using (ImRaii.PushStyle((ImGuiStyleVar)1, Vector2.Zero, true)) {
			using (ImRaii.IEndObject iendObject1 = ImRaii.Child(ImU8String.op_Implicit("##CfgStyleFrame"), Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail(), false)) {
				if (!iendObject1.Success)
					return;
				using (ImRaii.PushStyle((ImGuiStyleVar)16 /*0x10*/, Vector2.Zero, true)) {
					using (ImRaii.IEndObject iendObject2 = ImRaii.Table(ImU8String.op_Implicit("##KeyActionTable"), 2, (ImGuiTableFlags)1921)) {
						if (!iendObject2.Success)
							return;
						if (!Dalamud.Bindings.ImGui.ImGui.IsWindowFocused())
							this.SetEditing(null);
						Dalamud.Bindings.ImGui.ImGui.TableSetupColumn(ImU8String.op_Implicit("Keys"), (ImGuiTableColumnFlags)0, 0.0f, 0U);
						Dalamud.Bindings.ImGui.ImGui.TableSetupColumn(ImU8String.op_Implicit("Action"), (ImGuiTableColumnFlags)0, 0.0f, 0U);
						foreach (var action in this.Actions)
							this.DrawAction(action);
					}
				}
			}
		}
	}

	private void DrawAction(KeyAction action) {
		Dalamud.Bindings.ImGui.ImGui.TableNextRow();
		Dalamud.Bindings.ImGui.ImGui.TableNextColumn();
		this.DrawKeybind(action.GetKeybind());
		Dalamud.Bindings.ImGui.ImGui.TableNextColumn();
		var str = this._locale.Translate("actions." + action.GetName());
		Dalamud.Bindings.ImGui.ImGui.SetCursorPos(Dalamud.Bindings.ImGui.ImGui.GetCursorPos() + CellPadding);
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(str));
	}

	private void DrawKeybind(ActionKeybind keybind) {
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(8, 1);
		((ImU8String) ref imU8String).AppendLiteral("Keybind_");
		((ImU8String) ref imU8String).AppendFormatted<int>(keybind.GetHashCode(), "X");
		using (ImRaii.PushId(imU8String, true)) {
			using (ImRaii.PushStyle((ImGuiStyleVar)11, 0.0f, true)) {
				var flag = this.Editing == keybind;
				uint colorU32 = Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol)9);
				using (ImRaii.PushColor((ImGuiCol)21, flag ? colorU32 : 0U, true)) {
					using (ImRaii.PushColor((ImGuiCol)23, colorU32, true)) {
						using (ImRaii.PushColor((ImGuiCol)22, flag ? colorU32 : Dalamud.Bindings.ImGui.ImGui.GetColorU32((ImGuiCol)8), true)) {
							float columnWidth = Dalamud.Bindings.ImGui.ImGui.GetColumnWidth();
							Vector2 vector2_1 = new Vector2(columnWidth, Dalamud.Bindings.ImGui.ImGui.GetFrameHeightWithSpacing()) + CellPadding;
							if (flag) {
								ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
								Vector2 vector2_2 = ((ImGuiStylePtr) ref style ).ItemSpacing;
								Dalamud.Bindings.ImGui.ImGui.SetCursorPos(Dalamud.Bindings.ImGui.ImGui.GetCursorPos() + CellPadding);
								Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(columnWidth - CellPadding.X - vector2_2.X);
								this.EditKeybind(keybind);
								Dalamud.Bindings.ImGui.ImGui.Dummy(CellPadding - vector2_2);
							} else if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit(keybind.Combo.GetShortcutString()), vector2_1))
								this.SetEditing(keybind);
							if (Dalamud.Bindings.ImGui.ImGui.IsItemClicked((ImGuiMouseButton)1)) {
								keybind.Combo = new KeyCombo((VirtualKey)0);
							} else {
								if (this.Editing == null || this.Editing == keybind || !Dalamud.Bindings.ImGui.ImGui.IsItemFocused())
									return;
								this.SetEditing(null);
							}
						}
					}
				}
			}
		}
	}

	private void SetEditing(ActionKeybind? keybind) {
		this.FinishEdit();
		this.Editing = keybind;
		this.KeyCombo = null;
		this.KeysHandled.Clear();
	}

	private void FinishEdit() {
		if (this.Editing == null || this.KeyCombo == null)
			return;
		if (this.KeyCombo.Key != null)
			this.Editing.Combo = this.KeyCombo;
		Ktisis.Ktisis.Log.Info($"Applying edit ({this.KeyCombo.GetShortcutString()})", Array.Empty<object>());
	}

	private void EditKeybind(ActionKeybind keybind) {
		using (ImRaii.PushId(keybind.GetHashCode(), true)) {
			using (ImRaii.PushColor((ImGuiCol)49, 0U, true)) {
				if (this.KeyCombo == null)
					this.KeyCombo = new KeyCombo((VirtualKey)0);
				List<VirtualKey> list1 = KeyHelpers.GetKeysDown().ToList();
				List<VirtualKey> list2 = list1.Except((IEnumerable<VirtualKey>)this.KeysHandled).ToList();
				if (this.KeyCombo.Key != null && !list1.Contains(this.KeyCombo.Key)) {
					this.SetEditing(null);
				} else {
					this.KeysHandled.AddRange((IEnumerable<VirtualKey>)list2);
					foreach (VirtualKey key1 in list2) {
						if (key1 == 13) {
							this.SetEditing(null);
							return;
						}
						if (key1 == 8) {
							this.KeyCombo = null;
							this.SetEditing(null);
							return;
						}
						if (this.KeyCombo.Key == null)
							this.KeyCombo.Key = key1;
						else if (KeyHelpers.IsModifierKey(key1) && !KeyHelpers.IsModifierKey(this.KeyCombo.Key)) {
							this.KeyCombo.AddModifier(key1);
						} else {
							VirtualKey key2 = this.KeyCombo.Key;
							this.KeyCombo.Key = key1;
							this.KeyCombo.AddModifier(key2);
						}
					}
					var shortcutString = (this.KeysHandled.Count > 0 ? this.KeyCombo : keybind.Combo).GetShortcutString();
					Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("##EditKeybind"), ref shortcutString, 256 /*0x0100*/, (ImGuiInputTextFlags)16384 /*0x4000*/, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate)null);
					Dalamud.Bindings.ImGui.ImGui.SetKeyboardFocusHere(-1);
				}
			}
		}
	}
}
