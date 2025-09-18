// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.WindEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Common.Utility;
using Ktisis.Core.Attributes;
using Ktisis.Scene.Modules;
using Ktisis.Structs.Env;
using System;

#nullable enable
namespace Ktisis.Interface.Components.Environment.Editors;

[Transient]
public class WindEditor : EditorBase
{
  public override string Name { get; } = "Wind";

  public override bool IsActivated(EnvOverride flags) => flags.HasFlag((Enum) EnvOverride.Wind);

  public override void Draw(IEnvModule module, ref EnvState state)
  {
    this.DrawToggleCheckbox("Enable", EnvOverride.Wind, module);
    using (this.Disable(module))
    {
      this.DrawAngle("Direction", ref state.Wind.Direction, 0.0f, 360f);
      this.DrawAngle("Angle", ref state.Wind.Angle, 0.0f, 180f);
      Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("Speed"), ref state.Wind.Speed, 0.0f, 1.5f, new ImU8String(), (ImGuiSliderFlags) 0);
    }
  }

  private void DrawAngle(string label, ref float angle, float min, float max)
  {
    float num = angle * MathHelpers.Deg2Rad;
    if (!Dalamud.Bindings.ImGui.ImGui.SliderAngle(ImU8String.op_Implicit(label), ref num, min, max, new ImU8String(), (ImGuiSliderFlags) 0))
      return;
    angle = num * MathHelpers.Rad2Deg;
  }
}
