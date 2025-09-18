// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.RainEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Core.Attributes;
using Ktisis.Scene.Modules;
using Ktisis.Structs.Env;

namespace Ktisis.Interface.Components.Environment.Editors;

[Transient]
public class RainEditor : EditorBase {
	public override string Name { get; } = "Rain";

	public override bool IsActivated(EnvOverride flags) => flags.HasFlag(EnvOverride.Rain);

	public override void Draw(IEnvModule module, ref EnvState state) {
		this.DrawToggleCheckbox("Enable", EnvOverride.Rain, module);
		using (this.Disable(module)) {
			Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Intensity"), ref state.Rain.Intensity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags)0);
			Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Thickness"), ref state.Rain.Size, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags)0);
			Dalamud.Bindings.ImGui.ImGui.ColorEdit4(ImU8String.op_Implicit("Color"), ref state.Rain.Color, (ImGuiColorEditFlags)0);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Weight"), ref state.Rain.Weight, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags)0);
			Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Scattering"), ref state.Rain.Scatter, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags)0);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Raindrops"), ref state.Rain.Raindrops, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags)0);
		}
	}
}
