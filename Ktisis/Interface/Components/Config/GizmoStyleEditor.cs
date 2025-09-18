// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Config.GizmoStyleEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using GLib.Widgets;

using Ktisis.Core.Attributes;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Sections;
using Ktisis.Localization;

namespace Ktisis.Interface.Components.Config;

[Transient]
public class GizmoStyleEditor {
	private readonly ConfigManager _cfg;
	private readonly LocaleManager _locale;

	public GizmoStyleEditor(ConfigManager cfg, LocaleManager locale) {
		this._cfg = cfg;
		this._locale = locale;
	}

	private Configuration Config => this._cfg.File;

	public void Draw() {
		var defaultStyle = GizmoConfig.DefaultStyle;
		var style = this.Config.Gizmo.Style;
		using (ImRaii.Child(ImU8String.op_Implicit("##CfgStyleFrame"), Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail(), true)) {
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._locale.Translate("config.gizmo.editor.general.title")), (ImGuiTreeNodeFlags)0)) {
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.general.dir_x"), ref style.ColorDirectionX, defaultStyle.ColorDirectionX);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.general.dir_y"), ref style.ColorDirectionY, defaultStyle.ColorDirectionY);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.general.dir_z"), ref style.ColorDirectionZ, defaultStyle.ColorDirectionZ);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.general.active"), ref style.ColorSelection, defaultStyle.ColorSelection);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.general.inactive"), ref style.ColorInactive, defaultStyle.ColorInactive);
			}
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._locale.Translate("config.gizmo.editor.position.title")), (ImGuiTreeNodeFlags)0)) {
				DrawStyleFloat("Gizmo Scale##posGizmoScale", ref this.Config.Gizmo.PositionGizmoScale, 0.1f, 0.05f, 0.5f);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.position.line_thick") + "##PosThickness", ref style.TranslationLineThickness, defaultStyle.TranslationLineThickness);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.position.arrow_size") + "##PosArrowSize", ref style.TranslationLineArrowSize, defaultStyle.TranslationLineArrowSize);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.position.axis_thick"), ref style.HatchedAxisLineThickness, defaultStyle.HatchedAxisLineThickness);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.position.circle_size") + "##PosCircleSize", ref style.CenterCircleSize, defaultStyle.CenterCircleSize);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.position.plane_x") + "##PosPlaneColorX", ref style.ColorPlaneX, defaultStyle.ColorPlaneX);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.position.plane_y") + "##PosPlaneColorY", ref style.ColorPlaneY, defaultStyle.ColorPlaneY);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.position.plane_z") + "##PosPlaneColorZ", ref style.ColorPlaneZ, defaultStyle.ColorPlaneZ);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.position.line_color") + "##PosLineColor", ref style.ColorTranslationLine, defaultStyle.ColorTranslationLine);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.position.axis_color"), ref style.ColorHatchedAxisLines, defaultStyle.ColorHatchedAxisLines);
			}
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._locale.Translate("config.gizmo.editor.rotation.title")), (ImGuiTreeNodeFlags)0)) {
				DrawStyleFloat("Gizmo Scale##rotGizmoScale", ref this.Config.Gizmo.RotationGizmoScale, 0.1f, 0.05f, 0.5f);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.rotation.inner_thick") + "##RotateThickness", ref style.RotationLineThickness, defaultStyle.RotationLineThickness);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.rotation.outer_thick") + "##RotateThicknessOuter", ref style.RotationOuterLineThickness, defaultStyle.RotationOuterLineThickness);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.rotation.border_color") + "##RotateUsingBorder", ref style.ColorRotationUsingBorder, defaultStyle.ColorRotationUsingBorder);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.rotation.fill_color") + "##RotateUsingFill", ref style.ColorRotationUsingFill, defaultStyle.ColorRotationUsingFill);
			}
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._locale.Translate("config.gizmo.editor.scale.title")), (ImGuiTreeNodeFlags)0)) {
				DrawStyleFloat("Gizmo Scale##scaGizmoScale", ref this.Config.Gizmo.ScaleGizmoScale, 0.1f, 0.05f, 0.5f);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.scale.line_thick") + "##ScaleThickness", ref style.ScaleLineThickness, defaultStyle.ScaleLineThickness);
				DrawStyleFloat(this._locale.Translate("config.gizmo.editor.scale.circle_size") + "##ScaleSize", ref style.ScaleLineCircleSize, defaultStyle.ScaleLineCircleSize);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.scale.line_color") + "##ScaleColor", ref style.ColorScaleLine, defaultStyle.ColorScaleLine);
			}
			if (Dalamud.Bindings.ImGui.ImGui.CollapsingHeader(ImU8String.op_Implicit(this._locale.Translate("config.gizmo.editor.text.title")), (ImGuiTreeNodeFlags)0)) {
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.text.color"), ref style.ColorText, defaultStyle.ColorText);
				GizmoStyleEditor.DrawStyleColor(this._locale.Translate("config.gizmo.editor.text.shadow_color"), ref style.ColorTextShadow, defaultStyle.ColorTextShadow);
			}
			this.Config.Gizmo.Style = style;
		}
	}

	private static void DrawStyleColor(string label, ref Vector4 value, Vector4 defaultValue) {
		float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(13, 1);
		((ImU8String) ref imU8String).AppendLiteral("##StyleFloat_");
		((ImU8String) ref imU8String).AppendFormatted<string>(label);
		using (ImRaii.PushId(imU8String, true)) {
			using (ImRaii.Disabled(value.Equals(defaultValue))) {
				if (Buttons.IconButtonTooltip((FontAwesomeIcon)61666, "Reset to default"))
					value = defaultValue;
			}
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() - cursorPosX));
			Dalamud.Bindings.ImGui.ImGui.ColorEdit4(ImU8String.op_Implicit(label), ref value, (ImGuiColorEditFlags)0);
		}
	}

	private static void DrawStyleFloat(
		string label,
		ref float value,
		float defaultValue,
		float minValue = -1f,
		float maxValue = -1f
	) {
		float cursorPosX = Dalamud.Bindings.ImGui.ImGui.GetCursorPosX();
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(13, 1);
		((ImU8String) ref imU8String).AppendLiteral("##StyleFloat_");
		((ImU8String) ref imU8String).AppendFormatted<string>(label);
		using (ImRaii.PushId(imU8String, true)) {
			using (ImRaii.Disabled(value.Equals(defaultValue))) {
				if (Buttons.IconButtonTooltip((FontAwesomeIcon)61666, "Reset to default"))
					value = defaultValue;
			}
			ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
			Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
			Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() - (Dalamud.Bindings.ImGui.ImGui.GetCursorPosX() - cursorPosX));
			if (minValue == -1.0 && maxValue == -1.0)
				Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit(label), ref value, 0.01f, 0.0f, 0.0f, new ImU8String(), (ImGuiSliderFlags)0);
			else
				Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit(label), ref value, 0.01f, minValue, maxValue, new ImU8String(), (ImGuiSliderFlags)0);
		}
	}
}
