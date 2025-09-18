// Decompiled with JetBrains decompiler
// Type: Ktisis.Legacy.Interface.MigratorWindow
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using GLib.Widgets;

using Ktisis.Interface.Types;

namespace Ktisis.Legacy.Interface;

public class MigratorWindow : KtisisWindow {
	private const uint ColorYellow = 4278255615;
	private const int WaitTime = 10;
	private readonly IDalamudPluginInterface _dpi;
	private readonly LegacyMigrator _migrator;
	private readonly Stopwatch _timer = new Stopwatch();
	private bool _elapsed;

	public MigratorWindow(IDalamudPluginInterface dpi, LegacyMigrator migrator)
		: base("Ktisis Development Preview", (ImGuiWindowFlags)64 /*0x40*/) {
		this._dpi = dpi;
		this._migrator = migrator;
	}

	private bool CanBegin => this._timer.Elapsed.TotalSeconds >= 10.0 || this._elapsed;

	public virtual void OnOpen() {
		this._timer.Reset();
		this._timer.Start();
		this._elapsed = false;
	}

	public virtual void Draw() {
		if (!this._elapsed && this.CanBegin) {
			this._timer.Stop();
			this._elapsed = true;
		}
		ImGuiStylePtr style = Dalamud.Bindings.ImGui.ImGui.GetStyle();
		Icons.DrawIcon((FontAwesomeIcon)61546);
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("You have installed a development version of Ktisis."));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("This version is currently a "));
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		using (ImRaii.PushColor((ImGuiCol)0, 4278255615U, true))
			Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("development preview"));
		Dalamud.Bindings.ImGui.ImGui.SameLine(0.0f, ((ImGuiStylePtr) ref style).ItemInnerSpacing.X);
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(" - it is primarily a testbed for new features."));
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Only the bare essentials have been implemented at this stage so a lot of UI/UX will be missing."));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Icons.DrawIcon((FontAwesomeIcon)61529);
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("What to expect:"));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("This is not the full feature set of the final release."));
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit(
			"The following will be introduced at a later point during testing:\n\t• Everything missing from the current release\n\t• Editing spawned weapons and props\n\t• Equipment model manipulation\n\t• Importing and exporting light presets\n\t• Animation controls\n\t• Copy & paste\n"));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Undo and redo is currently only implemented for object transforms."));
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Support is planned for edits made to objects, such as appearance changes."));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Character appearance edits may also conflict with changes made by Glamourer."));
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("I hope to discuss with its developer about implementing an IPC to resolve this."));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Many configuration options will also be missing, which will be added during the testing period."));
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Your current configuration will not be carried over into this version."));
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		using (ImRaii.Disabled(!this.CanBegin && (!Dalamud.Bindings.ImGui.ImGui.IsKeyDown((ImGuiKey)641) || !Dalamud.Bindings.ImGui.ImGui.IsKeyDown((ImGuiKey)642)))) {
			string str;
			if (!this.CanBegin)
				str = $"Begin ({Math.Ceiling(10M - (decimal)this._timer.Elapsed.Seconds)}s)";
			else
				str = "Begin";
			if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit(str), new Vector2())) {
				this._migrator.Begin();
				this.Close();
			}
		}
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		if (Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Close"), new Vector2()))
			this.Close();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
	}
}
