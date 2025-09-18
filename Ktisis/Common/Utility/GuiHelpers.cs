// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.GuiHelpers
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Bindings.ImGui;
using Ktisis.Editor.Selection;

#nullable disable
namespace Ktisis.Common.Utility;

public static class GuiHelpers
{
  public static SelectMode GetSelectMode()
  {
    SelectMode selectMode = SelectMode.Default;
    if (Dalamud.Bindings.ImGui.ImGui.IsKeyDown((ImGuiKey) 641))
      selectMode = SelectMode.Multiple;
    return selectMode;
  }

  public static float CalcContrastRatio(uint background, uint foreground)
  {
    float num1 = (float) (background >> 24 & (uint) byte.MaxValue);
    float num2 = (float) (foreground >> 24 & (uint) byte.MaxValue);
    float num3 = (float) ((0.00083372544031590223 * (double) num1 * (double) (background >> 16 /*0x10*/ & (uint) byte.MaxValue) + 0.002804705873131752 * (double) num1 * (double) (background >> 8 & (uint) byte.MaxValue) + 0.00028313725488260388 * (double) num1 * (double) (background & (uint) byte.MaxValue) + 0.05000000074505806) / (0.00083372544031590223 * (double) num2 * (double) (foreground >> 16 /*0x10*/ & (uint) byte.MaxValue) + 0.002804705873131752 * (double) num2 * (double) (foreground >> 8 & (uint) byte.MaxValue) + 0.00028313725488260388 * (double) num2 * (double) (foreground & (uint) byte.MaxValue) + 0.05000000074505806));
    return (double) num3 < 1.0 ? 1f / num3 : num3;
  }

  public static uint CalcBlackWhiteTextColor(uint background)
  {
    return (double) GuiHelpers.CalcContrastRatio(background, uint.MaxValue) >= 2.0 ? uint.MaxValue : 4278190080U /*0xFF000000*/;
  }
}
