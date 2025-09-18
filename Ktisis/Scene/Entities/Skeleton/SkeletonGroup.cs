// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.SkeletonGroup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;
using System.Linq;

using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Skeleton;

public abstract class SkeletonGroup(ISceneManager scene) : SkeletonNode(scene), IVisibility {
	public bool Visible {
		get => this.RecurseVisible().All(vis => vis.Visible);
		set {
			foreach (var visibility in this.RecurseVisible())
				visibility.Visible = value;
		}
	}

	private IEnumerable<IVisibility> RecurseVisible() {
		return this.Children.Where(child => child is IVisibility).Cast<IVisibility>();
	}

	protected void Clean(int pIndex, uint pId) {
		foreach (var sceneEntity in this.GetChildren().Where(item => {
			switch (item) {
				case BoneNodeGroup boneNodeGroup2:
					boneNodeGroup2.Clean(pIndex, pId);
					return boneNodeGroup2.IsStale();
				case BoneNode boneNode2:
					return boneNode2.Info.PartialIndex == pIndex && (int)boneNode2.PartialId != (int)pId;
				default:
					return false;
			}
		}).ToList())
			sceneEntity.Remove();
	}

	public IEnumerable<BoneNode> GetAllBones() {
		var skeletonGroup1 = this;
		var unique = new HashSet<BoneNode>();
		foreach (var child in skeletonGroup1.Children) {
			if (!(child is BoneNode allBone)) {
				if (child is SkeletonGroup skeletonGroup2) {
					var enumerator = skeletonGroup2.GetAllBones().GetEnumerator();
					while (enumerator.MoveNext()) {
						yield return enumerator.Current;
					}
					enumerator = null;
				}
			} else if (unique.Add(allBone))
				yield return allBone;
		}
	}

	public IEnumerable<BoneNode> GetIndividualBones() {
		var results = new List<BoneNode>();
		foreach (var sceneEntity in this.Recurse()) {
			if (!(sceneEntity is BoneNodeGroup boneNodeGroup)) {
				if (sceneEntity is BoneNode boneNode)
					results.Add(boneNode);
			} else
				results.AddRange(boneNodeGroup.GetIndividualBones());
		}
		var pose = this.Pose;
		results = results.Distinct().ToList();
		results.RemoveAll(bone => {
			var boneIndex = bone.Info.BoneIndex;
			var partialIx = bone.Info.PartialIndex;
			var partialInfo1 = pose.GetPartialInfo(partialIx);
			if (partialInfo1 == null)
				return false;
			if (partialInfo1.GetParentsOf(boneIndex).Any(parentId => results.Any(x => x.MatchesId(partialIx, parentId))))
				return true;
			if (partialIx == 0)
				return false;
			var partialInfo2 = pose.GetPartialInfo(0);
			if (partialInfo2 == null)
				return false;
			var connectedParentBoneIndex = partialInfo1.ConnectedParentBoneIndex;
			return partialInfo2.GetParentsOf(connectedParentBoneIndex).Prepend(connectedParentBoneIndex).Any(id => results.Any(x => x.MatchesId(0, id)));
		});
		return results;
	}
}
