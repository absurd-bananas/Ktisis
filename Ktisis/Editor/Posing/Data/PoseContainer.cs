// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.PoseContainer
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Editor.Posing.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Posing.Data;

[Serializable]
public class PoseContainer : Dictionary<string, Transform>
{
  public unsafe void Store(Skeleton* modelSkeleton)
  {
    if ((IntPtr) modelSkeleton == IntPtr.Zero)
      return;
    this.Clear();
    ushort partialSkeletonCount = modelSkeleton->PartialSkeletonCount;
    PartialSkeleton* partialSkeletons = modelSkeleton->PartialSkeletons;
    for (int index1 = 0; index1 < (int) partialSkeletonCount; ++index1)
    {
      PartialSkeleton partialSkeleton = partialSkeletons[index1];
      hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
      if ((IntPtr) havokPose != IntPtr.Zero && (IntPtr) havokPose->Skeleton != IntPtr.Zero)
      {
        hkaSkeleton* skeleton = havokPose->Skeleton;
        for (int index2 = 0; index2 < skeleton->Bones.Length; ++index2)
        {
          if (index2 != (int) partialSkeleton.ConnectedBoneIndex)
          {
            string key = ((hkStringPtr) ref skeleton->Bones[index2].Name).String;
            if (!StringExtensions.IsNullOrEmpty(key))
              this[key] = new Transform(havokPose->ModelPose[index2]);
          }
        }
      }
    }
  }

  public unsafe void Apply(
    Skeleton* modelSkeleton,
    PoseTransforms transforms = PoseTransforms.Rotation,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    if ((IntPtr) modelSkeleton == IntPtr.Zero)
      return;
    for (int partialIndex = 0; partialIndex < (int) modelSkeleton->PartialSkeletonCount; ++partialIndex)
      this.ApplyToPartial(modelSkeleton, partialIndex, transforms, boneTypeInclusion);
  }

  public unsafe void ApplyToBones(
    Skeleton* modelSkeleton,
    IEnumerable<PartialBoneInfo> bones,
    PoseTransforms transforms = PoseTransforms.Rotation,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
    foreach (PartialBoneInfo bone in bones)
    {
      int partialIndex = bone.PartialIndex;
      List<int> intList;
      if (!dictionary.TryGetValue(partialIndex, out intList))
      {
        intList = new List<int>();
        dictionary.Add(partialIndex, intList);
      }
      intList.Add(bone.BoneIndex);
    }
    for (int index = 0; index < (int) modelSkeleton->PartialSkeletonCount; ++index)
    {
      List<int> bones1;
      if (dictionary.TryGetValue(index, out bones1))
        this.ApplyToPartialBones(modelSkeleton, index, (IEnumerable<int>) bones1, transforms, boneTypeInclusion);
    }
  }

  public unsafe void ApplyToPartial(
    Skeleton* modelSkeleton,
    int partialIndex,
    PoseTransforms transforms = PoseTransforms.Rotation,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    PartialSkeleton partialSkeleton = modelSkeleton->PartialSkeletons[partialIndex];
    hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
    if ((IntPtr) havokPose == IntPtr.Zero || (IntPtr) havokPose->Skeleton == IntPtr.Zero)
      return;
    this.ApplyToPartialBones(modelSkeleton, partialIndex, Enumerable.Range(1, havokPose->Skeleton->Bones.Length - 1), transforms, boneTypeInclusion);
  }

  public unsafe void ApplyToPartialBones(
    Skeleton* modelSkeleton,
    int partialIndex,
    IEnumerable<int> bones,
    PoseTransforms transforms = PoseTransforms.Rotation,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    if ((IntPtr) modelSkeleton == IntPtr.Zero)
      return;
    PartialSkeleton partialSkeleton = modelSkeleton->PartialSkeletons[partialIndex];
    hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
    if ((IntPtr) havokPose == IntPtr.Zero || (IntPtr) havokPose->Skeleton == IntPtr.Zero)
      return;
    hkaSkeleton* skeleton = havokPose->Skeleton;
    Quaternion offset = Quaternion.Identity;
    if (partialIndex > 0)
    {
      Quaternion quaternion1 = HavokPosing.ParentSkeleton(modelSkeleton, partialIndex);
      short connectedBoneIndex = partialSkeleton.ConnectedBoneIndex;
      Quaternion quaternion2 = havokPose->ModelPose[(int) connectedBoneIndex].Rotation.ToQuaternion();
      string key = ((hkStringPtr) ref skeleton->Bones[(int) connectedBoneIndex].Name).String;
      Transform transform;
      if (!StringExtensions.IsNullOrEmpty(key) && this.TryGetValue(key, out transform))
        offset = quaternion2 / transform.Rotation / quaternion1;
      else
        Ktisis.Ktisis.Log.Warning($"Failed to find parent bone '{key}' for partial {partialIndex}!", Array.Empty<object>());
    }
    foreach (int boneIndex in Enumerable.Range(1, skeleton->Bones.Length - 1).Intersect<int>(bones))
      this.ApplyToBone(modelSkeleton, havokPose, partialIndex, boneIndex, offset, transforms, boneTypeInclusion);
  }

  public unsafe void ApplyToBone(
    Skeleton* modelSkeleton,
    hkaPose* pose,
    int partialIndex,
    int boneIndex,
    Quaternion offset,
    PoseTransforms transforms = PoseTransforms.Rotation,
    BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both)
  {
    string key = ((hkStringPtr) ref pose->Skeleton->Bones[boneIndex].Name).String;
    Transform transform1;
    if (StringExtensions.IsNullOrEmpty(key) || boneTypeInclusion == BoneTypeInclusion.Body && (key == "j_ago" || key.StartsWith("j_f_") || key.StartsWith("n_f_")) || boneTypeInclusion == BoneTypeInclusion.Face && key != "j_kao" && key != "j_ago" && !key.StartsWith("j_f_") && !key.StartsWith("n_f_") || !this.TryGetValue(key, out transform1))
      return;
    Transform modelTransform = HavokPosing.GetModelTransform(pose, boneIndex);
    Transform transform2 = new Transform(modelTransform.Position, modelTransform.Rotation, modelTransform.Scale);
    bool flag = partialIndex == 0 && boneIndex == 1 && transforms.HasFlag((Enum) PoseTransforms.PositionRoot);
    if (flag)
      modelTransform.Rotation = offset * transform1.Rotation;
    if (transforms.HasFlag((Enum) PoseTransforms.Position) | flag)
      transform2.Position = transform1.Position;
    if (transforms.HasFlag((Enum) PoseTransforms.Rotation))
      transform2.Rotation = offset * transform1.Rotation;
    if (transforms.HasFlag((Enum) PoseTransforms.Scale))
      transform2.Scale = transform1.Scale;
    HavokPosing.SetModelTransform(pose, boneIndex, transform2);
    HavokPosing.Propagate(modelSkeleton, partialIndex, boneIndex, transform2, modelTransform);
  }

  public unsafe void MainBoneYReset(
    Skeleton* modelSkeleton,
    PartialBoneInfo mainBone,
    Quaternion originalRotation,
    bool flipYawCorrection,
    bool flipRotationCorrect)
  {
    if ((IntPtr) modelSkeleton == IntPtr.Zero || !flipYawCorrection && !flipRotationCorrect)
      return;
    PartialSkeleton partialSkeleton = modelSkeleton->PartialSkeletons[mainBone.PartialIndex];
    hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
    if ((IntPtr) havokPose == IntPtr.Zero || (IntPtr) havokPose->Skeleton == IntPtr.Zero)
      return;
    Transform modelTransform = HavokPosing.GetModelTransform(havokPose, mainBone.BoneIndex);
    Transform transform = new Transform(modelTransform.Position, modelTransform.Rotation, modelTransform.Scale);
    if (flipYawCorrection)
    {
      Quaternion fromAxisAngle = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.Atan2((float) (2.0 * ((double) originalRotation.W * (double) originalRotation.Y + (double) originalRotation.X * (double) originalRotation.Z)), (float) (1.0 - 2.0 * ((double) originalRotation.Y * (double) originalRotation.Y + (double) originalRotation.Z * (double) originalRotation.Z))) - MathF.Atan2((float) (2.0 * ((double) modelTransform.Rotation.W * (double) modelTransform.Rotation.Y + (double) modelTransform.Rotation.X * (double) modelTransform.Rotation.Z)), (float) (1.0 - 2.0 * ((double) modelTransform.Rotation.Y * (double) modelTransform.Rotation.Y + (double) modelTransform.Rotation.Z * (double) modelTransform.Rotation.Z))));
      transform.Rotation = fromAxisAngle * modelTransform.Rotation;
    }
    if (flipRotationCorrect)
    {
      Quaternion fromAxisAngle = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 3.14159274f);
      transform.Rotation = fromAxisAngle * transform.Rotation;
    }
    HavokPosing.SetModelTransform(havokPose, mainBone.BoneIndex, transform);
    HavokPosing.Propagate(modelSkeleton, mainBone.PartialIndex, mainBone.BoneIndex, transform, modelTransform);
  }

  public unsafe void ApplyFlipToBones(Skeleton* modelSkeleton)
  {
    if ((IntPtr) modelSkeleton == IntPtr.Zero)
      return;
    for (int index = 0; index < (int) modelSkeleton->PartialSkeletonCount; ++index)
    {
      PartialSkeleton partialSkeleton = modelSkeleton->PartialSkeletons[index];
      hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
      if ((IntPtr) havokPose != IntPtr.Zero && (IntPtr) havokPose->Skeleton != IntPtr.Zero)
      {
        hkaSkeleton* skeleton = havokPose->Skeleton;
        IEnumerable<int> second = Enumerable.Range(1, havokPose->Skeleton->Bones.Length - 1);
        Quaternion quaternion1 = Quaternion.Identity;
        if (index > 0)
        {
          Quaternion quaternion2 = HavokPosing.ParentSkeleton(modelSkeleton, index);
          short connectedBoneIndex = partialSkeleton.ConnectedBoneIndex;
          Quaternion quaternion3 = havokPose->ModelPose[(int) connectedBoneIndex].Rotation.ToQuaternion();
          string key = ((hkStringPtr) ref skeleton->Bones[(int) connectedBoneIndex].Name).String;
          Transform transform;
          if (!StringExtensions.IsNullOrEmpty(key) && this.TryGetValue(key, out transform))
            quaternion1 = quaternion3 / transform.Rotation / quaternion2;
        }
        this.PopulateFlippedPoseContainer(modelSkeleton);
        foreach (int boneIx in Enumerable.Range(1, skeleton->Bones.Length - 1).Intersect<int>(second))
        {
          string key = ((hkStringPtr) ref havokPose->Skeleton->Bones[boneIx].Name).String;
          Transform transform1;
          if (!StringExtensions.IsNullOrEmpty(key) && !key.StartsWith("j_f_") && !key.StartsWith("n_f_") && this.TryGetValue(key, out transform1))
          {
            Transform modelTransform = HavokPosing.GetModelTransform(havokPose, boneIx);
            Transform transform2 = new Transform(modelTransform.Position, modelTransform.Rotation, modelTransform.Scale);
            transform2.Rotation = quaternion1 * transform1.Rotation;
            HavokPosing.SetModelTransform(havokPose, boneIx, transform2);
            HavokPosing.Propagate(modelSkeleton, index, boneIx, transform2, modelTransform);
          }
        }
      }
    }
  }

  public unsafe void PopulateFlippedPoseContainer(Skeleton* modelSkeleton)
  {
    if ((IntPtr) modelSkeleton == IntPtr.Zero)
      return;
    for (int index1 = 0; index1 < (int) modelSkeleton->PartialSkeletonCount; ++index1)
    {
      PartialSkeleton partialSkeleton = modelSkeleton->PartialSkeletons[index1];
      hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
      if ((IntPtr) havokPose != IntPtr.Zero && (IntPtr) havokPose->Skeleton != IntPtr.Zero)
      {
        hkaSkeleton* skeleton = havokPose->Skeleton;
        IEnumerable<int> second = Enumerable.Range(1, havokPose->Skeleton->Bones.Length - 1);
        foreach (int boneIx1 in Enumerable.Range(1, skeleton->Bones.Length - 1).Intersect<int>(second))
        {
          hkaBone bone = havokPose->Skeleton->Bones[boneIx1];
          string key = ((hkStringPtr) ref bone.Name).String;
          if (!StringExtensions.IsNullOrEmpty(key) && !key.StartsWith("j_f_") && !key.StartsWith("n_f_") && this.TryGetValue(key, out Transform _))
          {
            Transform modelTransform1 = HavokPosing.GetModelTransform(havokPose, boneIx1);
            this[key] = new Transform(modelTransform1.Position, modelTransform1.Rotation, modelTransform1.Scale);
            if (key.EndsWith("_l"))
            {
              string str = key.Substring(0, key.Length - 1) + "r";
              int boneIx2 = -1;
              for (int index2 = 0; index2 < havokPose->Skeleton->Bones.Length; ++index2)
              {
                bone = havokPose->Skeleton->Bones[index2];
                if (((hkStringPtr) ref bone.Name).String == str)
                {
                  boneIx2 = index2;
                  break;
                }
              }
              if (boneIx2 != -1)
              {
                Transform modelTransform2 = HavokPosing.GetModelTransform(havokPose, boneIx2);
                this[key].Rotation = new Quaternion(-modelTransform2.Rotation.X, -modelTransform2.Rotation.Y, modelTransform2.Rotation.Z, modelTransform2.Rotation.W);
              }
            }
            else if (key.EndsWith("_r"))
            {
              string str = key.Substring(0, key.Length - 1) + "l";
              int boneIx3 = -1;
              for (int index3 = 0; index3 < havokPose->Skeleton->Bones.Length; ++index3)
              {
                bone = havokPose->Skeleton->Bones[index3];
                if (((hkStringPtr) ref bone.Name).String == str)
                {
                  boneIx3 = index3;
                  break;
                }
              }
              if (boneIx3 != -1)
              {
                Transform modelTransform3 = HavokPosing.GetModelTransform(havokPose, boneIx3);
                this[key].Rotation = new Quaternion(-modelTransform3.Rotation.X, -modelTransform3.Rotation.Y, modelTransform3.Rotation.Z, modelTransform3.Rotation.W);
              }
            }
            else
              this[key].Rotation = new Quaternion(-modelTransform1.Rotation.X, -modelTransform1.Rotation.Y, modelTransform1.Rotation.Z, modelTransform1.Rotation.W);
          }
        }
      }
    }
  }
}
