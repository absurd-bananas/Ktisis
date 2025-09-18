// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.Constraints.TwoJointEndNode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Types;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton.Constraints;

public class TwoJointEndNode : IkEndNode, ITwoJointsNode, IIkNode
{
  public TwoJointsGroup Group { get; }

  public TwoJointEndNode(
    ISceneManager scene,
    EntityPose pose,
    PartialBoneInfo bone,
    uint partialId,
    TwoJointsGroup group)
    : base(scene, pose, bone, partialId)
  {
    this.Group = group;
  }

  protected override bool IsOverride => this.IsEnabled && this.Group.Mode == TwoJointsMode.Fixed;

  public override Transform GetTransformTarget(Transform offset, Transform world)
  {
    offset.Position += this.Group.TargetPosition.ModelToWorldPos(offset);
    offset.Rotation *= this.Group.TargetRotation;
    offset.Scale = world.Scale;
    return offset;
  }

  public override unsafe void SetTransformTarget(
    Transform transform,
    Transform offset,
    Transform world)
  {
    if ((IntPtr) this.Pose.GetSkeleton() == IntPtr.Zero)
      return;
    bool flag = false;
    if (this.Group.EnforcePosition)
    {
      this.Group.TargetPosition = transform.Position.WorldToModelPos(offset);
    }
    else
    {
      world.Position = transform.Position;
      flag = true;
    }
    if (this.Group.EnforceRotation)
    {
      this.Group.TargetRotation = Quaternion.Inverse(offset.Rotation) * transform.Rotation;
    }
    else
    {
      world.Rotation = transform.Rotation;
      flag = true;
    }
    if (!world.Scale.Equals(transform.Scale))
    {
      world.Scale = transform.Scale;
      flag = true;
    }
    if (!flag)
      return;
    this.SetTransformWorld(world);
  }
}
