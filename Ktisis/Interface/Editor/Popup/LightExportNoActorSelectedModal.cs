// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Editor.Popup.LightExportNoActorSelectedModal
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Ktisis.Data.Config;
using Ktisis.Interface.Types;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Editor.Popup;

public class LightExportNoActorSelectedModal(Action exportLight, Configuration config) : KtisisPopup("##LightExportNoActorSelected", (ImGuiWindowFlags) 134217728 /*0x08000000*/)
{
  private bool disableWarning;

  protected override void OnDraw()
  {
    Vector4 dalamudOrange = ImGuiColors.DalamudOrange;
    ref Vector4 local = ref dalamudOrange;
    ImU8String imU8String1;
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String1).\u002Ector(40, 0);
    ((ImU8String) ref imU8String1).AppendLiteral("Light Export Warning: No actor selected.");
    ImU8String imU8String2 = imU8String1;
    Dalamud.Bindings.ImGui.ImGui.TextColored(ref local, imU8String2);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    // ISSUE: explicit constructor call
    ((ImU8String) ref imU8String1).\u002Ector(82, 0);
    ((ImU8String) ref imU8String1).AppendLiteral("This may produce undesirable results if you later import this light with position.");
    Dalamud.Bindings.ImGui.ImGui.Text(imU8String1);
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("• A light's position is exported as an offset from an actor's position."));
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("• If you continue without selection, the 'primary' actor will be selected instead."));
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("• The 'primary' actor is usually your own character."));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Separator();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Click Confirm to open the Export window, you can still select an actor while the Export window is open."));
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    Dalamud.Bindings.ImGui.ImGui.Spacing();
    if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Confirm"), new Vector2()))
    {
      this.Close();
      exportLight();
    }
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, (float) ((double) ((ImGuiStylePtr) ref style).ItemInnerSpacing.X * 4.0));
    if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Don't show this warning again."), ref this.disableWarning))
      return;
    config.File.ExportLightIgnoreNoActorSelectedWarning = this.disableWarning;
  }
}
