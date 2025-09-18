// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.FogEditor
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
public class FogEditor : EditorBase
{
  public override string Name { get; } = "Fog";

  public override bool IsActivated(EnvOverride flags) => flags.HasFlag((Enum) EnvOverride.Fog);

  public override void Draw(IEnvModule module, ref EnvState state)
  {
    this.DrawToggleCheckbox("Enable", EnvOverride.Fog, module);
    using (this.Disable(module))
    {
      Dalamud.Bindings.ImGui.ImGui.ColorEdit4(ImU8String.op_Implicit("Color"), ref state.Fog.Color, (ImGuiColorEditFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Distance"), ref state.Fog.Distance, 0.0f, 1000f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Thickness"), ref state.Fog.Thickness, 0.0f, 100f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Opacity"), ref state.Fog.Opacity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Sky Visibility"), ref state.Fog.SkyVisibility, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
    }
  }
}
