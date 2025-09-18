// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.EntityPoseConverter
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Common.Utility;
using Ktisis.Data.Files;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Editor.Posing.Data;

public class EntityPoseConverter(EntityPose target) {
	public bool IsPoseValid => target.IsValid;

	public unsafe PoseContainer Save() {
		var poseContainer = new PoseContainer();
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton != IntPtr.Zero)
			poseContainer.Store(skeleton);
		return poseContainer;
	}

	public PoseFile SaveFile() => new PoseFile { Bones = this.Save() };

	public unsafe void Load(
		PoseContainer pose,
		PoseTransforms transforms,
		BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both
	) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		pose.Apply(skeleton, transforms, boneTypeInclusion);
	}

	public unsafe void LoadPartial(
		PoseContainer pose,
		int partialIndex,
		PoseTransforms transforms,
		BoneTypeInclusion boneTypeInclusion
	) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		pose.ApplyToPartial(skeleton, partialIndex, transforms, boneTypeInclusion);
	}

	public unsafe void LoadBones(
		PoseContainer pose,
		IEnumerable<PartialBoneInfo> bones,
		PoseTransforms transforms,
		BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both
	) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		pose.ApplyToBones(skeleton, bones, transforms, boneTypeInclusion);
	}

	public void LoadSelectedBones(
		PoseContainer pose,
		PoseTransforms transforms,
		bool selectedBonesIncludeChildren,
		BoneTypeInclusion boneTypeInclusion
	) {
		var selectedBones = this.GetSelectedBones(selectedBonesIncludeChildren);
		this.LoadBones(pose, selectedBones, transforms, boneTypeInclusion);
	}

	public void LoadUnselectedBones(
		PoseContainer pose,
		PoseTransforms transforms,
		bool selectedBonesIncludeChildren,
		BoneTypeInclusion boneTypeInclusion
	) {
		var bones1 = this.GetBones();
		var selectedBones = this.GetSelectedBones(selectedBonesIncludeChildren);
		var bones2 = new List<PartialBoneInfo>();
		foreach (var partialBoneInfo in bones1) {
			var b = partialBoneInfo;
			if (selectedBones.FirstOrDefault(x => x.Name == b.Name) == null)
				bones2.Add(b);
		}
		this.LoadBones(pose, bones2, transforms, boneTypeInclusion);
	}

	public unsafe void LoadReferencePose() {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		for (var partialIndex = 0; partialIndex < (int)skeleton->PartialSkeletonCount; ++partialIndex) {
			PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[partialIndex];
			hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
			if ((IntPtr)havokPose != IntPtr.Zero) {
				((hkaPose)(IntPtr)havokPose).SetToReferencePose();
				HavokPosing.SyncModelSpace(skeleton, partialIndex);
			}
		}
	}

	public unsafe PoseContainer FlipBones(bool flipYawCorrection, bool flipRotationCorrect) {
		var poseContainer = new PoseContainer();
		var list = this.GetBones().ToList();
		foreach (var partialBoneInfo in list)
			poseContainer[partialBoneInfo.Name] = new Transform();
		if (list.All(bone => bone.PartialIndex == 0))
			return poseContainer;
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero)
			return poseContainer;
		var array = list.ToArray();
		var partialBoneInfo1 = array[0];
		Quaternion rotation = HavokPosing.GetModelTransform(((PartialSkeleton)(IntPtr)(skeleton->PartialSkeletons + partialBoneInfo1.PartialIndex)).GetHavokPose(0), partialBoneInfo1.BoneIndex).Rotation;
		poseContainer.ApplyFlipToBones(skeleton);
		poseContainer.MainBoneYReset(skeleton, array[0], rotation, flipYawCorrection, flipRotationCorrect);
		return poseContainer;
	}

	public unsafe PoseContainer FilterSelectedBones(
		PoseContainer pose,
		bool selectedBonesIncludeChildren,
		bool all = true
	) {
		var poseContainer = new PoseContainer();
		var list = this.GetSelectedBones(selectedBonesIncludeChildren, all).ToList();
		foreach (var partialBoneInfo in list) {
			Transform transform;
			if (pose.TryGetValue(partialBoneInfo.Name, out transform))
				poseContainer[partialBoneInfo.Name] = transform;
		}
		if (list.All(bone => bone.PartialIndex == 0))
			return poseContainer;
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero)
			return poseContainer;
		for (var index = 1; index < (int)skeleton->PartialSkeletonCount; ++index) {
			PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[index];
			hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
			if ((IntPtr)havokPose != IntPtr.Zero && (IntPtr)havokPose->Skeleton != IntPtr.Zero) {
				string key = ((hkStringPtr) ref havokPose ->Skeleton->Bones[(int)partialSkeleton.ConnectedBoneIndex].Name).string;
				Transform transform;
				if (!StringExtensions.IsNullOrEmpty(key) && !poseContainer.ContainsKey(key) && pose.TryGetValue(key, out transform))
					poseContainer[key] = transform;
			}
		}
		return poseContainer;
	}

	public IEnumerable<PartialBoneInfo> IntersectBonesByName(IEnumerable<PartialBoneInfo> second) {
		return Enumerable.IntersectBy<PartialBoneInfo, string>(this.GetBones(), second.Select(bone => bone.Name), (Func<PartialBoneInfo, string>)(bone => bone.Name));
	}

	private unsafe IEnumerable<PartialBoneInfo> GetBones() {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero)
			return Array.Empty<PartialBoneInfo>();
		var bones = new List<PartialBoneInfo>();
		bones.AddRange(this.GetPartialBones(0));
		for (var index = 0; index < (int)skeleton->PartialSkeletonCount; ++index)
			bones.AddRange(this.GetPartialBones(index));
		return bones;
	}

	private unsafe IEnumerable<PartialBoneInfo> GetPartialBones(int index) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero)
			return Array.Empty<PartialBoneInfo>();
		PartialSkeleton partial = skeleton->PartialSkeletons[index];
		Span<ulong> havokPoses = ((PartialSkeleton) ref partial ).HavokPoses;
		if (!havokPoses.IsEmpty) {
			havokPoses = ((PartialSkeleton) ref partial).HavokPoses;
			if (havokPoses[0] != 0UL)
				return new BoneEnumerator(index, partial).EnumerateBones();
		}
		return Array.Empty<PartialBoneInfo>();
	}

	public IEnumerable<PartialBoneInfo> GetSelectedBones(bool selectedBonesIncludeChildren, bool all = true) {
		IEnumerable<SkeletonNode> nodes;
		if (!selectedBonesIncludeChildren) {
			nodes = target.Recurse().Prepend(target).Where(entity => entity is SkeletonNode && entity.IsSelected).Cast<SkeletonNode>();
		} else {
			var list1 = target.Recurse().Prepend(target).OfType<SkeletonNode>().ToList();
			var list2 = list1.Where(entity => entity.IsSelected).ToList();
			var skeletonNodeSet = new HashSet<SkeletonNode>();
			foreach (var node in list2) {
				skeletonNodeSet.Add(node);
				this.AddDescendantsToSet(node, list1, skeletonNodeSet);
			}
			nodes = skeletonNodeSet.AsEnumerable();
		}
		return this.GetBoneSelectionFrom(nodes, all).Distinct();
	}

	public IEnumerable<PartialBoneInfo> GetUnselectedBones(
		bool selectedBonesIncludeChildren,
		bool all = true
	) {
		IEnumerable<SkeletonNode> nodes;
		if (!selectedBonesIncludeChildren) {
			nodes = target.Recurse().Prepend(target).Where(entity => entity is SkeletonNode && !entity.IsSelected).Cast<SkeletonNode>();
		} else {
			var list1 = target.Recurse().Prepend(target).OfType<SkeletonNode>().ToList();
			var list2 = list1.Where(entity => !entity.IsSelected).ToList();
			var skeletonNodeSet = new HashSet<SkeletonNode>();
			foreach (var node in list2) {
				skeletonNodeSet.Add(node);
				this.AddDescendantsToSet(node, list1, skeletonNodeSet);
			}
			nodes = skeletonNodeSet.AsEnumerable();
		}
		return this.GetBoneSelectionFrom(nodes, all).Distinct();
	}

	private void AddDescendantsToSet(
		SkeletonNode node,
		List<SkeletonNode> allBones,
		HashSet<SkeletonNode> selectedBonesSet
	) {
		switch (node) {
			case BoneNode node1:
				using (var enumerator = allBones.GetEnumerator()) {
					while (enumerator.MoveNext()) {
						if (enumerator.Current is BoneNode current && current.IsBoneDescendantOf(node1))
							selectedBonesSet.Add(current);
					}
					break;
				}
			case SkeletonGroup skeletonGroup:
				using (var enumerator = skeletonGroup.GetAllBones().GetEnumerator()) {
					while (enumerator.MoveNext()) {
						var current = enumerator.Current;
						selectedBonesSet.Add(current);
						this.AddDescendantsToSet(current, allBones, selectedBonesSet);
					}
					break;
				}
		}
	}

	private IEnumerable<PartialBoneInfo> GetBoneSelectionFrom(
		IEnumerable<SkeletonNode> nodes,
		bool all = true
	) {
		foreach (var node in nodes) {
			if (!(node is BoneNode boneNode)) {
				if (node is SkeletonGroup skeletonGroup) {
					var enumerator = this.GetBoneSelectionFrom(all ? skeletonGroup.GetAllBones() : (IEnumerable<SkeletonNode>)skeletonGroup.GetIndividualBones()).GetEnumerator();
					while (enumerator.MoveNext()) {
						yield return enumerator.Current;
					}
					enumerator = null;
				}
			} else
				yield return boneNode.Info;
		}
	}
}
