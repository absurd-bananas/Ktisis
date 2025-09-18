// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.BoneNodeGroup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Linq;

using Ktisis.Data.Config.Bones;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Skeleton;

public class BoneNodeGroup : SkeletonGroup, IAttachTarget {

	public BoneNodeGroup(ISceneManager scene, EntityPose pose)
		: base(scene) {
		this.Type = EntityType.BoneGroup;
		this.Pose = pose;
	}
	public BoneCategory? Category { get; set; }

	public bool TryAcceptAttach(IAttachable child) {
		BoneNode boneNode = Enumerable.MinBy<BoneNode, int>(this.GetIndividualBones().Where(bone => bone.Info.PartialIndex == 0), (Func<BoneNode, int>)(bone => bone.Info.BoneIndex));
		// ISSUE: explicit non-virtual call
		return boneNode != null && __nonvirtual(boneNode.TryAcceptAttach(child));
	}

	public bool IsStale() => !this.IsValid || this.GetChildren().Count == 0 || this.IsDisabledNsfw();

	private bool IsDisabledNsfw() {
		var category = this.Category;
		return category != null && category.IsNsfw && !this.Scene.Context.Config.Categories.ShowNsfwBones;
	}
}
