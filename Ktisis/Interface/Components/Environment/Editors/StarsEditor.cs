// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.StarsEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Core.Attributes;
using Ktisis.Scene.Modules;
using Ktisis.Structs.Env;
using System;

#nullable enable
namespace Ktisis.Interface.Components.Environment.Editors;

[Transient]
public class StarsEditor : EditorBase
{
  public override string Name { get; } = "Stars";

  public override bool IsActivated(EnvOverride flags) => flags.HasFlag((Enum) EnvOverride.Stars);

  public override void Draw(IEnvModule module, ref EnvState state)
  {
    this.DrawToggleCheckbox("Enable", EnvOverride.Stars, module);
    using (this.Disable(module))
    {
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Stars"), ref state.Stars.Stars, 0.0f, 20f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Intensity##1"), ref state.Stars.StarIntensity, 0.0f, 2.5f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Constellations"), ref state.Stars.Constellations, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Intensity##2"), ref state.Stars.ConstellationIntensity, 0.0f, 2.5f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Galaxy Intensity"), ref state.Stars.GalaxyIntensity, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.ColorEdit4(ImU8String.op_Implicit("Moon Color"), ref state.Stars.MoonColor, (ImGuiColorEditFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Moon Brightness"), ref state.Stars.MoonBrightness, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    }
  }
}
