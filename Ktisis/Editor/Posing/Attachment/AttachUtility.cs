// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Attachment.AttachUtility
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Havok.Animation.Rig;
using Ktisis.Common.Utility;
using Ktisis.Structs.Animation;
using Ktisis.Structs.Attachment;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Posing.Attachment;

public static class AttachUtility
{
  public static unsafe void SetBoneAttachment(
    Skeleton* parent,
    Skeleton* child,
    Attach* attach,
    ushort parentBoneId,
    ushort childBoneId = 0)
  {
    if (parent == child)
      throw new Exception("Attempting to parent attachment point to itself.");
    int num = attach->Count > 0U ? 1 : 0;
    attach->Type = AttachType.BoneIndex;
    attach->Count = 1U;
    attach->Parent = (void*) parent;
    attach->Child = child;
    attach->Param->ParentId = parentBoneId;
    attach->Param->ChildId = childBoneId;
    if (num != 0)
      return;
    attach->Param->Transform = (Transform) new Transform();
  }

  public static unsafe bool TryGetParentBoneIndex(Attach* attach, out ushort index)
  {
    index = attach->Param->ParentId;
    bool parentBoneIndex;
    switch (attach->Type)
    {
      case AttachType.ElementId:
        parentBoneIndex = ((SkeletonEx*) attach->GetParentSkeleton())->TryGetBoneIndexForElementId((uint) index, out index);
        break;
      case AttachType.BoneIndex:
        parentBoneIndex = true;
        break;
      default:
        parentBoneIndex = false;
        break;
    }
    return parentBoneIndex;
  }

  public static unsafe void SetTransformRelative(
    Attach* attach,
    Transform target,
    Transform source)
  {
    Skeleton* parentSkeleton = attach->GetParentSkeleton();
    if ((IntPtr) parentSkeleton == IntPtr.Zero || (IntPtr) parentSkeleton->PartialSkeletons == IntPtr.Zero || ((PartialSkeleton) (IntPtr) parentSkeleton->PartialSkeletons).HavokPoses.IsEmpty)
      return;
    hkaPose* havokPose = ((PartialSkeleton) (IntPtr) parentSkeleton->PartialSkeletons).GetHavokPose(0);
    ushort index1;
    if ((IntPtr) havokPose == IntPtr.Zero || !AttachUtility.TryGetParentBoneIndex(attach, out index1))
      return;
    Quaternion quaternion = Quaternion.Identity;
    if (attach->Type == AttachType.ElementId)
    {
      SkeletonEx* skeletonExPtr = (SkeletonEx*) parentSkeleton;
      for (int index2 = 0; (long) index2 < (long) skeletonExPtr->ElementCount; ++index2)
      {
        ElementParam* elementParamPtr = skeletonExPtr->ElementParam + index2;
        if ((int) (ushort) elementParamPtr->ElementId == (int) attach->Param->ParentId)
          quaternion = (elementParamPtr->Rotation * MathHelpers.Rad2Deg).EulerAnglesToQuaternion();
      }
    }
    Transform modelTransform = HavokPosing.GetModelTransform(havokPose, (int) index1);
    Quaternion rotation = Quaternion.Inverse(Quaternion.op_Implicit(parentSkeleton->Transform.Rotation) * modelTransform.Rotation * quaternion);
    Transform transform = new Transform(attach->Param->Transform);
    transform.Position += Vector3.Transform(target.Position - source.Position, rotation);
    transform.Rotation = rotation * target.Rotation;
    attach->Param->Transform = (Transform) transform;
  }

  public static unsafe void Detach(Attach* attach)
  {
    attach->Type = AttachType.None;
    attach->Count = 0U;
    attach->Parent = (void*) null;
    attach->Child = (Skeleton*) null;
    if ((IntPtr) attach->Param == IntPtr.Zero)
      return;
    attach->Param->ParentId = ushort.MaxValue;
    attach->Param->ChildId = ushort.MaxValue;
  }
}
