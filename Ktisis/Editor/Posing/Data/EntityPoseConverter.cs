// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.EntityPoseConverter
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using Ktisis.Common.Utility;
using Ktisis.Data.Files;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Posing.Data;

public class EntityPoseConverter(EntityPose target)
{
  public bool IsPoseValid => target.IsValid;

  public unsafe PoseContainer Save()
  {
    PoseContainer poseContainer = new PoseContainer();
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton != IntPtr.Zero)
      poseContainer.Store(skeleton);
    return poseContainer;
  }

  public PoseFile SaveFile()
  {
    return new PoseFile() { Bones = this.Save() };
  }

  public unsafe void Load(
    PoseContainer pose,
    PoseTransforms transforms,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    pose.Apply(skeleton, transforms, boneTypeInclusion);
  }

  public unsafe void LoadPartial(
    PoseContainer pose,
    int partialIndex,
    PoseTransforms transforms,
    BoneTypeInclusion boneTypeInclusion)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    pose.ApplyToPartial(skeleton, partialIndex, transforms, boneTypeInclusion);
  }

  public unsafe void LoadBones(
    PoseContainer pose,
    IEnumerable<PartialBoneInfo> bones,
    PoseTransforms transforms,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    pose.ApplyToBones(skeleton, bones, transforms, boneTypeInclusion);
  }

  public void LoadSelectedBones(
    PoseContainer pose,
    PoseTransforms transforms,
    bool selectedBonesIncludeChildren,
    BoneTypeInclusion boneTypeInclusion)
  {
    IEnumerable<PartialBoneInfo> selectedBones = this.GetSelectedBones(selectedBonesIncludeChildren);
    this.LoadBones(pose, selectedBones, transforms, boneTypeInclusion);
  }

  public void LoadUnselectedBones(
    PoseContainer pose,
    PoseTransforms transforms,
    bool selectedBonesIncludeChildren,
    BoneTypeInclusion boneTypeInclusion)
  {
    IEnumerable<PartialBoneInfo> bones1 = this.GetBones();
    IEnumerable<PartialBoneInfo> selectedBones = this.GetSelectedBones(selectedBonesIncludeChildren);
    List<PartialBoneInfo> bones2 = new List<PartialBoneInfo>();
    foreach (PartialBoneInfo partialBoneInfo in bones1)
    {
      PartialBoneInfo b = partialBoneInfo;
      if (selectedBones.FirstOrDefault<PartialBoneInfo>((Func<PartialBoneInfo, bool>) (x => x.Name == b.Name)) == null)
        bones2.Add(b);
    }
    this.LoadBones(pose, (IEnumerable<PartialBoneInfo>) bones2, transforms, boneTypeInclusion);
  }

  public unsafe void LoadReferencePose()
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    for (int partialIndex = 0; partialIndex < (int) skeleton->PartialSkeletonCount; ++partialIndex)
    {
      PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[partialIndex];
      hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
      if ((IntPtr) havokPose != IntPtr.Zero)
      {
        ((hkaPose) (IntPtr) havokPose).SetToReferencePose();
        HavokPosing.SyncModelSpace(skeleton, partialIndex);
      }
    }
  }

  public unsafe PoseContainer FlipBones(bool flipYawCorrection, bool flipRotationCorrect)
  {
    PoseContainer poseContainer = new PoseContainer();
    List<PartialBoneInfo> list = this.GetBones().ToList<PartialBoneInfo>();
    foreach (PartialBoneInfo partialBoneInfo in list)
      poseContainer[partialBoneInfo.Name] = new Transform();
    if (list.All<PartialBoneInfo>((Func<PartialBoneInfo, bool>) (bone => bone.PartialIndex == 0)))
      return poseContainer;
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return poseContainer;
    PartialBoneInfo[] array = list.ToArray();
    PartialBoneInfo partialBoneInfo1 = array[0];
    Quaternion rotation = HavokPosing.GetModelTransform(((PartialSkeleton) (IntPtr) (skeleton->PartialSkeletons + partialBoneInfo1.PartialIndex)).GetHavokPose(0), partialBoneInfo1.BoneIndex).Rotation;
    poseContainer.ApplyFlipToBones(skeleton);
    poseContainer.MainBoneYReset(skeleton, array[0], rotation, flipYawCorrection, flipRotationCorrect);
    return poseContainer;
  }

  public unsafe PoseContainer FilterSelectedBones(
    PoseContainer pose,
    bool selectedBonesIncludeChildren,
    bool all = true)
  {
    PoseContainer poseContainer = new PoseContainer();
    List<PartialBoneInfo> list = this.GetSelectedBones(selectedBonesIncludeChildren, all).ToList<PartialBoneInfo>();
    foreach (PartialBoneInfo partialBoneInfo in list)
    {
      Transform transform;
      if (pose.TryGetValue(partialBoneInfo.Name, out transform))
        poseContainer[partialBoneInfo.Name] = transform;
    }
    if (list.All<PartialBoneInfo>((Func<PartialBoneInfo, bool>) (bone => bone.PartialIndex == 0)))
      return poseContainer;
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return poseContainer;
    for (int index = 1; index < (int) skeleton->PartialSkeletonCount; ++index)
    {
      PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[index];
      hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
      if ((IntPtr) havokPose != IntPtr.Zero && (IntPtr) havokPose->Skeleton != IntPtr.Zero)
      {
        string key = ((hkStringPtr) ref havokPose->Skeleton->Bones[(int) partialSkeleton.ConnectedBoneIndex].Name).String;
        Transform transform;
        if (!StringExtensions.IsNullOrEmpty(key) && !poseContainer.ContainsKey(key) && pose.TryGetValue(key, out transform))
          poseContainer[key] = transform;
      }
    }
    return poseContainer;
  }

  public IEnumerable<PartialBoneInfo> IntersectBonesByName(IEnumerable<PartialBoneInfo> second)
  {
    return Enumerable.IntersectBy<PartialBoneInfo, string>(this.GetBones(), second.Select<PartialBoneInfo, string>((Func<PartialBoneInfo, string>) (bone => bone.Name)), (Func<PartialBoneInfo, string>) (bone => bone.Name));
  }

  private unsafe IEnumerable<PartialBoneInfo> GetBones()
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return (IEnumerable<PartialBoneInfo>) Array.Empty<PartialBoneInfo>();
    List<PartialBoneInfo> bones = new List<PartialBoneInfo>();
    bones.AddRange(this.GetPartialBones(0));
    for (int index = 0; index < (int) skeleton->PartialSkeletonCount; ++index)
      bones.AddRange(this.GetPartialBones(index));
    return (IEnumerable<PartialBoneInfo>) bones;
  }

  private unsafe IEnumerable<PartialBoneInfo> GetPartialBones(int index)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = target.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return (IEnumerable<PartialBoneInfo>) Array.Empty<PartialBoneInfo>();
    PartialSkeleton partial = skeleton->PartialSkeletons[index];
    Span<ulong> havokPoses = ((PartialSkeleton) ref partial).HavokPoses;
    if (!havokPoses.IsEmpty)
    {
      havokPoses = ((PartialSkeleton) ref partial).HavokPoses;
      if (havokPoses[0] != 0UL)
        return new BoneEnumerator(index, partial).EnumerateBones();
    }
    return (IEnumerable<PartialBoneInfo>) Array.Empty<PartialBoneInfo>();
  }

  public IEnumerable<PartialBoneInfo> GetSelectedBones(bool selectedBonesIncludeChildren, bool all = true)
  {
    IEnumerable<SkeletonNode> nodes;
    if (!selectedBonesIncludeChildren)
    {
      nodes = target.Recurse().Prepend<SceneEntity>((SceneEntity) target).Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is SkeletonNode && entity.IsSelected)).Cast<SkeletonNode>();
    }
    else
    {
      List<SkeletonNode> list1 = target.Recurse().Prepend<SceneEntity>((SceneEntity) target).OfType<SkeletonNode>().ToList<SkeletonNode>();
      List<SkeletonNode> list2 = list1.Where<SkeletonNode>((Func<SkeletonNode, bool>) (entity => entity.IsSelected)).ToList<SkeletonNode>();
      HashSet<SkeletonNode> skeletonNodeSet = new HashSet<SkeletonNode>();
      foreach (SkeletonNode node in list2)
      {
        skeletonNodeSet.Add(node);
        this.AddDescendantsToSet(node, list1, skeletonNodeSet);
      }
      nodes = skeletonNodeSet.AsEnumerable<SkeletonNode>();
    }
    return this.GetBoneSelectionFrom(nodes, all).Distinct<PartialBoneInfo>();
  }

  public IEnumerable<PartialBoneInfo> GetUnselectedBones(
    bool selectedBonesIncludeChildren,
    bool all = true)
  {
    IEnumerable<SkeletonNode> nodes;
    if (!selectedBonesIncludeChildren)
    {
      nodes = target.Recurse().Prepend<SceneEntity>((SceneEntity) target).Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is SkeletonNode && !entity.IsSelected)).Cast<SkeletonNode>();
    }
    else
    {
      List<SkeletonNode> list1 = target.Recurse().Prepend<SceneEntity>((SceneEntity) target).OfType<SkeletonNode>().ToList<SkeletonNode>();
      List<SkeletonNode> list2 = list1.Where<SkeletonNode>((Func<SkeletonNode, bool>) (entity => !entity.IsSelected)).ToList<SkeletonNode>();
      HashSet<SkeletonNode> skeletonNodeSet = new HashSet<SkeletonNode>();
      foreach (SkeletonNode node in list2)
      {
        skeletonNodeSet.Add(node);
        this.AddDescendantsToSet(node, list1, skeletonNodeSet);
      }
      nodes = skeletonNodeSet.AsEnumerable<SkeletonNode>();
    }
    return this.GetBoneSelectionFrom(nodes, all).Distinct<PartialBoneInfo>();
  }

  private void AddDescendantsToSet(
    SkeletonNode node,
    List<SkeletonNode> allBones,
    HashSet<SkeletonNode> selectedBonesSet)
  {
    switch (node)
    {
      case BoneNode node1:
        using (List<SkeletonNode>.Enumerator enumerator = allBones.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            if (enumerator.Current is BoneNode current && current.IsBoneDescendantOf(node1))
              selectedBonesSet.Add((SkeletonNode) current);
          }
          break;
        }
      case SkeletonGroup skeletonGroup:
        using (IEnumerator<BoneNode> enumerator = skeletonGroup.GetAllBones().GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            BoneNode current = enumerator.Current;
            selectedBonesSet.Add((SkeletonNode) current);
            this.AddDescendantsToSet((SkeletonNode) current, allBones, selectedBonesSet);
          }
          break;
        }
    }
  }

  private IEnumerable<PartialBoneInfo> GetBoneSelectionFrom(
    IEnumerable<SkeletonNode> nodes,
    bool all = true)
  {
    foreach (SkeletonNode node in nodes)
    {
      if (!(node is BoneNode boneNode))
      {
        if (node is SkeletonGroup skeletonGroup)
        {
          IEnumerator<PartialBoneInfo> enumerator = this.GetBoneSelectionFrom(all ? (IEnumerable<SkeletonNode>) skeletonGroup.GetAllBones() : (IEnumerable<SkeletonNode>) skeletonGroup.GetIndividualBones()).GetEnumerator();
          while (enumerator.MoveNext())
            yield return enumerator.Current;
          enumerator = (IEnumerator<PartialBoneInfo>) null;
        }
      }
      else
        yield return boneNode.Info;
    }
  }
}
