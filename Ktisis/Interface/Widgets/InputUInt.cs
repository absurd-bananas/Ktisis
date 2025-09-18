// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Widgets.InputUInt
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;

#nullable enable
namespace Ktisis.Interface.Widgets;

public static class InputUInt
{
  public static bool Draw(string label, ref uint value)
  {
    int num1 = (int) value;
    int num2 = Dalamud.Bindings.ImGui.ImGui.InputInt(ImU8String.op_Implicit(label), ref num1, 1, 0, new ImU8String(), (ImGuiInputTextFlags) 0) ? 1 : 0;
    if (num2 == 0)
      return num2 != 0;
    value = (uint) num1;
    return num2 != 0;
  }
}
