// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Widgets.Environment.DayTimeControls
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable disable
using System;

using Ktisis.Structs.Env;

namespace Ktisis.Interface.Widgets.Environment;

public static class DayTimeControls {
	public const float MaxTime = 86400f;

	public unsafe static bool DrawTime(EnvManagerEx* env, out float time) {
		time = 0.0f;
		if ((IntPtr)env == IntPtr.Zero)
			return false;
		time = env->_base.DayTimeSeconds;
		var dateTime = new DateTime().AddSeconds(time);
		var num1 = Dalamud.Bindings.ImGui.ImGui.SliderFloat(ImU8String.op_Implicit("##TimeControls_Slider"), ref time, 0.0f, 86400f, ImU8String.op_Implicit(dateTime.ToShortTimeString()), (ImGuiSliderFlags)128 /*0x80*/) ? 1 : 0;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail().X);
		var num2 = Dalamud.Bindings.ImGui.ImGui.DragFloat(ImU8String.op_Implicit("##TimeControls_Drag"), ref time, 10f, 0.0f, 86400f, ImU8String.op_Implicit("%.0f"), (ImGuiSliderFlags)0) ? 1 : 0;
		return (num1 | num2) != 0;
	}

	public unsafe static bool DrawDay(EnvManagerEx* env, out int day) {
		day = CalculateDay(env);
		return Dalamud.Bindings.ImGui.ImGui.SliderInt(ImU8String.op_Implicit("##MoonPhase"), ref day, 0, 30, new ImU8String(), (ImGuiSliderFlags)0);
	}

	public unsafe static int CalculateDay(EnvManagerEx* env) => (int)Math.Ceiling(((double)FFXIVClientStructs.FFXIV.Client.System.Framework.Framework.Instance()->ClientTime.EorzeaTime - (double)env->_base.DayTimeSeconds) / 86400.0) % 32 /*0x20*/;
}
