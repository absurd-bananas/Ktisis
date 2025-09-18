// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.TransformResolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;
using System.Linq;

using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Editor.Transforms;

public static class TransformResolver {
	public static SceneEntity? GetPoseTarget(IEnumerable<SceneEntity> entities) {
		var poseTarget = (BoneNode)null;
		foreach (var boneNode in entities.Where(item => item is BoneNode).Cast<BoneNode>()) {
			if (poseTarget == null) {
				poseTarget = boneNode;
			} else {
				var pose = boneNode.Pose;
				if (pose == poseTarget.Pose) {
					var partialIndex1 = boneNode.Info.PartialIndex;
					var partialInfo = pose.GetPartialInfo(partialIndex1);
					if (partialInfo != null) {
						var num1 = partialIndex1;
						var partialIndex2 = poseTarget.Info.PartialIndex;
						int? nullable1;
						if (num1 == partialIndex2) {
							nullable1 = poseTarget.Info.BoneIndex;
						} else {
							var num2 = num1;
							var index = partialIndex2;
							var num3 = index;
							if (num2 < num3) {
								var connectedParentBoneIndex = pose.GetPartialInfo(index)?.ConnectedParentBoneIndex;
								nullable1 = connectedParentBoneIndex.HasValue ? connectedParentBoneIndex.GetValueOrDefault() : new int?();
							} else
								nullable1 = new int?();
						}
						var nullable2 = nullable1;
						if (nullable2.HasValue && (partialInfo.IsBoneDescendantOf(nullable2.Value, boneNode.Info.BoneIndex) || poseTarget.Info.ParentIndex == boneNode.Info.ParentIndex && boneNode.Info.BoneIndex < poseTarget.Info.BoneIndex))
							poseTarget = boneNode;
					}
				}
			}
		}
		return poseTarget;
	}

	public static IEnumerable<SceneEntity> GetCorrelatingBones(
		IEnumerable<SceneEntity> entities,
		bool yieldDefault = false
	) {
		var unique = new HashSet<BoneNode>();
		foreach (var entity in entities) {
			if (!(entity is BoneNode correlatingBone)) {
				if (entity is SkeletonGroup skeletonGroup) {
					var enumerator = skeletonGroup.GetIndividualBones().Where(bone => unique.Add(bone)).GetEnumerator();
					while (enumerator.MoveNext()) {
						yield return enumerator.Current;
					}
					enumerator = null;
				} else if (yieldDefault)
					yield return entity;
			} else if (unique.Add(correlatingBone))
				yield return correlatingBone;
		}
	}

	public static Dictionary<EntityPose, Dictionary<int, List<BoneNode>>> BuildPoseMap(
		SceneEntity? target,
		IEnumerable<SceneEntity> entities
	) {
		var dictionary1 = new Dictionary<EntityPose, Dictionary<int, List<BoneNode>>>();
		foreach (var boneNode in GetCorrelatingBones(entities).Cast<BoneNode>()) {
			var pose = boneNode.Pose;
			if (pose != target) {
				Dictionary<int, List<BoneNode>> dictionary2;
				var num1 = dictionary1.TryGetValue(pose, out dictionary2) ? 1 : 0;
				if (dictionary2 == null)
					dictionary2 = new Dictionary<int, List<BoneNode>>();
				var partialIndex = boneNode.Info.PartialIndex;
				List<BoneNode> boneNodeList;
				var num2 = dictionary2.TryGetValue(partialIndex, out boneNodeList) ? 1 : 0;
				if (boneNodeList == null)
					boneNodeList = new List<BoneNode>();
				boneNodeList.Add(boneNode);
				if (num2 == 0)
					dictionary2.Add(partialIndex, boneNodeList);
				if (num1 == 0)
					dictionary1.Add(pose, dictionary2);
			}
		}
		return dictionary1;
	}
}
