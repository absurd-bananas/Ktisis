// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.TransformTarget
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Common.Utility;
using Ktisis.Editor.Posing;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Editor.Transforms;

public class TransformTarget : ITransformTarget, ITransform {
	private readonly Dictionary<EntityPose, Dictionary<int, List<BoneNode>>> PoseMap;

	public TransformTarget(SceneEntity? primary, IEnumerable<SceneEntity> targets) {
		targets = targets.ToList();
		this.Primary = primary;
		this.Targets = targets;
		this.PoseMap = TransformResolver.BuildPoseMap(primary, targets);
	}

	public SceneEntity? Primary { get; }

	public IEnumerable<SceneEntity> Targets { get; }

	public TransformSetup Setup { get; set; } = new TransformSetup();

	public Transform? GetTransform() => this.Primary is ITransform primary ? primary.GetTransform() : null;

	public void SetTransform(Transform transform) {
		var transform1 = this.GetTransform();
		if (transform1 == null)
			return;
		this.TransformObjects(transform, transform1);
		this.TransformSkeletons(transform, transform1);
	}

	private void TransformObjects(Transform transform, Transform initial) {
		Matrix4x4 result1;
		if (!Matrix4x4.Invert(initial.ComposeMatrix(), out result1))
			return;
		Matrix4x4 result2 = result1 * transform.ComposeMatrix();
		if (this.Setup.MirrorRotation)
			Matrix4x4.Invert(result2, out result2);
		foreach (var sceneEntity in this.Targets.Where(tar => tar != null && tar.IsValid && !(tar is BoneNode))) {
			if (sceneEntity is ITransform transform2) {
				var transform1 = transform2.GetTransform();
				if (transform1 != null) {
					if (sceneEntity == this.Primary) {
						transform2.SetTransform(transform);
					} else {
						transform1.DecomposeMatrix(transform1.ComposeMatrix() * result2);
						transform2.SetTransform(transform1);
					}
				}
			}
		}
	}

	private unsafe void TransformSkeletons(Transform transform, Transform initial) {
		var delta = new Transform(transform.Position - initial.Position, transform.Rotation / initial.Rotation, transform.Scale / initial.Scale);
		foreach (var pose in this.PoseMap) {
			EntityPose entityPose1;
			Dictionary<int, List<BoneNode>> dictionary1;
			pose.Deconstruct(ref entityPose1, ref dictionary1);
			var entityPose2 = entityPose1;
			var dictionary2 = dictionary1;
			FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = entityPose2.GetSkeleton();
			if ((IntPtr)skeleton != IntPtr.Zero && (IntPtr)skeleton->PartialSkeletons != IntPtr.Zero) {
				ushort partialSkeletonCount = skeleton->PartialSkeletonCount;
				for (var key = 0; key < partialSkeletonCount; ++key) {
					List<BoneNode> source;
					if (dictionary2.TryGetValue(key, out source)) {
						PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[key];
						hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
						if ((IntPtr)havokPose != IntPtr.Zero) {
							foreach (var bone in source.Where(bone => bone.IsValid))
								this.TransformBone(transform, initial, delta, skeleton, havokPose, bone);
						}
					}
				}
			}
		}
	}

	private unsafe void TransformBone(
		Transform transform,
		Transform initial,
		Transform delta,
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton,
		hkaPose* hkaPose,
		BoneNode bone
	) {
		var boneIndex = bone.Info.BoneIndex;
		var transform1 = bone.GetTransform();
		if (transform1 == null)
			return;
		bool mirrorRotation = this.Setup.MirrorRotation;
		if (mirrorRotation && this.Primary is BoneNode primary)
			mirrorRotation &= !bone.IsBoneDescendantOf(primary);
		Matrix4x4 matrix;
		if (bone == this.Primary) {
			matrix = transform.ComposeMatrix();
		} else {
			Vector3 scales = transform1.Scale * delta.Scale;
			Quaternion quaternion1;
			Vector3 vector3;
			if (mirrorRotation) {
				quaternion1 = Quaternion.Conjugate(delta.Rotation);
				vector3 = -delta.Position;
			} else {
				quaternion1 = delta.Rotation;
				vector3 = delta.Position;
			}
			Quaternion quaternion2 = !this.Setup.RelativeBones ? quaternion1 * transform1.Rotation : transform1.Rotation / initial.Rotation * quaternion1 * initial.Rotation;
			Matrix4x4 scale = Matrix4x4.CreateScale(scales);
			Matrix4x4 fromQuaternion = Matrix4x4.CreateFromQuaternion(quaternion2);
			Matrix4x4 translation = Matrix4x4.CreateTranslation(transform1.Position + vector3);
			Matrix4x4 matrix4x4 = fromQuaternion;
			matrix = scale * matrix4x4 * translation;
		}
		var modelTransform1 = HavokPosing.GetModelTransform(hkaPose, boneIndex);
		bone.SetMatrix(matrix);
		if (!this.Setup.ParentBones)
			return;
		var modelTransform2 = HavokPosing.GetModelTransform(hkaPose, boneIndex);
		HavokPosing.Propagate(skeleton, bone.Info.PartialIndex, bone.Info.BoneIndex, modelTransform2, modelTransform1);
	}
}
