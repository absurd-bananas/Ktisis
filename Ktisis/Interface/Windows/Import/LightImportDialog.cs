// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.Import.LightImportDialog
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Data;
using Ktisis.Interface.Components.Files;
using Ktisis.Interface.Types;
using Ktisis.Scene;
using Ktisis.Scene.Entities.World;

namespace Ktisis.Interface.Windows.Import;

public class LightImportDialog : EntityEditWindow<LightEntity> {
	private readonly IEditorContext _ctx;
	private readonly FileSelect<SceneManager.LightFile> _select;

	public LightImportDialog(IEditorContext ctx, FileSelect<SceneManager.LightFile> select)
		: base("Import Light", ctx, (ImGuiWindowFlags)64 /*0x40*/) {
		this._ctx = ctx;
		this._select = select;
		select.OnOpenDialog = this.OnFileDialogOpen;
	}

	private void OnFileDialogOpen(FileSelect<SceneManager.LightFile> sender) {
		this._ctx.Interface.OpenLightFile(sender.SetFile);
	}

	public virtual void Draw() {
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(24, 1);
		((ImU8String) ref imU8String).AppendLiteral("Import light preset for ");
		((ImU8String) ref imU8String).AppendFormatted<string>(this.Target.Name);
		Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this._select.Draw();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawLightApplication();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
	}

	private void DrawLightApplication() {
		using (ImRaii.Disabled(!this._select.IsFileOpened)) {
			this.DrawTransformSelect();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawPropertiesSelect();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Apply"), new Vector2()))
				return;
			this.ApplyLightFile();
		}
	}

	private void DrawTransformSelect() {
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Transforms:"));
		var importLightTransforms = this._ctx.Config.File.ImportLightTransforms;
		var flag1 = importLightTransforms.HasFlag(PoseTransforms.Rotation);
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Rotation##LightImportRot"), ref flag1))
			this._ctx.Config.File.ImportLightTransforms ^= PoseTransforms.Rotation;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		var flag2 = importLightTransforms.HasFlag(PoseTransforms.Position);
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Position##LightImportPos"), ref flag2))
			this._ctx.Config.File.ImportLightTransforms ^= PoseTransforms.Position;
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit(
				"This is an offset from the selected actor position.\nMake sure to select an actor before importing if you want to retain Position.\nIf no actor is selected, the first actor in the list will be selected instead."));
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		var flag3 = importLightTransforms.HasFlag(PoseTransforms.Scale);
		if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Scale##LightImportScale"), ref flag3))
			return;
		this._ctx.Config.File.ImportLightTransforms ^= PoseTransforms.Scale;
	}

	private void DrawPropertiesSelect() {
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Properties:"));
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Lighting##LightImportLighting"), ref this._ctx.Config.File.ImportLightLighting);
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Import lighting properties."));
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Color##LightImportColor"), ref this._ctx.Config.File.ImportLightColor);
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Import light color."));
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Shadows##LightImportShadows"), ref this._ctx.Config.File.ImportLightShadows);
		if (!Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			return;
		Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Import shadow properties."));
	}

	private void ApplyLightFile() {
		var file = this._select.Selected?.File;
		if (file == null)
			return;
		var target = this.Target;
		if (target == null)
			return;
		var importLightTransforms = this._ctx.Config.File.ImportLightTransforms;
		var importLightLighting = this._ctx.Config.File.ImportLightLighting;
		var importLightColor = this._ctx.Config.File.ImportLightColor;
		var importLightShadows = this._ctx.Config.File.ImportLightShadows;
		this._ctx.Scene.ApplyLightFile(target, file, importLightTransforms, importLightLighting, importLightColor, importLightShadows);
	}
}
