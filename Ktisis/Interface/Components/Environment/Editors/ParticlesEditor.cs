// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.ParticlesEditor
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
public class ParticlesEditor : EditorBase
{
  private readonly SetTextureSelect _texDust;

  public override string Name { get; } = "Particles";

  public ParticlesEditor(SetTextureSelect texDust) => this._texDust = texDust;

  public override bool IsActivated(EnvOverride flags) => flags.HasFlag((Enum) EnvOverride.Dust);

  public override void Draw(IEnvModule module, ref EnvState state)
  {
    this.DrawToggleCheckbox("Enable", EnvOverride.Dust, module);
    using (this.Disable(module))
    {
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Intensity"), ref state.Dust.Intensity, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Size"), ref state.Dust.Size, 0.0f, 20f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Glow"), ref state.Dust.Glow, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.ColorEdit4(ImU8String.op_Implicit("Color"), ref state.Dust.Color, (ImGuiColorEditFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Weight"), ref state.Dust.Weight, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Spread"), ref state.Dust.Spread, 0.0f, 10f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Speed"), ref state.Dust.Speed, 0.0f, 1f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Spin"), ref state.Dust.Spin, 0.05f, 5f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.Spacing();
      this._texDust.Draw("Texture", ref state.Dust.TextureId, new SetTextureSelect.ResolvePathHandler(this.ResolvePath));
    }
  }

  private string ResolvePath(uint id)
  {
    string str;
    if (id == 1U)
      str = "bgcommon/nature/snow/texture/snow.tex";
    else
      str = $"bgcommon/nature/dust/texture/dust_{Math.Max(0U, id - 2U):D3}.tex";
    return str;
  }
}
