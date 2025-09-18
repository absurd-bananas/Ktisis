// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.Editors.LightWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Linq;

using Ktisis.Editor.Context.Types;
using Ktisis.Interface.Types;
using Ktisis.Localization;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.World;
using Ktisis.Structs.Lights;

namespace Ktisis.Interface.Windows.Editors;

public class LightWindow : EntityEditWindow<LightEntity> {
	private readonly IEditorContext _ctx;
	private readonly LocaleManager _locale;

	public LightWindow(IEditorContext ctx, LocaleManager locale)
		: base("Light Editor", ctx) {
		this._ctx = ctx;
		this._locale = locale;
	}

	public override void PreDraw() {
		base.PreDraw();
		Window.WindowSizeConstraints windowSizeConstraints;
		// ISSUE: explicit constructor call
		((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
		((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = new Vector2(400f, 300f);
		ref Window.WindowSizeConstraints local = ref windowSizeConstraints;
		ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
		Vector2 vector2 = ((ImGuiIOPtr) ref io ).DisplaySize * 0.9f;
		((Window.WindowSizeConstraints) ref local).MaximumSize = vector2;
		this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
	}

	public virtual void Draw() {
		this.UpdateTarget();
		var selection = this.Context.Selection;
		if (selection.Count == 1 && selection.GetSelected().First() is LightEntity target1)
			this.SetTarget(target1);
		var target2 = this.Target;
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(1, 1);
		((ImU8String) ref imU8String).AppendFormatted<string>(target2.Name);
		((ImU8String) ref imU8String).AppendLiteral(":");
		Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		using (ImRaii.TabBar(ImU8String.op_Implicit("##LightEditTabs"))) {
			this.DrawTab("Light", this.DrawLightTab, target2);
			this.DrawTab("Shadows", this.DrawShadowsTab, target2);
			this.DrawImportExport(target2);
		}
	}

	private void DrawImportExport(LightEntity entity1) {
		if (entity1 == null)
			return;
		if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Import"), new Vector2()))
			this._ctx.Interface.OpenLightImport(entity1);
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Export"), new Vector2()))
			return;
		var flag = false;
		if (!this._ctx.Config.File.ExportLightIgnoreNoActorSelectedWarning) {
			var source = this._ctx.Scene.Children.Where(entity2 => entity2 is ActorEntity).Cast<ActorEntity>();
			if (source.Count() > 0 && source.FirstOrDefault(x => x.IsSelected) == null && source.Count() > 1)
				flag = true;
		}
		if (flag)
			this._ctx.Interface.OpenLightExportNoActorSelected(() => this._ctx.Interface.OpenLightExport(entity1), this._ctx.Config);
		else
			this._ctx.Interface.OpenLightExport(entity1);
	}

	private void DrawTab(string label, Action<LightEntity> draw, LightEntity entity) {
		using (ImRaii.IEndObject iendObject = ImRaii.TabItem(ImU8String.op_Implicit(label))) {
			if (!iendObject.Success)
				return;
			draw(entity);
		}
	}

	private unsafe void DrawLightTab(LightEntity entity) {
		var sceneLightPtr = entity.GetObject();
		RenderLight* renderLight = (IntPtr)sceneLightPtr != IntPtr.Zero ? sceneLightPtr->RenderLight : (RenderLight*)null;
		if ((IntPtr)renderLight == IntPtr.Zero)
			return;
		Dalamud.Bindings.ImGui.ImGui.Spacing();
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
		Dalamud.Bindings.ImGui.ImGui.Spacing();
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
