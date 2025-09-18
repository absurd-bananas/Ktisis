// Decompiled with JetBrains decompiler
// Type: Ktisis.Common.Extensions.ButtonsEx
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
namespace Ktisis.Common.Extensions;

public static class ButtonsEx {
	internal static bool IsClicked() =>
		Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(Dalamud.Bindings.ImGui.ImGui.GetItemRectMin(), Dalamud.Bindings.ImGui.ImGui.GetItemRectMax()) && Dalamud.Bindings.ImGui.ImGui.IsMouseClicked((ImGuiMouseButton)0);

	internal static bool IsClicked(Vector2 margin) => Dalamud.Bindings.ImGui.ImGui.IsMouseHoveringRect(Dalamud.Bindings.ImGui.ImGui.GetItemRectMin() - margin, Dalamud.Bindings.ImGui.ImGui.GetItemRectMax() + margin) &&
		Dalamud.Bindings.ImGui.ImGui.IsMouseClicked((ImGuiMouseButton)0);
}
