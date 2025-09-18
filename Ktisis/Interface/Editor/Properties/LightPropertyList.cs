// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.LightPropertyList
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Localization;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.World;
using Ktisis.Structs.Lights;

namespace Ktisis.Interface.Editor.Properties;

public class LightPropertyList : ObjectPropertyList {
	private readonly LocaleManager _locale;

	public LightPropertyList(LocaleManager locale) {
		this._locale = locale;
	}

	public override void Invoke(IPropertyListBuilder builder, SceneEntity entity) {
		var light = entity as LightEntity;
		if (light == null)
			return;
		builder.AddHeader("Light", (Action)(() => this.DrawLightTab(light)));
		builder.AddHeader("Shadows", (Action)(() => this.DrawShadowsTab(light)));
	}

	private unsafe void DrawLightTab(LightEntity entity) {
		var sceneLightPtr = entity.GetObject();
		RenderLight* renderLight = (IntPtr)sceneLightPtr != IntPtr.Zero ? sceneLightPtr->RenderLight : (RenderLight*)null;
		if ((IntPtr)renderLight == IntPtr.Zero)
			return;
		this.DrawLightFlag("Enable reflections", renderLight, LightFlags.Reflection);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		var str1 = this._locale.Translate($"lightType.{renderLight->LightType}");
		if (Dalamud.Bindings.ImGui.ImGui.BeginCombo(ImU8String.op_Implicit("Light Type"), ImU8String.op_Implicit(str1), (ImGuiComboFlags)0)) {
			foreach (LightType lightType in Enum.GetValues<LightType>()) {
				if (Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(this._locale.Translate($"lightType.{lightType}")), renderLight->LightType == lightType, (ImGuiSelectableFlags)0, new Vector2()))
					renderLight->LightType = lightType;
			}
			Dalamud.Bindings.ImGui.ImGui.EndCombo();
		}
		switch (renderLight->LightType) {
			case LightType.SpotLight:
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Cone Angle##LightAngle"), ref renderLight->LightAngle, 0.0f, 180f, ImU8String.op_Implicit("%0.0f deg"), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Falloff Angle##LightAngle"), ref renderLight->FalloffAngle, 0.0f, 180f, ImU8String.op_Implicit("%0.0f deg"), (ImGuiSliderFlags)0);
				break;
			case LightType.AreaLight:
				ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
				float x = ((ImGuiStylePtr) ref style ).ItemInnerSpacing.X;
				Dalamud.Bindings.ImGui.ImGui.PushItemWidth(Dalamud.Bindings.ImGui.ImGui.CalcItemWidth() / 2f - x);
				Dalamud.Bindings.ImGui.ImGui.SliderAngle(ImU8String.op_Implicit("##AngleX"), ref renderLight->AreaAngle.X, -90f, 90f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, x);
				Dalamud.Bindings.ImGui.ImGui.SliderAngle(ImU8String.op_Implicit("Light Angle##AngleY"), ref renderLight->AreaAngle.Y, -90f, 90f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.PopItemWidth();
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Falloff Angle##LightAngle"), ref renderLight->FalloffAngle, 0.0f, 180f, ImU8String.op_Implicit("%0.0f deg"), (ImGuiSliderFlags)0);
				break;
		}
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		var str2 = this._locale.Translate($"lightFalloff.{renderLight->FalloffType}");
		if (Dalamud.Bindings.ImGui.ImGui.BeginCombo(ImU8String.op_Implicit("Falloff Type"), ImU8String.op_Implicit(str2), (ImGuiComboFlags)0)) {
			foreach (FalloffType falloffType in Enum.GetValues<FalloffType>()) {
				if (Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(this._locale.Translate($"lightFalloff.{falloffType}")), renderLight->FalloffType == falloffType, (ImGuiSelectableFlags)0, new Vector2()))
					renderLight->FalloffType = falloffType;
			}
			Dalamud.Bindings.ImGui.ImGui.EndCombo();
		}
		Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Falloff Power##FalloffPower"), ref renderLight->Falloff, 0.01f, 0.0f, 1000f, new ImU8String(), (ImGuiSliderFlags)0);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Vector3 rgb = renderLight->Color.RGB;
		if (Dalamud.Bindings.ImGui.ImGui.ColorEdit3(ImU8String.op_Implicit("Color"), ref rgb, (ImGuiColorEditFlags)8912896 /*0x880000*/))
			renderLight->Color.RGB = rgb;
		Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Intensity"), ref renderLight->Color.Intensity, 0.01f, 0.0f, 100f, new ImU8String(), (ImGuiSliderFlags)0);
		if (!Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Range##LightRange"), ref renderLight->Range, 0.1f, 0.0f, 999f, new ImU8String(), (ImGuiSliderFlags)0))
			return;
		entity.Flags |= LightEntityFlags.Update;
	}

	private unsafe void DrawShadowsTab(LightEntity entity) {
		var sceneLightPtr = entity.GetObject();
		RenderLight* renderLight = (IntPtr)sceneLightPtr != IntPtr.Zero ? sceneLightPtr->RenderLight : (RenderLight*)null;
		if ((IntPtr)renderLight == IntPtr.Zero)
			return;
		this.DrawLightFlag("Dynamic shadows", renderLight, LightFlags.Dynamic);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawLightFlag("Cast character shadows", renderLight, LightFlags.CharaShadow);
		this.DrawLightFlag("Cast object shadows", renderLight, LightFlags.ObjectShadow);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Shadow Range"), ref renderLight->CharaShadowRange, 0.1f, 0.0f, 1000f, new ImU8String(), (ImGuiSliderFlags)0);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Shadow Near"), ref renderLight->ShadowNear, 0.01f, 0.0f, 1000f, new ImU8String(), (ImGuiSliderFlags)0);
		Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("Shadow Far"), ref renderLight->ShadowFar, 0.01f, 0.0f, 1000f, new ImU8String(), (ImGuiSliderFlags)0);
	}

	private unsafe void DrawLightFlag(string label, RenderLight* light, LightFlags flag) {
		var flag1 = light->Flags.HasFlag(flag);
		if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(label), ref flag1))
			return;
		light->Flags ^= flag;
	}
}
