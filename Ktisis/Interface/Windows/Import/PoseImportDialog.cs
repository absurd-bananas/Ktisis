// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Windows.Import.PoseImportDialog
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Linq;

using Ktisis.Data.Files;
using Ktisis.Editor.Context.Types;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Types;
using Ktisis.Interface.Components.Files;
using Ktisis.Interface.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Interface.Windows.Import;

public class PoseImportDialog : EntityEditWindow<ActorEntity> {
	private readonly IEditorContext _ctx;
	private readonly FileSelect<PoseFile> _select;

	public PoseImportDialog(IEditorContext ctx, FileSelect<PoseFile> select)
		: base("Import Pose", ctx, (ImGuiWindowFlags)64 /*0x40*/) {
		this._ctx = ctx;
		this._select = select;
		select.OnOpenDialog = this.OnFileDialogOpen;
	}

	private void OnFileDialogOpen(FileSelect<PoseFile> sender) {
		this._ctx.Interface.OpenPoseFile(sender.SetFile);
	}

	public virtual void Draw() {
		this.UpdateTarget();
		ImU8String imU8String;
		// ISSUE: explicit constructor call
		((ImU8String) ref imU8String).\u002Ector(19, 1);
		((ImU8String) ref imU8String).AppendLiteral("Importing pose for ");
		((ImU8String) ref imU8String).AppendFormatted<string>(this.Target.Name);
		Dalamud.Bindings.ImGui.ImGui.Text(imU8String);
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this._select.Draw();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
		this.DrawPoseApplication();
		Dalamud.Bindings.ImGui.ImGui.Spacing();
	}

	private void DrawPoseApplication() {
		using (ImRaii.Disabled(!this._select.IsFileOpened)) {
			var isSelectBones = this.Target.Recurse().Where(child => child is SkeletonNode).Any(child => child.IsSelected);
			this.DrawTransformSelect();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			this.DrawApplyModes(isSelectBones);
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			Dalamud.Bindings.ImGui.ImGui.Spacing();
			if (!Dalamud.Bindings.ImGui.ImGui.Button(ImU8String.op_Implicit("Apply"), new Vector2()))
				return;
			this.ApplyPoseFile(isSelectBones);
		}
	}

	private void DrawTransformSelect() {
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Transforms:"));
		var file = this._ctx.Config.File;
		var importPoseTransforms = file.ImportPoseTransforms;
		var flag1 = importPoseTransforms.HasFlag(PoseTransforms.Rotation);
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Rotation##PoseImportRot"), ref flag1))
			file.ImportPoseTransforms ^= PoseTransforms.Rotation;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		var flag2 = importPoseTransforms.HasFlag(PoseTransforms.Position);
		if (Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Position##PoseImportPos"), ref flag2))
			file.ImportPoseTransforms ^= PoseTransforms.Position;
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		var flag3 = importPoseTransforms.HasFlag(PoseTransforms.Scale);
		if (!Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Scale##PoseImportScale"), ref flag3))
			return;
		file.ImportPoseTransforms ^= PoseTransforms.Scale;
	}

	private void DrawApplyModes(bool isSelectBones) {
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Import Type:"));
		if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit("All"), this._ctx.Config.File.ImportPoseIncludeType == BoneTypeInclusion.Both))
			this._ctx.Config.File.ImportPoseIncludeType = BoneTypeInclusion.Both;
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Import face & body bones.\n- The above Rot/Pos Transform options will be ignored."));
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit("Body Only"), this._ctx.Config.File.ImportPoseIncludeType == BoneTypeInclusion.Body))
			this._ctx.Config.File.ImportPoseIncludeType = BoneTypeInclusion.Body;
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Import body bones.\n- Optionally with selective bone import below."));
		Dalamud.Bindings.ImGui.ImGui.SameLine();
		if (Dalamud.Bindings.ImGui.ImGui.RadioButton(ImU8String.op_Implicit("Face Only"), this._ctx.Config.File.ImportPoseIncludeType == BoneTypeInclusion.Face))
			this._ctx.Config.File.ImportPoseIncludeType = BoneTypeInclusion.Face;
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Import face bones.\n- Optionally with selective bone import below.\n- The above Rot/Pos Transform options will be ignored."));
		Dalamud.Bindings.ImGui.ImGui.Separator();
		Dalamud.Bindings.ImGui.ImGui.Text(ImU8String.op_Implicit("Body Import Options:"));
		using (ImRaii.Disabled(!isSelectBones || this._ctx.Config.File.ImportPoseIncludeType == BoneTypeInclusion.Both))
			Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Selected bones only"), ref this._ctx.Config.File.ImportPoseSelectedBones);
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Only selected bones from the imported pose will be applied.\nMultiple bones can be selected with Ctrl+Click."));
		using (ImRaii.Disabled(!this._ctx.Config.File.ImportPoseSelectedBones || this._ctx.Config.File.ImportPoseIncludeType == BoneTypeInclusion.Both))
			Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Include child bones"), ref this._ctx.Config.File.ImportPoseSelectedBonesIncludeChildBones);
		if (Dalamud.Bindings.ImGui.ImGui.IsItemHovered())
			Dalamud.Bindings.ImGui.ImGui.SetTooltip(ImU8String.op_Implicit("Whether child bones descending from the selected bone(s) will also be applied."));
		var flag = this._ctx.Config.File.ImportPoseTransforms.HasFlag(PoseTransforms.Position);
		using (ImRaii.Disabled(!isSelectBones || !this._ctx.Config.File.ImportPoseSelectedBones || !flag))
			Dalamud.Bindings.ImGui.ImGui.Checkbox(ImU8String.op_Implicit("Anchor group positions"), ref this._ctx.Config.File.AnchorPoseSelectedBones);
		Dalamud.Bindings.ImGui.ImGui.Separator();
	}

	private void ApplyPoseFile(bool isSelectBones) {
		var file = this._select.Selected?.File;
		if (file == null)
			return;
		var pose = this.Target.Pose;
		if (pose == null)
			return;
		var importPoseTransforms = this._ctx.Config.File.ImportPoseTransforms;
		var selectedBones = isSelectBones && this._ctx.Config.File.ImportPoseSelectedBones;
		var includeChildBones = this._ctx.Config.File.ImportPoseSelectedBonesIncludeChildBones;
		var importPoseIncludeType = this._ctx.Config.File.ImportPoseIncludeType;
		var poseSelectedBones = this._ctx.Config.File.AnchorPoseSelectedBones;
		this._ctx.Posing.ApplyPoseFile(pose, file, importPoseTransforms, selectedBones, poseSelectedBones, includeChildBones, importPoseIncludeType);
	}
}
