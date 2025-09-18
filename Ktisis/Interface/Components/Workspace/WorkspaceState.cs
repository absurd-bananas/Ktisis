// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Workspace.WorkspaceState
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;
using System.Linq;

using GLib.Widgets;

using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Interface.Widgets;

namespace Ktisis.Interface.Components.Workspace;

public class WorkspaceState {
	private readonly IEditorContext _ctx;

	public WorkspaceState(IEditorContext ctx) {
		this._ctx = ctx;
	}

	public void Draw() {
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var y = (float)(((double)Dalamud.Bindings.ImGui.ImGui.GetFontSize() + (double)((ImGuiStylePtr) ref style ).ItemInnerSpacing.Y) *2.0) +((ImGuiStylePtr) ref style).ItemSpacing.Y;
		var flag = false;
		try {
			flag = Dalamud.Bindings.ImGui.ImGui.BeginChildFrame(Dalamud.Bindings.ImGui.ImGui.GetID(ImU8String.op_Implicit("SceneState_Frame")), new Vector2(-1f, y));
			if (!flag)
				return;
			this.DrawContext();
			this.DrawOverlayToggle();
		} finally {
			if (flag)
				Dalamud.Bindings.ImGui.ImGui.EndChildFrame();
		}
	}

	private void DrawContext() {
		float cursorPosY = Dalamud.Bindings.ImGui.ImGui.GetCursorPosY();
		float y = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().Y;
		var cursorPosX = (double)Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
		ImGuiStylePtr style1 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var x = (double)((ImGuiStylePtr) ref style1 ).ItemSpacing.X;
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosX((float)(cursorPosX + x));
		Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(cursorPosY + (float)((y - (double)Dalamud.Bindings.ImGui.ImGui.GetFrameHeight()) / 2.0));
		var isEnabled = this._ctx.Posing.IsEnabled;
		var str = isEnabled ? "enable" : "disable";
		var circleColor = isEnabled ? 4282046570U : 4283453124U;
		using (ImRaii.PushColor((ImGuiCol)21, isEnabled ? 4278255360U /*0xFF00FF00*/ : 4285558976U, true)) {
			if (ToggleButton.Draw("##KtisisPoseToggle", ref isEnabled, circleColor))
				this._ctx.Posing.SetEnabled(isEnabled);
			if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered()) {
				using (ImRaii.Tooltip())
					Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("workspace.posing.hint." + str)));
			}
			Dalamud.Bindings.ImGui.ImGui.SameLine();
			ImGuiStylePtr style2 = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			ImFontPtr iconFont = UiBuilder.IconFont;
			float num = ((ImFontPtr) ref iconFont ).FontSize * 2f + ((ImGuiStylePtr) ref style2).ItemInnerSpacing.Y;
			Dalamud.Bindings.ImGui.ImGui.SetCursorPosY(cursorPosY + (float)((y - (double)num) / 2.0));
			Dalamud.Bindings.ImGui.ImGui.BeginGroup();
			using (ImRaii.PushStyle((ImGuiStyleVar)13, Vector2.Zero, true)) {
				using (ImRaii.PushColor((ImGuiCol)0, circleColor, true))
					Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("workspace.posing.toggle." + str)));
				using (ImRaii.PushColor((ImGuiCol)0, 3758096383U /*0xDFFFFFFF*/, true))
					this.DrawTargetLabel(this._ctx.Transform);
			}
			Dalamud.Bindings.ImGui.ImGui.EndGroup();
		}
	}

	private void DrawTargetLabel(ITransformHandler transform) {
		var target = transform.Target;
		if (target == null) {
			Dalamud.Bindings.ImGui.ImGui.TextDisabled(ImU8String.op_Implicit(this._ctx.Locale.Translate("workspace.state.select_count.none")));
		} else {
			var str = target.Primary?.Name ?? "INVALID";
			var num1 = transform.Target.Targets.Count();
			if (num1 == 1) {
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(str));
			} else {
				var num2 = num1 - 1;
				Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(this._ctx.Locale.Translate("workspace.state.select_count." + (num2 > 1 ? "plural" : "single"), new Dictionary<string, string> {
					{
						"count",
						num2.ToString()
					}, {
						"target",
						target.Primary?.Name ?? "INVALID"
					}
				})));
			}
		}
	}

	private void DrawOverlayToggle() {
		using (ImRaii.PushId(ImU8String.op_Implicit("##OverlayToggleButton"), true)) {
			using (ImRaii.PushColor((ImGuiCol)21, 0U, true)) {
				Dalamud.Bindings.ImGui.ImGui.SameLine();
				var visible = this._ctx.Config.Overlay.Visible;
				using (ImRaii.PushColor((ImGuiCol)0, visible ? 4026531839U /*0xEFFFFFFF*/ : 2164260863U, true)) {
					var icon = visible ? 61550 : 61552;
					var str = this._ctx.Locale.Translate("actions.Overlay_Toggle");
					Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
					var num = contentRegionAvail.Y - Dalamud.Bindings.ImGui.ImGui.GetCursorPosY() / 2f;
					Dalamud.Bindings.ImGui.ImGui.SetCursorPosX(Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() + contentRegionAvail.X - num);
					var tooltip = str;
					Vector2? size = new Vector2?(new Vector2(num, num));
					if (!Buttons.IconButtonTooltip((FontAwesomeIcon)icon, tooltip, size))
						return;
					this._ctx.Config.Overlay.Visible = !visible;
				}
			}
		}
	}
}
