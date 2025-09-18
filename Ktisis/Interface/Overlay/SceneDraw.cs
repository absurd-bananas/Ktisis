// Decompiled with JetBrains decompiler
// Type: Ktisis.Interface.Overlay.SceneDraw
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Common.Utility;
using Ktisis.Core.Attributes;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Context.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.Utility;
using Ktisis.Scene.Types;
using Ktisis.Services.Game;

namespace Ktisis.Interface.Overlay;

[Transient]
public class SceneDraw {
	private readonly RefOverlay _refs;
	private readonly SelectableGui _select;
	private IEditorContext _ctx;
	private int i;

	public SceneDraw(SelectableGui select, RefOverlay refs) {
		this._select = select;
		this._refs = refs;
	}

	private OverlayConfig Config => this._ctx.Config.Overlay;

	public void SetContext(IEditorContext ctx) => this._ctx = ctx;

	public void DrawScene(bool gizmo = false, bool gizmoIsEnded = false) {
		var frame = this._select.BeginFrame();
		this.DrawEntities(frame, this._ctx.Scene.Children);
		this.DrawSelect(frame, gizmo, gizmoIsEnded);
	}

	private void DrawEntities(ISelectableFrame frame, IEnumerable<SceneEntity> entities) {
		this.i = 0;
		foreach (var entity in entities) {
			var opacityMultiplier = 1f;
			if (entity is ActorEntity actor) {
				opacityMultiplier = this.GetOpacityMultiplier(actor);
				if (opacityMultiplier == 0.0)
					continue;
			}
			this.DrawEntity(frame, entity, opacityMultiplier);
		}
	}

	private void DrawEntity(ISelectableFrame frame, SceneEntity entity, float opacityMultiplier) {
		switch (entity) {
			case EntityPose pose:
				this.DrawSkeleton(frame, pose, opacityMultiplier);
				break;
			case IVisibility visibility:
				if (!visibility.Visible || !(visibility is ITransform transform)) {
					if (entity is ReferenceImage image) {
						this._refs.DrawInstance(image);
					}
					break;
				}
				Vector3? position = transform.GetTransform()?.Position;
				if (position.HasValue && entity.Type != EntityType.BoneNode) {
					frame.AddItem(entity, position.Value, opacityMultiplier);
				}
				break;
		}
		++this.i;
		foreach (var child in entity.Children)
			this.DrawEntity(frame, child, opacityMultiplier);
	}

	private unsafe void DrawSkeleton(
		ISelectableFrame frame,
		EntityPose pose,
		float opacityMultiplier
	) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton1 = pose.GetSkeleton();
		if ((IntPtr)skeleton1 == IntPtr.Zero || (IntPtr)skeleton1->PartialSkeletons == IntPtr.Zero)
			return;
		Camera* sceneCamera = CameraService.GetSceneCamera();
		if ((IntPtr)sceneCamera == IntPtr.Zero)
			return;
		ImDrawListPtr windowDrawList = Dalamud.Bindings.ImGui.ImGui.GetWindowDrawList();
		var list = !this.Config.DrawLines || !this.Config.ColorSelectedBoneParentLine && !this.Config.ColorSelectedBoneDescendantLine || this.Config.DrawLinesGizmo && Ktisis.ImGuizmo.Gizmo.IsUsing
			? null
			: pose.GetAllBones().Where(x => x.IsSelected).ToList();
		var descendantSet = (HashSet<BoneNode>)null;
		if (list != null && this.Config.ColorSelectedBoneDescendantLine) {
			descendantSet = new HashSet<BoneNode>();
			foreach (var node in list) {
				foreach (var allBone in pose.GetAllBones()) {
					if (allBone.IsBoneDescendantOf(node))
						descendantSet.Add(allBone);
				}
			}
		}
		ushort partialSkeletonCount = skeleton1->PartialSkeletonCount;
		for (var partialIx = 0; partialIx < partialSkeletonCount; ++partialIx) {
			PartialSkeleton partialSkeleton = skeleton1->PartialSkeletons[partialIx];
			hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
			if ((IntPtr)havokPose != IntPtr.Zero && (IntPtr)havokPose->Skeleton != IntPtr.Zero) {
				hkaSkeleton* skeleton2 = havokPose->Skeleton;
				int length = skeleton2->Bones.Length;
				for (var boneIx1 = 0; boneIx1 < length; ++boneIx1) {
					var boneFromMap1 = pose.GetBoneFromMap(partialIx, boneIx1);
					// ISSUE: explicit non-virtual call
					if ((boneFromMap1 != null ? !__nonvirtual(boneFromMap1.Visible) ? 1 : 0 : 1) == 0) {
						var transform1 = boneFromMap1.CalcTransformWorld();
						if (transform1 != null) {
							frame.AddItem(boneFromMap1, transform1.Position, opacityMultiplier);
							if (this.Config.DrawLines && (this.Config.DrawLinesGizmo || !Ktisis.ImGuizmo.Gizmo.IsUsing)) {
								for (var boneIx2 = boneIx1; boneIx2 < length; ++boneIx2) {
									if ((int)skeleton2->ParentIndices[boneIx2] == boneIx1) {
										var boneFromMap2 = pose.GetBoneFromMap(partialIx, boneIx2);
										// ISSUE: explicit non-virtual call
										if ((boneFromMap2 != null ? !__nonvirtual(boneFromMap2.Visible) ? 1 : 0 : 1) == 0) {
											var transform2 = boneFromMap2.CalcTransformWorld();
											if (transform2 != null) {
												EntityDisplay entityDisplay = this._ctx.Config.GetEntityDisplay(boneFromMap1);
												this.DrawLine(sceneCamera, windowDrawList, transform1.Position, transform2.Position, this.GetBoneLineColor(boneFromMap2, list, descendantSet, entityDisplay), opacityMultiplier);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private uint GetBoneLineColor(
		BoneNode bone,
		List<BoneNode>? selectedBones,
		HashSet<BoneNode>? descendantSet,
		EntityDisplay display
	) {
		if (selectedBones == null)
			return display.Color == uint.MaxValue ? this.Config.DefaultLineColor : display.Color;
		if (bone.IsSelected)
			return !this.Config.ColorSelectedBoneParentLine ? display.Color : this.Config.SelectedBoneParentLineColor;
		if (descendantSet != null && descendantSet.Contains(bone))
			return this.Config.SelectedBoneDescendantLineColor;
		return display.Color == uint.MaxValue ? this.Config.DefaultLineColor : display.Color;
	}

	private unsafe float GetOpacityMultiplier(ActorEntity actor) {
		if (this.Config.ActiveActorOpacityMultiplier == (double)this.Config.InactiveActorOpacityMultiplier)
			return this.Config.ActiveActorOpacityMultiplier;
		if (this.Config.ActiveStateType == OverlayConfig.ActiveState.Target || this.Config.ActiveStateType == OverlayConfig.ActiveState.Both) {
			GameObject* gposeTarget = TargetSystem.Instance()->GPoseTarget;
			if (actor.Actor.Address == (IntPtr)gposeTarget)
				return this.Config.ActiveActorOpacityMultiplier;
		}
		if (this.Config.ActiveStateType == OverlayConfig.ActiveState.Selection || this.Config.ActiveStateType == OverlayConfig.ActiveState.Both) {
			actor.Recurse();
			if (actor.IsSelected || actor.Recurse().Any(x => x.IsSelected))
				return this.Config.ActiveActorOpacityMultiplier;
		}
		return this.Config.InactiveActorOpacityMultiplier;
	}

	private unsafe void DrawLine(
		Camera* camera,
		ImDrawListPtr drawList,
		Vector3 fromPos,
		Vector3 toPos,
		uint color,
		float opacityMultiplier
	) {
		Vector2 screenPos1;
		Vector2 screenPos2;
		if (!CameraService.WorldToScreen(camera, fromPos, out screenPos1) || !CameraService.WorldToScreen(camera, toPos, out screenPos2))
			return;
		var num = Ktisis.ImGuizmo.Gizmo.IsUsing ? this.Config.LineOpacityUsing : this.Config.LineOpacity;
		((ImDrawListPtr) ref drawList).AddLine(screenPos1, screenPos2, color.SetAlpha(Math.Clamp(num * opacityMultiplier, 0.0f, 1f)), this.Config.LineThickness);
	}

	private void DrawSelect(ISelectableFrame frame, bool gizmo, bool gizmoIsEnded) {
		SceneEntity clicked;
		if (!this._select.Draw(frame, out clicked, gizmo) || clicked == null || gizmo & gizmoIsEnded)
			return;
		var selectMode = GuiHelpers.GetSelectMode();
		this._ctx.Selection.Select(clicked, selectMode);
	}
}
