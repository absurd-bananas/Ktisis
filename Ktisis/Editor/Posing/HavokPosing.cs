// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.HavokPosing
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using FFXIVClientStructs.Havok.Common.Base.Math.Matrix;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using Ktisis.Common.Utility;
using Ktisis.Interop;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Posing;

public static class HavokPosing
{
  private static readonly Alloc<Matrix4x4> Matrix = new Alloc<Matrix4x4>(16UL /*0x10*/);

  public static unsafe Matrix4x4 GetMatrix(hkQsTransformf* transform)
  {
    ((hkQsTransformf) (IntPtr) transform).get4x4ColumnMajor((float*) HavokPosing.Matrix.Address);
    return *HavokPosing.Matrix.Data;
  }

  public static unsafe Matrix4x4 GetMatrix(hkaPose* pose, int boneIndex)
  {
    return (IntPtr) pose == IntPtr.Zero || (IntPtr) pose->ModelPose.Data == IntPtr.Zero ? Matrix4x4.Identity : HavokPosing.GetMatrix(pose->ModelPose.Data + boneIndex);
  }

  public static unsafe void SetMatrix(hkQsTransformf* trans, Matrix4x4 matrix)
  {
    *HavokPosing.Matrix.Data = matrix;
    ((hkQsTransformf) (IntPtr) trans).set((hkMatrix4f*) HavokPosing.Matrix.Address);
  }

  public static unsafe void SetMatrix(hkaPose* pose, int boneIndex, Matrix4x4 matrix)
  {
    HavokPosing.SetMatrix(pose->ModelPose.Data + boneIndex, matrix);
  }

  public static unsafe Transform? GetModelTransform(hkaPose* pose, int boneIx)
  {
    return (IntPtr) pose == IntPtr.Zero || (IntPtr) pose->ModelPose.Data == IntPtr.Zero || boneIx < 0 || boneIx > pose->ModelPose.Length ? (Transform) null : new Transform(HavokPosing.GetMatrix(pose->ModelPose.Data + boneIx));
  }

  public static unsafe void SetModelTransform(hkaPose* pose, int boneIx, Transform trans)
  {
    if ((IntPtr) pose == IntPtr.Zero || (IntPtr) pose->ModelPose.Data == IntPtr.Zero || boneIx < 0 || boneIx > pose->ModelPose.Length)
      return;
    HavokPosing.SetMatrix(pose->ModelPose.Data + boneIx, trans.ComposeMatrix());
  }

  public static unsafe Transform? GetLocalTransform(hkaPose* pose, int boneIx)
  {
    return (IntPtr) pose == IntPtr.Zero || (IntPtr) pose->LocalPose.Data == IntPtr.Zero || boneIx < 0 || boneIx > pose->LocalPose.Length ? (Transform) null : new Transform(HavokPosing.GetMatrix(pose->LocalPose.Data + boneIx));
  }

  public static unsafe void Propagate(
    Skeleton* skele,
    int partialIx,
    int boneIx,
    Transform target,
    Transform initial,
    bool propagatePartials = true)
  {
    PartialSkeleton partialSkeleton1 = skele->PartialSkeletons[partialIx];
    hkaPose* havokPose1 = ((PartialSkeleton) ref partialSkeleton1).GetHavokPose(0);
    if ((IntPtr) havokPose1 == IntPtr.Zero || (IntPtr) havokPose1->Skeleton == IntPtr.Zero)
      return;
    Vector3 position = target.Position;
    Vector3 deltaPos = position - initial.Position;
    Quaternion deltaRot = target.Rotation / initial.Rotation;
    HavokPosing.Propagate(havokPose1, boneIx, position, deltaPos, deltaRot);
    if (partialIx != 0 || !propagatePartials)
      return;
    hkaSkeleton* skeleton1 = havokPose1->Skeleton;
    for (int index = 0; index < (int) skele->PartialSkeletonCount; ++index)
    {
      PartialSkeleton partialSkeleton2 = skele->PartialSkeletons[index];
      if (!((PartialSkeleton) ref partialSkeleton2).HavokPoses.IsEmpty)
      {
        hkaPose* havokPose2 = ((PartialSkeleton) ref partialSkeleton2).GetHavokPose(0);
        if ((IntPtr) havokPose2 != IntPtr.Zero)
        {
          hkaSkeleton* skeleton2 = havokPose2->Skeleton;
          if (!HavokPosing.IsMultiRootSkeleton(skeleton2->ParentIndices))
          {
            short connectedBoneIndex = partialSkeleton2.ConnectedBoneIndex;
            short connectedParentBoneIndex = partialSkeleton2.ConnectedParentBoneIndex;
            if ((int) connectedParentBoneIndex == boneIx || HavokPosing.IsBoneDescendantOf(skeleton1->ParentIndices, (int) connectedParentBoneIndex, boneIx))
              HavokPosing.Propagate(havokPose2, (int) connectedBoneIndex, position, deltaPos, deltaRot);
          }
          else
          {
            foreach (int multiRoot in HavokPosing.GetMultiRoots(skeleton2->ParentIndices))
            {
              short boneNameIndex = HavokPosing.TryGetBoneNameIndex(havokPose1, ((hkStringPtr) ref skeleton2->Bones[multiRoot].Name).String);
              if (((((hkStringPtr) ref skeleton1->Bones[boneIx].Name).String == ((hkStringPtr) ref skeleton2->Bones[multiRoot].Name).String ? 1 : 0) | (boneNameIndex != (short) -1 ? (HavokPosing.IsBoneDescendantOf(skeleton1->ParentIndices, (int) boneNameIndex, boneIx) ? 1 : 0) : (false ? 1 : 0))) != 0)
                HavokPosing.Propagate(havokPose2, multiRoot, position, deltaPos, deltaRot);
            }
          }
        }
      }
    }
  }

  private static unsafe void Propagate(
    hkaPose* pose,
    int boneIx,
    Vector3 sourcePos,
    Vector3 deltaPos,
    Quaternion deltaRot)
  {
    hkaSkeleton* skeleton = pose->Skeleton;
    for (int index = boneIx; index < skeleton->Bones.Length; ++index)
    {
      if (HavokPosing.IsBoneDescendantOf(skeleton->ParentIndices, index, boneIx))
      {
        Transform modelTransform = HavokPosing.GetModelTransform(pose, index);
        Matrix4x4 scale = Matrix4x4.CreateScale(modelTransform.Scale);
        Matrix4x4 fromQuaternion = Matrix4x4.CreateFromQuaternion(deltaRot * modelTransform.Rotation);
        Matrix4x4 translation = Matrix4x4.CreateTranslation(deltaPos + sourcePos + Vector3.Transform(modelTransform.Position - sourcePos, deltaRot));
        HavokPosing.SetMatrix(pose, index, scale * fromQuaternion * translation);
      }
    }
  }

  public static unsafe Quaternion ParentSkeleton(Skeleton* modelSkeleton, int partialIndex)
  {
    PartialSkeleton partialSkeleton1 = modelSkeleton->PartialSkeletons[partialIndex];
    hkaPose* havokPose1 = ((PartialSkeleton) ref partialSkeleton1).GetHavokPose(0);
    if ((IntPtr) havokPose1 == IntPtr.Zero)
      return Quaternion.Identity;
    PartialSkeleton partialSkeleton2 = *modelSkeleton->PartialSkeletons;
    hkaPose* havokPose2 = ((PartialSkeleton) ref partialSkeleton2).GetHavokPose(0);
    if ((IntPtr) havokPose2 == IntPtr.Zero)
      return Quaternion.Identity;
    Transform modelTransform1 = HavokPosing.GetModelTransform(havokPose1, (int) partialSkeleton1.ConnectedBoneIndex);
    Transform modelTransform2 = HavokPosing.GetModelTransform(havokPose2, (int) partialSkeleton1.ConnectedParentBoneIndex);
    Quaternion quaternion = modelTransform2.Rotation / modelTransform1.Rotation;
    Transform transform1 = new Transform(modelTransform2.Position, modelTransform1.Rotation, modelTransform1.Scale);
    HavokPosing.SetModelTransform(havokPose1, (int) partialSkeleton1.ConnectedBoneIndex, transform1);
    HavokPosing.Propagate(modelSkeleton, partialIndex, (int) partialSkeleton1.ConnectedBoneIndex, transform1, modelTransform1);
    Transform transform2 = new Transform(modelTransform2.Position, quaternion * modelTransform1.Rotation, modelTransform2.Scale);
    HavokPosing.SetModelTransform(havokPose1, (int) partialSkeleton1.ConnectedBoneIndex, transform2);
    HavokPosing.Propagate(modelSkeleton, partialIndex, (int) partialSkeleton1.ConnectedBoneIndex, transform2, transform1);
    return quaternion;
  }

  public static unsafe void SyncModelSpace(Skeleton* skeleton, int partialIndex)
  {
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return;
    PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[partialIndex];
    hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
    if ((IntPtr) havokPose == IntPtr.Zero || (IntPtr) havokPose->Skeleton == IntPtr.Zero)
      return;
    for (int boneIx = 1; boneIx < havokPose->Skeleton->Bones.Length; ++boneIx)
    {
      Transform modelTransform1 = HavokPosing.GetModelTransform(havokPose, (int) havokPose->Skeleton->ParentIndices[boneIx]);
      if (modelTransform1 != null)
      {
        Transform localTransform = HavokPosing.GetLocalTransform(havokPose, boneIx);
        Transform modelTransform2 = HavokPosing.GetModelTransform(havokPose, boneIx);
        modelTransform2.Position = modelTransform1.Position + Vector3.Transform(localTransform.Position, modelTransform1.Rotation);
        modelTransform2.Rotation = modelTransform1.Rotation * localTransform.Rotation;
        HavokPosing.SetModelTransform(havokPose, boneIx, modelTransform2);
      }
    }
    if (partialIndex <= 0)
      return;
    HavokPosing.ParentSkeleton(skeleton, partialIndex);
  }

  public static unsafe short TryGetBoneNameIndex(hkaPose* pose, string? name)
  {
    if ((IntPtr) pose == IntPtr.Zero || (IntPtr) pose->Skeleton == IntPtr.Zero || StringExtensions.IsNullOrEmpty(name))
      return -1;
    hkArray<hkaBone> bones = pose->Skeleton->Bones;
    for (short boneNameIndex = 0; (int) boneNameIndex < bones.Length; ++boneNameIndex)
    {
      if (((hkStringPtr) ref bones[(int) boneNameIndex].Name).String == name)
        return boneNameIndex;
    }
    return -1;
  }

  public static bool IsBoneDescendantOf(hkArray<short> indices, int bone, int parent)
  {
    if (!HavokPosing.IsMultiRootSkeleton(indices) && parent < 1)
      return true;
    for (short index = indices[bone]; index != (short) -1; index = indices[(int) index])
    {
      if ((int) index == parent)
        return true;
    }
    return false;
  }

  public static bool IsMultiRootSkeleton(hkArray<short> indices)
  {
    return HavokPosing.GetMultiRoots(indices).Count > 1;
  }

  public static List<int> GetMultiRoots(hkArray<short> indices)
  {
    List<int> multiRoots = new List<int>();
    for (int index = 0; index < indices.Length; ++index)
    {
      if (indices[index] == (short) -1)
        multiRoots.Add(index);
    }
    return multiRoots;
  }
}
