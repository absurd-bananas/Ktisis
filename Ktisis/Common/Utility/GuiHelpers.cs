// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Utility.GuiHelpers
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using Ktisis.Editor.Selection;

namespace Ktisis.Common.Utility;

public static class GuiHelpers {
	public static SelectMode GetSelectMode() {
		var selectMode = SelectMode.Default;
		if (Dalamud.Bindings.ImGui.ImGui.IsKeyDown((ImGuiKey)641))
			selectMode = SelectMode.Multiple;
		return selectMode;
	}

	public static float CalcContrastRatio(uint background, uint foreground) {
		var num1 = (float)(background >> 24 & byte.MaxValue);
		var num2 = (float)(foreground >> 24 & byte.MaxValue);
		var num3 =
			(float)((0.00083372544031590223 * num1 * (background >> 16 /*0x10*/ & byte.MaxValue) + 0.002804705873131752 * num1 * (background >> 8 & byte.MaxValue) + 0.00028313725488260388 * num1 * (background & byte.MaxValue) + 0.05000000074505806) /
			(0.00083372544031590223 * num2 * (foreground >> 16 /*0x10*/ & byte.MaxValue) + 0.002804705873131752 * num2 * (foreground >> 8 & byte.MaxValue) + 0.00028313725488260388 * num2 * (foreground & byte.MaxValue) + 0.05000000074505806));
		return num3 < 1.0 ? 1f / num3 : num3;
	}

	public static uint CalcBlackWhiteTextColor(uint background) => CalcContrastRatio(background, uint.MaxValue) >= 2.0 ? uint.MaxValue : 4278190080U /*0xFF000000*/;
}
