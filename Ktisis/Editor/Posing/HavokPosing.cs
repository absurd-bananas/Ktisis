// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.HavokPosing
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Common.Utility;
using Ktisis.Interop;

namespace Ktisis.Editor.Posing;

public static class HavokPosing {
	private readonly static Alloc<Matrix4x4> Matrix = new Alloc<Matrix4x4>(16UL /*0x10*/);

	public unsafe static Matrix4x4 GetMatrix(hkQsTransformf* transform) {
		((hkQsTransformf)(IntPtr)transform).get4x4ColumnMajor((float*)Matrix.Address);
		return *Matrix.Data;
	}

	public unsafe static Matrix4x4 GetMatrix(hkaPose* pose, int boneIndex) => (IntPtr)pose == IntPtr.Zero || (IntPtr)pose->ModelPose.Data == IntPtr.Zero ? Matrix4x4.Identity : HavokPosing.GetMatrix(pose->ModelPose.Data + boneIndex);

	public unsafe static void SetMatrix(hkQsTransformf* trans, Matrix4x4 matrix) {
		*Matrix.Data = matrix;
		((hkQsTransformf)(IntPtr)trans).set((hkMatrix4f*)Matrix.Address);
	}

	public unsafe static void SetMatrix(hkaPose* pose, int boneIndex, Matrix4x4 matrix) {
		HavokPosing.SetMatrix(pose->ModelPose.Data + boneIndex, matrix);
	}

	public unsafe static Transform? GetModelTransform(hkaPose* pose, int boneIx) => (IntPtr)pose == IntPtr.Zero || (IntPtr)pose->ModelPose.Data == IntPtr.Zero || boneIx < 0 || boneIx > pose->ModelPose.Length
		? null
		: new Transform(HavokPosing.GetMatrix(pose->ModelPose.Data + boneIx));

	public unsafe static void SetModelTransform(hkaPose* pose, int boneIx, Transform trans) {
		if ((IntPtr)pose == IntPtr.Zero || (IntPtr)pose->ModelPose.Data == IntPtr.Zero || boneIx < 0 || boneIx > pose->ModelPose.Length)
			return;
		HavokPosing.SetMatrix(pose->ModelPose.Data + boneIx, trans.ComposeMatrix());
	}

	public unsafe static Transform? GetLocalTransform(hkaPose* pose, int boneIx) => (IntPtr)pose == IntPtr.Zero || (IntPtr)pose->LocalPose.Data == IntPtr.Zero || boneIx < 0 || boneIx > pose->LocalPose.Length
		? null
		: new Transform(HavokPosing.GetMatrix(pose->LocalPose.Data + boneIx));

	public unsafe static void Propagate(
		Skeleton* skele,
		int partialIx,
		int boneIx,
		Transform target,
		Transform initial,
		bool propagatePartials = true
	) {
		PartialSkeleton partialSkeleton1 = skele->PartialSkeletons[partialIx];
		hkaPose* havokPose1 = ((PartialSkeleton) ref partialSkeleton1 ).GetHavokPose(0);
		if ((IntPtr)havokPose1 == IntPtr.Zero || (IntPtr)havokPose1->Skeleton == IntPtr.Zero)
			return;
		Vector3 position = target.Position;
		Vector3 deltaPos = position - initial.Position;
		Quaternion deltaRot = target.Rotation / initial.Rotation;
		Propagate(havokPose1, boneIx, position, deltaPos, deltaRot);
		if (partialIx != 0 || !propagatePartials)
			return;
		hkaSkeleton* skeleton1 = havokPose1->Skeleton;
		for (var index = 0; index < (int)skele->PartialSkeletonCount; ++index) {
			PartialSkeleton partialSkeleton2 = skele->PartialSkeletons[index];
			if (!((PartialSkeleton) ref partialSkeleton2 ).HavokPoses.IsEmpty)
			{
				hkaPose* havokPose2 = ((PartialSkeleton) ref partialSkeleton2 ).GetHavokPose(0);
				if ((IntPtr)havokPose2 != IntPtr.Zero) {
					hkaSkeleton* skeleton2 = havokPose2->Skeleton;
					if (!IsMultiRootSkeleton(skeleton2->ParentIndices)) {
						short connectedBoneIndex = partialSkeleton2.ConnectedBoneIndex;
						short connectedParentBoneIndex = partialSkeleton2.ConnectedParentBoneIndex;
						if (connectedParentBoneIndex == boneIx || IsBoneDescendantOf(skeleton1->ParentIndices, connectedParentBoneIndex, boneIx))
							Propagate(havokPose2, connectedBoneIndex, position, deltaPos, deltaRot);
					} else {
						foreach (var multiRoot in GetMultiRoots(skeleton2->ParentIndices)) {
							short boneNameIndex = HavokPosing.TryGetBoneNameIndex(havokPose1, ((hkStringPtr) ref skeleton2->Bones[multiRoot].Name).String);
							if (((((hkStringPtr) ref skeleton1 ->Bones[boneIx].Name).string == ((hkStringPtr) ref skeleton2->Bones[multiRoot].Name).string ? 1 : 0) | (boneNameIndex != -1
								?
								IsBoneDescendantOf(skeleton1->ParentIndices, boneNameIndex, boneIx) ? 1 : 0
								: false
									? 1
									: 0)) != 0)
							Propagate(havokPose2, multiRoot, position, deltaPos, deltaRot);
						}
					}
				}
			}
		}
	}

	private unsafe static void Propagate(
		hkaPose* pose,
		int boneIx,
		Vector3 sourcePos,
		Vector3 deltaPos,
		Quaternion deltaRot
	) {
		hkaSkeleton* skeleton = pose->Skeleton;
		for (var index = boneIx; index < skeleton->Bones.Length; ++index) {
			if (IsBoneDescendantOf(skeleton->ParentIndices, index, boneIx)) {
				var modelTransform = GetModelTransform(pose, index);
				Matrix4x4 scale = Matrix4x4.CreateScale(modelTransform.Scale);
				Matrix4x4 fromQuaternion = Matrix4x4.CreateFromQuaternion(deltaRot * modelTransform.Rotation);
				Matrix4x4 translation = Matrix4x4.CreateTranslation(deltaPos + sourcePos + Vector3.Transform(modelTransform.Position - sourcePos, deltaRot));
				SetMatrix(pose, index, scale * fromQuaternion * translation);
			}
		}
	}

	public unsafe static Quaternion ParentSkeleton(Skeleton* modelSkeleton, int partialIndex) {
		PartialSkeleton partialSkeleton1 = modelSkeleton->PartialSkeletons[partialIndex];
		hkaPose* havokPose1 = ((PartialSkeleton) ref partialSkeleton1 ).GetHavokPose(0);
		if ((IntPtr)havokPose1 == IntPtr.Zero)
			return Quaternion.Identity;
		PartialSkeleton partialSkeleton2 = *modelSkeleton->PartialSkeletons;
		hkaPose* havokPose2 = ((PartialSkeleton) ref partialSkeleton2 ).GetHavokPose(0);
		if ((IntPtr)havokPose2 == IntPtr.Zero)
			return Quaternion.Identity;
		var modelTransform1 = GetModelTransform(havokPose1, (int)partialSkeleton1.ConnectedBoneIndex);
		var modelTransform2 = GetModelTransform(havokPose2, (int)partialSkeleton1.ConnectedParentBoneIndex);
		Quaternion quaternion = modelTransform2.Rotation / modelTransform1.Rotation;
		var transform1 = new Transform(modelTransform2.Position, modelTransform1.Rotation, modelTransform1.Scale);
		SetModelTransform(havokPose1, (int)partialSkeleton1.ConnectedBoneIndex, transform1);
		Propagate(modelSkeleton, partialIndex, (int)partialSkeleton1.ConnectedBoneIndex, transform1, modelTransform1);
		var transform2 = new Transform(modelTransform2.Position, quaternion * modelTransform1.Rotation, modelTransform2.Scale);
		SetModelTransform(havokPose1, (int)partialSkeleton1.ConnectedBoneIndex, transform2);
		Propagate(modelSkeleton, partialIndex, (int)partialSkeleton1.ConnectedBoneIndex, transform2, transform1);
		return quaternion;
	}

	public unsafe static void SyncModelSpace(Skeleton* skeleton, int partialIndex) {
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero)
			return;
		PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[partialIndex];
		hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
		if ((IntPtr)havokPose == IntPtr.Zero || (IntPtr)havokPose->Skeleton == IntPtr.Zero)
			return;
		for (var boneIx = 1; boneIx < havokPose->Skeleton->Bones.Length; ++boneIx) {
			var modelTransform1 = GetModelTransform(havokPose, (int)havokPose->Skeleton->ParentIndices[boneIx]);
			if (modelTransform1 != null) {
				var localTransform = GetLocalTransform(havokPose, boneIx);
				var modelTransform2 = GetModelTransform(havokPose, boneIx);
				modelTransform2.Position = modelTransform1.Position + Vector3.Transform(localTransform.Position, modelTransform1.Rotation);
				modelTransform2.Rotation = modelTransform1.Rotation * localTransform.Rotation;
				SetModelTransform(havokPose, boneIx, modelTransform2);
			}
		}
		if (partialIndex <= 0)
			return;
		ParentSkeleton(skeleton, partialIndex);
	}

	public unsafe static short TryGetBoneNameIndex(hkaPose* pose, string? name) {
		if ((IntPtr)pose == IntPtr.Zero || (IntPtr)pose->Skeleton == IntPtr.Zero || StringExtensions.IsNullOrEmpty(name))
			return -1;
		hkArray<hkaBone> bones = pose->Skeleton->Bones;
		for (short boneNameIndex = 0; boneNameIndex < bones.Length; ++boneNameIndex) {
			if (((hkStringPtr) ref bones [(int)boneNameIndex].Name).string == name)
			return boneNameIndex;
		}
		return -1;
	}

	public static bool IsBoneDescendantOf(hkArray<short> indices, int bone, int parent) {
		if (!IsMultiRootSkeleton(indices) && parent < 1)
			return true;
		for (short index = indices[bone]; index != -1; index = indices[(int)index]) {
			if (index == parent)
				return true;
		}
		return false;
	}

	public static bool IsMultiRootSkeleton(hkArray<short> indices) => GetMultiRoots(indices).Count > 1;

	public static List<int> GetMultiRoots(hkArray<short> indices) {
		var multiRoots = new List<int>();
		for (var index = 0; index < indices.Length; ++index) {
			if (indices[index] == -1)
				multiRoots.Add(index);
		}
		return multiRoots;
	}
}
