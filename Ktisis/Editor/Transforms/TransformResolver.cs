// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Transforms.TransformResolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Skeleton;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Editor.Transforms;

public static class TransformResolver
{
  public static SceneEntity? GetPoseTarget(IEnumerable<SceneEntity> entities)
  {
    BoneNode poseTarget = (BoneNode) null;
    foreach (BoneNode boneNode in entities.Where<SceneEntity>((Func<SceneEntity, bool>) (item => item is BoneNode)).Cast<BoneNode>())
    {
      if (poseTarget == null)
      {
        poseTarget = boneNode;
      }
      else
      {
        EntityPose pose = boneNode.Pose;
        if (pose == poseTarget.Pose)
        {
          int partialIndex1 = boneNode.Info.PartialIndex;
          PartialSkeletonInfo partialInfo = pose.GetPartialInfo(partialIndex1);
          if (partialInfo != null)
          {
            int num1 = partialIndex1;
            int partialIndex2 = poseTarget.Info.PartialIndex;
            int? nullable1;
            if (num1 == partialIndex2)
            {
              nullable1 = new int?(poseTarget.Info.BoneIndex);
            }
            else
            {
              int num2 = num1;
              int index = partialIndex2;
              int num3 = index;
              if (num2 < num3)
              {
                short? connectedParentBoneIndex = pose.GetPartialInfo(index)?.ConnectedParentBoneIndex;
                nullable1 = connectedParentBoneIndex.HasValue ? new int?((int) connectedParentBoneIndex.GetValueOrDefault()) : new int?();
              }
              else
                nullable1 = new int?();
            }
            int? nullable2 = nullable1;
            if (nullable2.HasValue && (partialInfo.IsBoneDescendantOf(nullable2.Value, boneNode.Info.BoneIndex) || poseTarget.Info.ParentIndex == boneNode.Info.ParentIndex && boneNode.Info.BoneIndex < poseTarget.Info.BoneIndex))
              poseTarget = boneNode;
          }
        }
      }
    }
    return (SceneEntity) poseTarget;
  }

  public static IEnumerable<SceneEntity> GetCorrelatingBones(
    IEnumerable<SceneEntity> entities,
    bool yieldDefault = false)
  {
    HashSet<BoneNode> unique = new HashSet<BoneNode>();
    foreach (SceneEntity entity in entities)
    {
      if (!(entity is BoneNode correlatingBone))
      {
        if (entity is SkeletonGroup skeletonGroup)
        {
          IEnumerator<BoneNode> enumerator = skeletonGroup.GetIndividualBones().Where<BoneNode>((Func<BoneNode, bool>) (bone => unique.Add(bone))).GetEnumerator();
          while (enumerator.MoveNext())
            yield return (SceneEntity) enumerator.Current;
          enumerator = (IEnumerator<BoneNode>) null;
        }
        else if (yieldDefault)
          yield return entity;
      }
      else if (unique.Add(correlatingBone))
        yield return (SceneEntity) correlatingBone;
    }
  }

  public static Dictionary<EntityPose, Dictionary<int, List<BoneNode>>> BuildPoseMap(
    SceneEntity? target,
    IEnumerable<SceneEntity> entities)
  {
    Dictionary<EntityPose, Dictionary<int, List<BoneNode>>> dictionary1 = new Dictionary<EntityPose, Dictionary<int, List<BoneNode>>>();
    foreach (BoneNode boneNode in TransformResolver.GetCorrelatingBones(entities).Cast<BoneNode>())
    {
      EntityPose pose = boneNode.Pose;
      if (pose != target)
      {
        Dictionary<int, List<BoneNode>> dictionary2;
        int num1 = dictionary1.TryGetValue(pose, out dictionary2) ? 1 : 0;
        if (dictionary2 == null)
          dictionary2 = new Dictionary<int, List<BoneNode>>();
        int partialIndex = boneNode.Info.PartialIndex;
        List<BoneNode> boneNodeList;
        int num2 = dictionary2.TryGetValue(partialIndex, out boneNodeList) ? 1 : 0;
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
