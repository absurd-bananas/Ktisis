// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.Editors.EnvWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using GLib.Widgets;

using Ktisis.Interface.Components.Environment;
using Ktisis.Interface.Components.Environment.Editors;
using Ktisis.Interface.Types;
using Ktisis.Interface.Widgets.Environment;
using Ktisis.Scene.Modules;
using Ktisis.Scene.Types;
using Ktisis.Services.Environment;
using Ktisis.Structs.Env;

namespace Ktisis.Interface.Windows.Editors;

public class EnvWindow : KtisisWindow {
	private readonly Dictionary<EnvEditorTab, EditorBase> _editors = new Dictionary<EnvEditorTab, EditorBase>();
	private readonly IEnvModule _module;
	private readonly ISceneManager _scene;
	private readonly WeatherSelect _weatherSelect;
	private EnvEditorTab Current;

	public EnvWindow(
		ISceneManager scene,
		IEnvModule module,
		WeatherSelect weatherSelect,
		SkyEditor sky,
		LightingEditor lighting,
		FogEditor fog,
		RainEditor rain,
		ParticlesEditor dust,
		StarsEditor stars,
		WindEditor wind
	)
		: base("Environment Editor") {
		this._scene = scene;
		this._module = module;
		this._weatherSelect = weatherSelect;
		this.Setup(EnvEditorTab.Sky, sky).Setup(EnvEditorTab.Light, lighting).Setup(EnvEditorTab.Fog, fog).Setup(EnvEditorTab.Rain, rain).Setup(EnvEditorTab.Particles, dust).Setup(EnvEditorTab.Stars, stars).Setup(EnvEditorTab.Wind, wind);
	}

	private EnvWindow Setup(EnvEditorTab id, EditorBase editor) {
		this._editors.Add(id, editor);
		return this;
	}

	public virtual void PreOpenCheck() {
		if (this._scene.IsValid && this._module.IsInit)
			return;
		Ktisis.Ktisis.Log.Verbose("State for env editor is stale, closing...", Array.Empty<object>());
		this.Close();
	}

	public virtual void PreDraw() {
		base.PreDraw();
		Window.WindowSizeConstraints windowSizeConstraints;
		// ISSUE: explicit constructor call
		((Window.WindowSizeConstraints) ref windowSizeConstraints).\u002Ector();
		((Window.WindowSizeConstraints) ref windowSizeConstraints).MinimumSize = new Vector2(400f, 300f);
		ref Window.WindowSizeConstraints local = ref windowSizeConstraints;
		ImGuiIOPtr io = Dalamud.Bindings.ImGui.ImGui.GetIO();
		Vector2 vector2 = ((ImGuiIOPtr) ref io ).DisplaySize * 0.9f;
		((Window.WindowSizeConstraints) ref local).MaximumSize = vector2;
		this.SizeConstraints = new Window.WindowSizeConstraints?(windowSizeConstraints);
	}

	public unsafe virtual void Draw() {
		var env = EnvManagerEx.Instance();
		if ((IntPtr)env == IntPtr.Zero)
			return;
		this.DrawSideBar(env);
		if (this.Current == EnvEditorTab.None)
			return;
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, (((ImGuiStylePtr) ref style).ItemSpacing + ((ImGuiStylePtr) ref style).FramePadding / 2f).X);
		this.DrawAdvancedEditor(env);
	}

	private unsafe void DrawSideBar(EnvManagerEx* env) {
		Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
		contentRegionAvail.X *= 0.35f;
		using (ImRaii.Child(ImU8String.op_Implicit("##EnvWeather"), contentRegionAvail)) {
			this.DrawWeatherTimeControls(env, contentRegionAvail.X);
			this.DrawAdvancedList();
		}
	}

	private unsafe void DrawWeatherTimeControls(EnvManagerEx* env, float width) {
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Weather"));
		WeatherInfo selected;
		if (this._weatherSelect.Draw(env, out selected) && selected != null) {
			var rowId = (byte)selected.RowId;
			this._module.Weather = rowId;
			env->_base.ActiveWeather = rowId;
		}
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		var num = this._module.Override.HasFlag(EnvOverride.TimeWeather) ? 1 : 0;
		if (Buttons.IconButton(num != 0 ? (FontAwesomeIcon)61475 : (FontAwesomeIcon)61596)) {
			this._module.Weather = env->_base.ActiveWeather;
			this._module.Time = env->_base.DayTimeSeconds;
			this._module.Day = DayTimeControls.CalculateDay(env);
			this._module.Override ^= EnvOverride.TimeWeather;
		}
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Time and Day"));
		using (ImRaii.Disabled(num == 0)) {
			float time;
			if (DayTimeControls.DrawTime(env, out time))
				this._module.Time = time;
			Dalamud.Bindings.ImGui.ImGui.SetNextItemWidth(width);
			int day;
			if (!DayTimeControls.DrawDay(env, out day))
				return;
			this._module.Day = day;
		}
	}

	private void DrawAdvancedList() {
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Advanced Editing"));
		Vector2 contentRegionAvail = Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail();
		ref float local = ref contentRegionAvail.Y;
		var num1 = (double)local;
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		var num2 = (double)((ImGuiStylePtr) ref style ).WindowPadding.Y / 2.0;
		local = (float)(num1 - num2);
		using (ImRaii.IEndObject iendObject = ImRaii.ListBox(ImU8String.op_Implicit("##AdvancedOptions"), contentRegionAvail)) {
			if (!iendObject.Success)
				return;
			foreach (var editor in this._editors) {
				EnvEditorTab envEditorTab1;
				EditorBase editorBase1;
				editor.Deconstruct(ref envEditorTab1, ref editorBase1);
				var envEditorTab2 = envEditorTab1;
				var editorBase2 = editorBase1;
				using (ImRaii.PushColor((ImGuiCol)0, (uint)int.MaxValue, !editorBase2.IsActivated(this._module.Override))) {
					var flag = envEditorTab2 == this.Current;
					if (Dalamud.Bindings.ImGui.ImGui.Selectable(ImU8String.op_Implicit(editorBase2.Name), flag, (ImGuiSelectableFlags)0, new Vector2()))
						this.Current = !flag ? envEditorTab2 : EnvEditorTab.None;
				}
			}
		}
	}

	private unsafe void DrawAdvancedEditor(EnvManagerEx* env) {
		using (ImRaii.IEndObject iendObject = ImRaii.Child(ImU8String.op_Implicit("##AdvancedFrame"), Dalamud.Bindings.ImGui.ImGui.GetContentRegionAvail())) {
			EditorBase editorBase;
			if (!iendObject.Success || !this._editors.TryGetValue(this.Current, out editorBase))
				return;
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(editorBase.Name));
			Dalamud.Bindings.ImGui.ImGui.Separator();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			editorBase.Draw(this._module, ref env->EnvState);
		}
	}

	private enum EnvEditorTab {
		None,
		Sky,
		Light,
		Fog,
		Rain,
		Particles,
		Stars,
		Wind
	}
}
