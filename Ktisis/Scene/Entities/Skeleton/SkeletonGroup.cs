// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.SkeletonGroup
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton;

public abstract class SkeletonGroup(ISceneManager scene) : SkeletonNode(scene), IVisibility
{
  public bool Visible
  {
    get => this.RecurseVisible().All<IVisibility>((Func<IVisibility, bool>) (vis => vis.Visible));
    set
    {
      foreach (IVisibility visibility in this.RecurseVisible())
        visibility.Visible = value;
    }
  }

  private IEnumerable<IVisibility> RecurseVisible()
  {
    return this.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (child => child is IVisibility)).Cast<IVisibility>();
  }

  protected void Clean(int pIndex, uint pId)
  {
    foreach (SceneEntity sceneEntity in this.GetChildren().Where<SceneEntity>((Func<SceneEntity, bool>) (item =>
    {
      switch (item)
      {
        case BoneNodeGroup boneNodeGroup2:
          boneNodeGroup2.Clean(pIndex, pId);
          return boneNodeGroup2.IsStale();
        case BoneNode boneNode2:
          return boneNode2.Info.PartialIndex == pIndex && (int) boneNode2.PartialId != (int) pId;
        default:
          return false;
      }
    })).ToList<SceneEntity>())
      sceneEntity.Remove();
  }

  public IEnumerable<BoneNode> GetAllBones()
  {
    SkeletonGroup skeletonGroup1 = this;
    HashSet<BoneNode> unique = new HashSet<BoneNode>();
    foreach (SceneEntity child in skeletonGroup1.Children)
    {
      if (!(child is BoneNode allBone))
      {
        if (child is SkeletonGroup skeletonGroup2)
        {
          IEnumerator<BoneNode> enumerator = skeletonGroup2.GetAllBones().GetEnumerator();
          while (enumerator.MoveNext())
            yield return enumerator.Current;
          enumerator = (IEnumerator<BoneNode>) null;
        }
      }
      else if (unique.Add(allBone))
        yield return allBone;
    }
  }

  public IEnumerable<BoneNode> GetIndividualBones()
  {
    List<BoneNode> results = new List<BoneNode>();
    foreach (SceneEntity sceneEntity in this.Recurse())
    {
      if (!(sceneEntity is BoneNodeGroup boneNodeGroup))
      {
        if (sceneEntity is BoneNode boneNode)
          results.Add(boneNode);
      }
      else
        results.AddRange(boneNodeGroup.GetIndividualBones());
    }
    EntityPose pose = this.Pose;
    results = results.Distinct<BoneNode>().ToList<BoneNode>();
    results.RemoveAll((Predicate<BoneNode>) (bone =>
    {
      int boneIndex = bone.Info.BoneIndex;
      int partialIx = bone.Info.PartialIndex;
      PartialSkeletonInfo partialInfo1 = pose.GetPartialInfo(partialIx);
      if (partialInfo1 == null)
        return false;
      if (partialInfo1.GetParentsOf(boneIndex).Any<short>((Func<short, bool>) (parentId => results.Any<BoneNode>((Func<BoneNode, bool>) (x => x.MatchesId(partialIx, (int) parentId))))))
        return true;
      if (partialIx == 0)
        return false;
      PartialSkeletonInfo partialInfo2 = pose.GetPartialInfo(0);
      if (partialInfo2 == null)
        return false;
      short connectedParentBoneIndex = partialInfo1.ConnectedParentBoneIndex;
      return partialInfo2.GetParentsOf((int) connectedParentBoneIndex).Prepend<short>(connectedParentBoneIndex).Any<short>((Func<short, bool>) (id => results.Any<BoneNode>((Func<BoneNode, bool>) (x => x.MatchesId(0, (int) id)))));
    }));
    return (IEnumerable<BoneNode>) results;
  }
}
