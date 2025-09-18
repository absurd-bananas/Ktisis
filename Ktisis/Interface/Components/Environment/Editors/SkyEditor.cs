// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.SkyEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Ktisis.Core.Attributes;
using Ktisis.Scene.Modules;
using Ktisis.Structs.Env;
using System;

#nullable enable
namespace Ktisis.Interface.Components.Environment.Editors;

[Transient]
public class SkyEditor : EditorBase
{
  private readonly SetTextureSelect _texSky;
  private readonly SetTextureSelect _texCloudTop;
  private readonly SetTextureSelect _texCloudSide;

  public override string Name { get; } = "Sky";

  public SkyEditor(
    SetTextureSelect texSky,
    SetTextureSelect texCloudTop,
    SetTextureSelect texCloudSide)
  {
    this._texSky = texSky;
    this._texCloudTop = texCloudTop;
    this._texCloudSide = texCloudSide;
  }

  public override bool IsActivated(EnvOverride flags)
  {
    return flags.HasFlag((Enum) EnvOverride.SkyId) || flags.HasFlag((Enum) EnvOverride.Clouds);
  }

  public override void Draw(IEnvModule module, ref EnvState state)
  {
    this.DrawToggleCheckbox("Edit skybox", EnvOverride.SkyId, module);
    using (ImRaii.Disabled(!module.Override.HasFlag((Enum) EnvOverride.SkyId)))
      this._texSky.Draw("Sky Texture", ref state.SkyId, (SetTextureSelect.ResolvePathHandler) (id => $"bgcommon/nature/sky/texture/sky_{id:D3}.tex"));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    this.DrawToggleCheckbox("Edit clouds", EnvOverride.Clouds, module);
    using (ImRaii.Disabled(!module.Override.HasFlag((Enum) EnvOverride.Clouds)))
    {
      this._texCloudTop.Draw("Top Clouds", ref state.Clouds.CloudTexture, (SetTextureSelect.ResolvePathHandler) (id => $"bgcommon/nature/cloud/texture/cloud_{id:D3}.tex"));
      this._texCloudSide.Draw("Side Clouds", ref state.Clouds.CloudSideTexture, (SetTextureSelect.ResolvePathHandler) (id => $"bgcommon/nature/cloud/texture/cloudside_{id:D3}.tex"));
      Dalamud.Bindings.ImGui.ImGui.ColorEdit3(ImU8String.op_Implicit("Cloud Color"), ref state.Clouds.CloudColor, (ImGuiColorEditFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.ColorEdit3(ImU8String.op_Implicit("Shadow Color"), ref state.Clouds.Color2, (ImGuiColorEditFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Shadows"), ref state.Clouds.Gradient, 0.0f, 2f, new ImU8String(), (ImGuiSliderFlags) 0);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Side Height"), ref state.Clouds.SideHeight, 0.0f, 2f, new ImU8String(), (ImGuiSliderFlags) 0);
    }
  }
}
