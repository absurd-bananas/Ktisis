// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.LightingEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Core.Attributes;
using Ktisis.Scene.Modules;
using Ktisis.Structs.Env;

namespace Ktisis.Interface.Components.Environment.Editors;

[Transient]
public class LightingEditor : EditorBase {
	public override string Name { get; } = "Light";

	public override bool IsActivated(EnvOverride flags) => flags.HasFlag(EnvOverride.Lighting);

	public override void Draw(IEnvModule module, ref EnvState state) {
		this.DrawToggleCheckbox("Enable", EnvOverride.Lighting, module);
		using (this.Disable(module)) {
			using (ImRaii.Disabled(!module.Override.HasFlag(EnvOverride.Lighting))) {
				Dalamud.Bindings.ImGui.ImGui.ColorEdit3(ImU8String.op_Implicit("Sunlight"), ref state.Lighting.SunLightColor, (ImGuiColorEditFlags)0);
				Dalamud.Bindings.ImGui.ImGui.ColorEdit3(ImU8String.op_Implicit("Moonlight"), ref state.Lighting.MoonLightColor, (ImGuiColorEditFlags)0);
				Dalamud.Bindings.ImGui.ImGui.ColorEdit3(ImU8String.op_Implicit("Ambient"), ref state.Lighting.Ambient, (ImGuiColorEditFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Unknown #1"), ref state.Lighting._unk1, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Saturation"), ref state.Lighting.AmbientSaturation, 0.0f, 5f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Temperature"), ref state.Lighting.Temperature, -2.5f, 2.5f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Unknown #2"), ref state.Lighting._unk2, 0.0f, 100f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Unknown #3"), ref state.Lighting._unk3, 0.0f, 100f, new ImU8String(), (ImGuiSliderFlags)0);
				Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Unknown #4"), ref state.Lighting._unk4, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags)0);
			}
		}
	}
}
