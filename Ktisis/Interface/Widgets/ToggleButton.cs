// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Widgets.ToggleButton
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Interface.Widgets;

public static class ToggleButton
{
  private static readonly Vector4 ToggleBg = new Vector4(0.35f, 0.35f, 0.35f, 1f);
  private static readonly Vector4 ToggleBgHover = new Vector4(0.78f, 0.78f, 0.78f, 1f);
  private const float ToggleWidthRatio = 1.55f;

  public static bool Draw(string id, ref bool v, uint circleColor = 4294967295 /*0xFFFFFFFF*/)
  {
    ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
    Span<Vector4> colors = ((ImGuiStylePtr) ref style).Colors;
    Vector2 cursorScreenPos = Dalamud.Bindings.ImGui.ImGui.GetCursorScreenPos();
    ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
    float frameHeight = Dalamud.Bindings.ImGui.ImGui.GetFrameHeight();
    float x = frameHeight * 1.55f;
    float num = frameHeight * 0.5f;
    bool flag = false;
    Dalamud.Bindings.ImGui.ImGui.InvisibleButton(ImU8String.op_Implicit(id), new Vector2(x, frameHeight), (ImGuiButtonFlags) 0);
    if (Dalamud.Bindings.ImGui.ImGui.IsItemClicked())
    {
      flag = true;
      v = !v;
    }
    Vector4 vector4 = !Dalamud.Bindings.ImGui.ImGui.IsItemHovered() ? (v ? ToggleButton.ToggleBg : colors[21] * 0.6f) : (v ? ToggleButton.ToggleBgHover : colors[23]);
    Vector2 vector2 = new Vector2(cursorScreenPos.X + x, cursorScreenPos.Y + frameHeight);
    ((ImDrawListPtr) ref windowDrawList).AddRectFilled(cursorScreenPos, vector2, Dalamud.Bindings.ImGui.ImGui.GetColorU32(vector4), frameHeight * 0.5f);
    ((ImDrawListPtr) ref windowDrawList).AddCircleFilled(new Vector2((float) ((double) cursorScreenPos.X + (double) num + (double) (v ? 1 : 0) * ((double) x - (double) num * 2.0)), cursorScreenPos.Y + num), num - 1.5f, circleColor);
    return flag;
  }
}
