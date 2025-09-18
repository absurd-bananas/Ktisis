// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.PoseBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using Ktisis.Data.Config;
using Ktisis.Data.Config.Bones;
using Ktisis.Data.Config.Sections;
using Ktisis.Editor.Posing.Ik;
using Ktisis.Editor.Posing.Ik.Ccd;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Editor.Posing.Types;
using Ktisis.Localization;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.Skeleton.Constraints;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#nullable enable
namespace Ktisis.Scene.Factory.Builders;

public sealed class PoseBuilder : 
  EntityBuilder<EntityPose, IPoseBuilder>,
  IPoseBuilder,
  IEntityBuilder<EntityPose, IPoseBuilder>,
  IEntityBuilderBase<EntityPose, IPoseBuilder>
{
  public PoseBuilder(ISceneManager scene)
    : base(scene)
  {
    this.Name = "Pose";
  }

  protected override IPoseBuilder Builder => (IPoseBuilder) this;

  protected override EntityPose Build()
  {
    IIkController ikController = this.Scene.Context.Posing.CreateIkController();
    EntityPose entityPose = new EntityPose(this.Scene, (IPoseBuilder) this, ikController);
    ikController.Setup((ISkeleton) entityPose);
    return entityPose;
  }

  public IBoneTreeBuilder BuildBoneTree(int index, uint partialId, PartialSkeleton partial)
  {
    return (IBoneTreeBuilder) new PoseBuilder.BoneTreeBuilder(this.Scene, index, partialId, partial);
  }

  private class BoneTreeBuilder : BoneEnumerator, IBoneTreeBuilder
  {
    private readonly ISceneManager _scene;
    private readonly uint PartialId;
    private readonly Dictionary<BoneCategory, List<PartialBoneInfo>> CategoryMap = new Dictionary<BoneCategory, List<PartialBoneInfo>>();
    private readonly List<PartialBoneInfo> BoneList = new List<PartialBoneInfo>();

    private Configuration Config => this._scene.Context.Config;

    private LocaleManager Locale => this._scene.Context.Locale;

    public BoneTreeBuilder(
      ISceneManager scene,
      int index,
      uint partialId,
      PartialSkeleton partial)
      : base(index, partial)
    {
      this._scene = scene;
      this.PartialId = partialId;
    }

    public unsafe IBoneTreeBuilder BuildBoneList()
    {
      if ((IntPtr) this.GetSkeleton() != IntPtr.Zero)
      {
        IEnumerable<PartialBoneInfo> collection = this.EnumerateBones();
        this.BoneList.Clear();
        this.BoneList.AddRange(collection);
      }
      return (IBoneTreeBuilder) this;
    }

    public unsafe IBoneTreeBuilder BuildCategoryMap()
    {
      this.CategoryMap.Clear();
      CategoryConfig categories = this.Config.Categories;
      hkaSkeleton* skeleton = this.GetSkeleton();
      if ((IntPtr) skeleton == IntPtr.Zero)
        return (IBoneTreeBuilder) this;
      foreach (PartialBoneInfo enumerateBone in this.EnumerateBones())
      {
        BoneCategory key1 = categories.ResolveBestCategory(skeleton, enumerateBone.BoneIndex);
        if (key1 == null)
          Ktisis.Ktisis.Log.Warning($"Failed to find category for {enumerateBone.Name}! Skipping...", Array.Empty<object>());
        else if (!key1.IsNsfw || this.Config.Categories.ShowNsfwBones)
        {
          List<PartialBoneInfo> partialBoneInfoList1;
          if (this.CategoryMap.TryGetValue(key1, out partialBoneInfoList1))
          {
            partialBoneInfoList1.Add(enumerateBone);
          }
          else
          {
            Dictionary<BoneCategory, List<PartialBoneInfo>> categoryMap = this.CategoryMap;
            BoneCategory key2 = key1;
            int capacity = 1;
            List<PartialBoneInfo> partialBoneInfoList2 = new List<PartialBoneInfo>(capacity);
            CollectionsMarshal.SetCount<PartialBoneInfo>(partialBoneInfoList2, capacity);
            CollectionsMarshal.AsSpan<PartialBoneInfo>(partialBoneInfoList2)[0] = enumerateBone;
            categoryMap.Add(key2, partialBoneInfoList2);
          }
        }
      }
      this.BuildOrphanedCategories(categories);
      return (IBoneTreeBuilder) this;
    }

    private void BuildOrphanedCategories(CategoryConfig categories)
    {
      List<BoneCategory> keys = this.CategoryMap.Keys.ToList<BoneCategory>();
      foreach (BoneCategory boneCategory in keys.Where<BoneCategory>((Func<BoneCategory, bool>) (category => category.ParentCategory != null && keys.All<BoneCategory>((Func<BoneCategory, bool>) (x => x.Name != category.ParentCategory)))).ToList<BoneCategory>())
      {
        BoneCategory category = boneCategory;
        BoneCategory key = categories.CategoryList.Find((Predicate<BoneCategory>) (x => x.Name == category.ParentCategory));
        if (key != null && !this.CategoryMap.ContainsKey(key))
          this.CategoryMap.Add(key, new List<PartialBoneInfo>());
      }
    }

    public void BindTo(EntityPose pose)
    {
      if (this.CategoryMap.Count > 0)
        this.BindGroups((SkeletonNode) pose, (BoneCategory) null);
      if (this.BoneList.Count <= 0)
        return;
      this.BindBones((SkeletonNode) pose, this.BoneList);
    }

    private void BindGroups(SkeletonNode node, BoneCategory? parent)
    {
      KeyValuePair<BoneCategory, List<PartialBoneInfo>>[] categories = this.CategoryMap.Where<KeyValuePair<BoneCategory, List<PartialBoneInfo>>>((Func<KeyValuePair<BoneCategory, List<PartialBoneInfo>>, bool>) (x => x.Key.ParentCategory == parent?.Name)).ToArray<KeyValuePair<BoneCategory, List<PartialBoneInfo>>>();
      List<BoneNodeGroup> source = (List<BoneNodeGroup>) null;
      List<SceneEntity> list = node.Children.ToList<SceneEntity>();
      if (list.Count > 0)
        source = list.Where<SceneEntity>((Func<SceneEntity, bool>) (x => x is BoneNodeGroup)).Cast<BoneNodeGroup>().ToList<BoneNodeGroup>();
      if (source != null)
      {
        foreach (BoneNodeGroup node1 in source.Where<BoneNodeGroup>((Func<BoneNodeGroup, bool>) (group => ((IEnumerable<KeyValuePair<BoneCategory, List<PartialBoneInfo>>>) categories).All<KeyValuePair<BoneCategory, List<PartialBoneInfo>>>((Func<KeyValuePair<BoneCategory, List<PartialBoneInfo>>, bool>) (cat => cat.Key.Name != group.Name)))))
          this.BindGroups((SkeletonNode) node1, node1.Category);
      }
      foreach (KeyValuePair<BoneCategory, List<PartialBoneInfo>> keyValuePair in categories)
      {
        BoneCategory boneCategory;
        List<PartialBoneInfo> partialBoneInfoList;
        keyValuePair.Deconstruct(ref boneCategory, ref partialBoneInfoList);
        BoneCategory category = boneCategory;
        List<PartialBoneInfo> bones = partialBoneInfoList;
        BoneNodeGroup groupNode = source?.Find((Predicate<BoneNodeGroup>) (group => group.Category == category));
        int num = groupNode == null ? 1 : 0;
        if (groupNode == null)
          groupNode = this.CreateGroupNode(node.Pose, category);
        groupNode.Name = this.Locale.GetCategoryName(category);
        groupNode.Category = category;
        groupNode.SortPriority = category.SortPriority ?? -1;
        this.BindGroups((SkeletonNode) groupNode, category);
        this.BindBones((SkeletonNode) groupNode, bones);
        if (num != 0 && groupNode.Children.Any<SceneEntity>())
          node.Add((SceneEntity) groupNode);
      }
      node.OrderByPriority();
    }

    private void BindBones(SkeletonNode node, List<PartialBoneInfo> bones)
    {
      List<BoneNode> boneNodeList = (List<BoneNode>) null;
      List<SceneEntity> list = node.Children.ToList<SceneEntity>();
      if (list.Count > 0)
        boneNodeList = list.Where<SceneEntity>((Func<SceneEntity, bool>) (x => x is BoneNode boneNode1 && boneNode1.Info.PartialIndex == this.Index)).Cast<BoneNode>().ToList<BoneNode>();
      int num = (int) this.Partial.ConnectedBoneIndex + 1;
      foreach (PartialBoneInfo bone1 in bones)
      {
        PartialBoneInfo boneInfo = bone1;
        BoneNode entity = boneNodeList?.Find((Predicate<BoneNode>) (bone => bone.Info.Name == boneInfo.Name));
        if (entity != null)
        {
          if (this.Index != entity.Info.PartialIndex)
          {
            node.Remove((SceneEntity) entity);
          }
          else
          {
            entity.Info = boneInfo;
            entity.PartialId = this.PartialId;
          }
        }
        else
        {
          BoneNode boneNode2 = this.CreateBoneNode(node, boneInfo);
          boneNode2.Name = this.Locale.GetBoneName(boneInfo);
          boneNode2.SortPriority = num + boneInfo.BoneIndex;
          node.Add((SceneEntity) boneNode2);
        }
      }
      node.OrderByPriority();
    }

    private BoneNodeGroup CreateGroupNode(EntityPose pose, BoneCategory category)
    {
      string name = category.Name;
      BoneCategory boneCategory = category;
      if (boneCategory != null)
      {
        TwoJointsGroupParams twoJointsGroup = boneCategory.TwoJointsGroup;
        if (twoJointsGroup != null)
          goto label_3;
label_2:
        CcdGroupParams ccdGroup = boneCategory.CcdGroup;
        if (ccdGroup != null)
        {
          CcdGroupParams ccdGroupParams = ccdGroup;
          CcdGroup group;
          if (pose.IkController.TrySetupGroup(name, ccdGroupParams, out group))
            return (BoneNodeGroup) new IkNodeGroup<CcdGroup>(this._scene, pose, group);
          goto label_7;
        }
        goto label_7;
label_3:
        TwoJointsGroupParams jointsGroupParams = twoJointsGroup;
        TwoJointsGroup group1;
        if (pose.IkController.TrySetupGroup(name, jointsGroupParams, out group1))
          return (BoneNodeGroup) new IkNodeGroup<TwoJointsGroup>(this._scene, pose, group1);
        goto label_2;
      }
label_7:
      return new BoneNodeGroup(this._scene, pose);
    }

    private BoneNode CreateBoneNode(SkeletonNode parent, PartialBoneInfo boneInfo)
    {
      switch (parent)
      {
        case IkNodeGroup<TwoJointsGroup> ikNodeGroup1:
          if ((int) ikNodeGroup1.Group.EndBoneIndex == boneInfo.BoneIndex)
            return (BoneNode) new TwoJointEndNode(this._scene, parent.Pose, boneInfo, this.PartialId, ikNodeGroup1.Group);
          break;
        case IkNodeGroup<CcdGroup> ikNodeGroup2:
          if ((int) ikNodeGroup2.Group.EndBoneIndex == boneInfo.BoneIndex)
            return (BoneNode) new CcdEndNode(this._scene, parent.Pose, boneInfo, this.PartialId, ikNodeGroup2.Group);
          break;
      }
      return new BoneNode(this._scene, parent.Pose, boneInfo, this.PartialId);
    }
  }
}
