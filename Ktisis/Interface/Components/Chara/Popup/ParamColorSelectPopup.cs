// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Chara.Popup.ParamColorSelectPopup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Linq;

using Ktisis.Editor.Characters.Types;

namespace Ktisis.Interface.Components.Chara.Popup;

public class ParamColorSelectPopup {
	private bool _isOpen;
	private bool _isOpening;
	private Vector4[] Colors = Array.Empty<Vector4>();
	private CustomizeIndex Index;
	private bool IsAlpha;

	private string PopupId => $"##ColorSelect_{this.GetHashCode():X}";

	public void Open(CustomizeIndex index, uint[] colors) {
		this._isOpening = true;
		this.Index = index;
		this.IsAlpha = colors.Length == 128 /*0x80*/;
		// ISSUE: reference to a compiler-generated field
		// ISSUE: reference to a compiler-generated field
		this.Colors = colors.Take(this.IsAlpha ? 96 /*0x60*/ : colors.Length).Select<uint, Vector4>(ParamColorSelectPopup.\u003C\u003EO.\u003C0\u003E__ColorConvertU32ToFloat4 ??
			(ParamColorSelectPopup.\u003C\u003EO.\u003C0\u003E__ColorConvertU32ToFloat4 = new Func<uint, Vector4>(Dalamud.Bindings.ImGui.ImGui.ColorConvertU32ToFloat4))).ToArray();
	}

	public void Draw(ICustomizeEditor editor) {
		if (this._isOpening) {
			this._isOpening = false;
			Dalamud.Bindings.ImGui.ImGui.OpenPopup(ImU8String.op_Implicit(this.PopupId), (ImGuiPopupFlags)0);
		}
		if (!Dalamud.Bindings.ImGui.ImGui.IsPopupOpen(ImU8String.op_Implicit(this.PopupId), (ImGuiPopupFlags)0))
			return;
		using (ImRaii.IEndObject iendObject = ImRaii.Popup(ImU8String.op_Implicit(this.PopupId), (ImGuiWindowFlags)64 /*0x40*/)) {
			if (!iendObject.Success) {
				if (!this._isOpen)
					return;
				this.OnClose();
			} else {
				this._isOpen = true;
				var customization = editor.GetCustomization(this.Index);
				if (this.IsAlpha) {
					this.DrawAlphaToggle(editor, customization);
					Dalamud.Bindings.ImGui.ImGui.Spacing();
				}
				this.DrawColorInput(editor, customization);
				Dalamud.Bindings.ImGui.ImGui.Spacing();
				this.DrawColorTable(editor, customization);
			}
		}
	}

	private void DrawColorInput(ICustomizeEditor editor, byte current) {
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetFrameHeight() * 8f);
		var num = current & (this.IsAlpha ? -129 : byte.MaxValue);
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(8, 1);
		((ImU8String) ref imU8String).AppendLiteral("##Input_");
		((ImU8String) ref imU8String).AppendFormatted<CustomizeIndex>(this.Index);
		if (!Dalamud.Bindings.ImGui.ImGui.InputInt(imU8String, ref num, 0, 0, new ImU8String(), (ImGuiInputTextFlags)0))
			return;
		this.SetColor(editor, current, (byte)num);
	}

	private void DrawColorTable(ICustomizeEditor editor, byte current) {
		using (ImRaii.PushStyle((ImGuiStyleVar)13, Vector2.Zero, true)) {
			using (ImRaii.PushStyle((ImGuiStyleVar)11, 0.0f, true)) {
				for (var index = 0; index < this.Colors.Length; ++index) {
					if (index % 8 != 0)
						Dalamud.Bindings.ImGui.ImGui.SameLine();
					Vector4 color = this.Colors[index];
					ImU8String imU8String = new ImU8String(2, 2);
					((ImU8String) ref imU8String).AppendFormatted<int>(index);
					((ImU8String) ref imU8String).AppendLiteral("##");
					((ImU8String) ref imU8String).AppendFormatted<CustomizeIndex>(this.Index);
					if (Dalamud.Bindings.ImGui.ImGui.ColorButton(imU8String, ref color, (ImGuiColorEditFlags)7536640 /*0x730000*/, new Vector2()))
						this.SetColor(editor, current, (byte)index);
				}
			}
		}
	}

	private void DrawAlphaToggle(ICustomizeEditor editor, byte current) {
		var flag = (current & 128U /*0x80*/) > 0U;
		if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Transparency"), ref flag))
			return;
		editor.SetCustomization(this.Index, (byte)(current ^ 128U /*0x80*/));
	}

	private void SetColor(ICustomizeEditor editor, byte current, byte value) {
		if (this.IsAlpha)
			value |= (byte)(current & 128U /*0x80*/);
		if (this.Index == 9)
			editor.SetEyeColor(value);
		else
			editor.SetCustomization(this.Index, value);
	}

	private void OnClose() => this.Colors = Array.Empty<Vector4>();
}
