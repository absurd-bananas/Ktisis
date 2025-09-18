// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Popup.EntityRenameModal
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Interface.Types;
using Ktisis.Scene.Entities;

namespace Ktisis.Interface.Editor.Popup;

public class EntityRenameModal : KtisisPopup {
	private bool _isFirstDraw;
	private string Name;

	public EntityRenameModal(SceneEntity entity) {
		// ISSUE: reference to a compiler-generated field
		this.\u003Centity\u003EP = entity;
		this._isFirstDraw = true;
		// ISSUE: reference to a compiler-generated field
		this.Name = this.\u003Centity\u003EP.Name;
		// ISSUE: explicit constructor call
		base.\u002Ector("##EntityRename", (ImGuiWindowFlags)134217728 /*0x08000000*/);
	}

	protected override void OnDraw() {
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(10, 1);
		((ImU8String) ref imU8String).AppendLiteral("Rename '");
		// ISSUE: reference to a compiler-generated field
		((ImU8String) ref imU8String).AppendFormatted<string>(this.\u003Centity\u003EP.Name);
		((ImU8String) ref imU8String).AppendLiteral("':");
		Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
		Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("##NameInput"), ref this.Name, 100, (ImGuiInputTextFlags)0, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate)null);
		var num = this.Name.Length > 0 ? 1 : 0;
		if (num != 0 && Dalamud.Bindings.ImGui.ImGui.IsKeyPressed((ImGuiKey)525) && Dalamud.Bindings.ImGui.ImGui.IsItemDeactivated())
			this.Confirm();
		if (this._isFirstDraw) {
			this._isFirstDraw = false;
			Dalamud.Bindings.ImGui.ImGui.SetKeyboardFocusHere(-1);
		}
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		using (ImRaii.Disabled(num == 0)) {
			if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Confirm"), new Vector2()))
				this.Confirm();
		}
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Cancel"), new Vector2()))
			return;
		this.Close();
	}

	private void Confirm() {
		// ISSUE: reference to a compiler-generated field
		this.\u003Centity\u003EP.Name = this.Name;
		this.Close();
	}
}
