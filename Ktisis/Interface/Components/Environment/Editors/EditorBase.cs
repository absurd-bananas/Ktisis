// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Components.Environment.Editors.EditorBase
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Ktisis.Scene.Modules;
using Ktisis.Structs.Env;
using System;

#nullable enable
namespace Ktisis.Interface.Components.Environment.Editors;

public abstract class EditorBase
{
  public abstract string Name { get; }

  public abstract bool IsActivated(EnvOverride flags);

  public abstract void Draw(IEnvModule module, ref EnvState state);

  protected ImRaii.IEndObject Disable(IEnvModule module)
  {
    return ImRaii.Disabled(!this.IsActivated(module.Override));
  }

  protected void DrawToggleCheckbox(string label, EnvOverride flag, IEnvModule module)
  {
    bool flag1 = module.Override.HasFlag((Enum) flag);
    if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit(label), ref flag1))
      module.Override ^= flag;
    Dalamud.Bindings.ImGui.ImGui.Spacing();
  }
}
