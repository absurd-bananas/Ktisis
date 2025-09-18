// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.PoseBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Data.Config;
using Ktisis.Data.Config.Bones;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Posing.Ik.Ccd;
using Ktisis.Editor.Posing.Types;
using Ktisis.Localization;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.Skeleton.Constraints;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Factory.Builders;

public sealed class PoseBuilder :
	EntityBuilder<EntityPose, IPoseBuilder>,
	IPoseBuilder,
	IEntityBuilder<EntityPose, IPoseBuilder>,
	IEntityBuilderBase<EntityPose, IPoseBuilder> {
	public PoseBuilder(ISceneManager scene)
		: base(scene) {
		this.Name = "Pose";
	}

	protected override IPoseBuilder Builder => this;

	public IBoneTreeBuilder BuildBoneTree(int index, uint partialId, PartialSkeleton partial) => new BoneTreeBuilder(this.Scene, index, partialId, partial);

	protected override EntityPose Build() {
		var ikController = this.Scene.Context.Posing.CreateIkController();
		var entityPose = new EntityPose(this.Scene, this, ikController);
		ikController.Setup(entityPose);
		return entityPose;
	}

	private class BoneTreeBuilder : BoneEnumerator, IBoneTreeBuilder {
		private readonly ISceneManager _scene;
		private readonly List<PartialBoneInfo> BoneList = new List<PartialBoneInfo>();
		private readonly Dictionary<BoneCategory, List<PartialBoneInfo>> CategoryMap = new Dictionary<BoneCategory, List<PartialBoneInfo>>();
		private readonly uint PartialId;

		public BoneTreeBuilder(
			ISceneManager scene,
			int index,
			uint partialId,
			PartialSkeleton partial
		)
			: base(index, partial) {
			this._scene = scene;
			this.PartialId = partialId;
		}

		private Configuration Config => this._scene.Context.Config;

		private LocaleManager Locale => this._scene.Context.Locale;

		public unsafe IBoneTreeBuilder BuildBoneList() {
			if ((IntPtr)this.GetSkeleton() != IntPtr.Zero) {
				var collection = this.EnumerateBones();
				this.BoneList.Clear();
				this.BoneList.AddRange(collection);
			}
			return this;
		}

		public unsafe IBoneTreeBuilder BuildCategoryMap() {
			this.CategoryMap.Clear();
			var categories = this.Config.Categories;
			hkaSkeleton* skeleton = this.GetSkeleton();
			if ((IntPtr)skeleton == IntPtr.Zero)
				return this;
			foreach (var enumerateBone in this.EnumerateBones()) {
				var key1 = categories.ResolveBestCategory(skeleton, enumerateBone.BoneIndex);
				if (key1 == null)
					Ktisis.Ktisis.Log.Warning($"Failed to find category for {enumerateBone.Name}! Skipping...", Array.Empty<object>());
				else if (!key1.IsNsfw || this.Config.Categories.ShowNsfwBones) {
					List<PartialBoneInfo> partialBoneInfoList1;
					if (this.CategoryMap.TryGetValue(key1, out partialBoneInfoList1)) {
						partialBoneInfoList1.Add(enumerateBone);
					} else {
						var categoryMap = this.CategoryMap;
						var key2 = key1;
						var capacity = 1;
						var partialBoneInfoList2 = new List<PartialBoneInfo>(capacity);
						CollectionsMarshal.SetCount<PartialBoneInfo>(partialBoneInfoList2, capacity);
						CollectionsMarshal.AsSpan<PartialBoneInfo>(partialBoneInfoList2)[0] = enumerateBone;
						categoryMap.Add(key2, partialBoneInfoList2);
					}
				}
			}
			this.BuildOrphanedCategories(categories);
			return this;
		}

		public void BindTo(EntityPose pose) {
			if (this.CategoryMap.Count > 0)
				this.BindGroups(pose, null);
			if (this.BoneList.Count <= 0)
				return;
			this.BindBones(pose, this.BoneList);
		}

		private void BuildOrphanedCategories(CategoryConfig categories) {
			var keys = this.CategoryMap.Keys.ToList();
			foreach (var boneCategory in keys.Where(category => category.ParentCategory != null && keys.All(x => x.Name != category.ParentCategory)).ToList()) {
				var category = boneCategory;
				var key = categories.CategoryList.Find((Predicate<BoneCategory>)(x => x.Name == category.ParentCategory));
				if (key != null && !this.CategoryMap.ContainsKey(key))
					this.CategoryMap.Add(key, new List<PartialBoneInfo>());
			}
		}

		private void BindGroups(SkeletonNode node, BoneCategory? parent) {
			var categories = this.CategoryMap.Where(x => x.Key.ParentCategory == parent?.Name).ToArray();
			var source = (List<BoneNodeGroup>)null;
			var list = node.Children.ToList();
			if (list.Count > 0)
				source = list.Where(x => x is BoneNodeGroup).Cast<BoneNodeGroup>().ToList();
			if (source != null) {
				foreach (var node1 in source.Where(group => categories.All(cat => cat.Key.Name != group.Name)))
					this.BindGroups(node1, node1.Category);
			}
			foreach (var keyValuePair in categories) {
				BoneCategory boneCategory;
				List<PartialBoneInfo> partialBoneInfoList;
				keyValuePair.Deconstruct(ref boneCategory, ref partialBoneInfoList);
				var category = boneCategory;
				var bones = partialBoneInfoList;
				var groupNode = source?.Find((Predicate<BoneNodeGroup>)(group => group.Category == category));
				var num = groupNode == null ? 1 : 0;
				if (groupNode == null)
					groupNode = this.CreateGroupNode(node.Pose, category);
				groupNode.Name = this.Locale.GetCategoryName(category);
				groupNode.Category = category;
				groupNode.SortPriority = category.SortPriority ?? -1;
				this.BindGroups(groupNode, category);
				this.BindBones(groupNode, bones);
				if (num != 0 && groupNode.Children.Any())
					node.Add(groupNode);
			}
			node.OrderByPriority();
		}

		private void BindBones(SkeletonNode node, List<PartialBoneInfo> bones) {
			var boneNodeList = (List<BoneNode>)null;
			var list = node.Children.ToList();
			if (list.Count > 0)
				boneNodeList = list.Where(x => x is BoneNode boneNode1 && boneNode1.Info.PartialIndex == this.Index).Cast<BoneNode>().ToList();
			var num = (int)this.Partial.ConnectedBoneIndex + 1;
			foreach (var bone1 in bones) {
				var boneInfo = bone1;
				var entity = boneNodeList?.Find((Predicate<BoneNode>)(bone => bone.Info.Name == boneInfo.Name));
				if (entity != null) {
					if (this.Index != entity.Info.PartialIndex) {
						node.Remove(entity);
					} else {
						entity.Info = boneInfo;
						entity.PartialId = this.PartialId;
					}
				} else {
					var boneNode2 = this.CreateBoneNode(node, boneInfo);
					boneNode2.Name = this.Locale.GetBoneName(boneInfo);
					boneNode2.SortPriority = num + boneInfo.BoneIndex;
					node.Add(boneNode2);
				}
			}
			node.OrderByPriority();
		}

		private BoneNodeGroup CreateGroupNode(EntityPose pose, BoneCategory category) {
			var name = category.Name;
			var boneCategory = category;
			if (boneCategory != null) {
				var twoJointsGroup = boneCategory.TwoJointsGroup;
				if (twoJointsGroup != null)
					goto label_3;
				label_2:
				var ccdGroup = boneCategory.CcdGroup;
				if (ccdGroup != null) {
					var ccdGroupParams = ccdGroup;
					CcdGroup group;
					if (pose.IkController.TrySetupGroup(name, ccdGroupParams, out group))
						return new IkNodeGroup<CcdGroup>(this._scene, pose, group);
					goto label_7;
				}
				goto label_7;
				label_3:
				var jointsGroupParams = twoJointsGroup;
				TwoJointsGroup group1;
				if (pose.IkController.TrySetupGroup(name, jointsGroupParams, out group1))
					return (BoneNodeGroup)new IkNodeGroup<TwoJointsGroup>(this._scene, pose, group1);
				goto label_2;
			}
			label_7:
			return new BoneNodeGroup(this._scene, pose);
		}

		private BoneNode CreateBoneNode(SkeletonNode parent, PartialBoneInfo boneInfo) {
			switch (parent) {
				case IkNodeGroup<TwoJointsGroup> ikNodeGroup1:
					if ((int)ikNodeGroup1.Group.EndBoneIndex == boneInfo.BoneIndex)
						return new TwoJointEndNode(this._scene, parent.Pose, boneInfo, this.PartialId, ikNodeGroup1.Group);
					break;
				case IkNodeGroup<CcdGroup> ikNodeGroup2:
					if (ikNodeGroup2.Group.EndBoneIndex == boneInfo.BoneIndex)
						return new CcdEndNode(this._scene, parent.Pose, boneInfo, this.PartialId, ikNodeGroup2.Group);
					break;
			}
			return new BoneNode(this._scene, parent.Pose, boneInfo, this.PartialId);
		}
	}
}
