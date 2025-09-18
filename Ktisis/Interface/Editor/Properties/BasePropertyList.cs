// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Properties.BasePropertyList
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Interface.Editor.Properties.Types;
using Ktisis.Scene.Entities;
using System;

#nullable enable
namespace Ktisis.Interface.Editor.Properties;

public class BasePropertyList : ObjectPropertyList
{
  public override void Invoke(IPropertyListBuilder builder, SceneEntity entity)
  {
    builder.AddHeader("General", (Action) (() => this.DrawTab(entity)));
  }

  private void DrawTab(SceneEntity entity)
  {
    string name = entity.Name;
    if (!Dalamud.Bindings.ImGui.ImGui.InputText(ImU8String.op_Implicit("Name"), ref name, 100, (ImGuiInputTextFlags) 0, (Dalamud.Bindings.ImGui.ImGui.ImGuiInputTextCallbackDelegate) null))
      return;
    entity.Name = name;
  }
}
