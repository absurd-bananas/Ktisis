// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.TwoJoints.TwoJointsSolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using Ktisis.Common.Utility;
using Ktisis.Interop;
using Ktisis.Structs.Havok;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Posing.Ik.TwoJoints;

public class TwoJointsSolver(IkModule module) : IDisposable
{
  private readonly Alloc<TwoJointsIkSetup> AllocIkSetup = new Alloc<TwoJointsIkSetup>(16UL /*0x10*/);

  public unsafe TwoJointsIkSetup* IkSetup => this.AllocIkSetup.Data;

  public unsafe void Setup()
  {
    if (this.AllocIkSetup.Address == IntPtr.Zero)
      throw new Exception("Allocation for IkSetup failed.");
    *this.IkSetup = new TwoJointsIkSetup()
    {
      m_firstJointIdx = (short) -1,
      m_secondJointIdx = (short) -1,
      m_endBoneIdx = (short) -1,
      m_firstJointTwistIdx = (short) -1,
      m_secondJointTwistIdx = (short) -1,
      m_hingeAxisLS = new Vector4(0.0f, 0.0f, 1f, 1f),
      m_cosineMaxHingeAngle = -1f,
      m_cosineMinHingeAngle = 1f,
      m_firstJointIkGain = 1f,
      m_secondJointIkGain = 1f,
      m_endJointIkGain = 1f,
      m_endTargetMS = Vector4.Zero,
      m_endTargetRotationMS = Quaternion.Identity,
      m_endBoneOffsetLS = Vector4.Zero,
      m_endBoneRotationOffsetLS = Quaternion.Identity,
      m_enforceEndPosition = true,
      m_enforceEndRotation = false
    };
  }

  public unsafe bool Solve(hkaPose* poseIn, hkaPose* poseOut, bool frozen = false)
  {
    if ((IntPtr) poseOut == IntPtr.Zero || (IntPtr) poseOut->Skeleton == IntPtr.Zero)
      return false;
    if (frozen)
    {
      ((hkaPose) (IntPtr) poseIn).SetToReferencePose();
      ((hkaPose) (IntPtr) poseIn).SyncModelSpace();
      this.UpdateModelPose(poseIn, poseOut);
    }
    byte num1 = 0;
    IntPtr num2 = module.SolveTwoJoints(&num1, this.IkSetup, poseIn);
    if (num1 == (byte) 0)
      return false;
    ((hkaPose) (IntPtr) poseIn).SyncModelSpace();
    if (frozen)
      this.ApplyModelPoseStatic(poseIn, poseOut);
    else
      this.ApplyModelPoseDynamic(poseIn, poseOut);
    return true;
  }

  public unsafe bool SolveGroup(
    hkaPose* poseIn,
    hkaPose* poseOut,
    TwoJointsGroup group,
    bool frozen = false)
  {
    if (!group.IsEnabled)
      return false;
    TwoJointsIkSetup* ikSetup = this.IkSetup;
    ikSetup->m_firstJointIdx = group.FirstBoneIndex;
    ikSetup->m_firstJointTwistIdx = group.FirstTwistIndex;
    ikSetup->m_secondJointIdx = group.SecondBoneIndex;
    ikSetup->m_secondJointTwistIdx = group.SecondTwistIndex;
    ikSetup->m_endBoneIdx = group.EndBoneIndex;
    ikSetup->m_firstJointIkGain = group.FirstBoneGain;
    ikSetup->m_secondJointIkGain = group.SecondBoneGain;
    ikSetup->m_endJointIkGain = group.EndBoneGain;
    ikSetup->m_enforceEndPosition = group.EnforcePosition;
    ikSetup->m_enforceEndRotation = group.EnforceRotation;
    ikSetup->m_hingeAxisLS = new Vector4(group.HingeAxis, 1f);
    ikSetup->m_cosineMinHingeAngle = group.MinHingeAngle;
    ikSetup->m_cosineMaxHingeAngle = group.MaxHingeAngle;
    Transform modelTransform = HavokPosing.GetModelTransform(poseOut, (int) group.EndBoneIndex);
    if (modelTransform == null)
      return false;
    int num = group.Mode == TwoJointsMode.Relative ? 1 : 0;
    if (num != 0 || !group.EnforcePosition)
      group.TargetPosition = modelTransform.Position;
    if (num != 0 || !group.EnforceRotation)
      group.TargetRotation = modelTransform.Rotation;
    ikSetup->m_endTargetMS = new Vector4(group.TargetPosition, 0.0f);
    ikSetup->m_endTargetRotationMS = group.TargetRotation;
    return this.Solve(poseIn, poseOut, frozen);
  }

  private unsafe void UpdateModelPose(hkaPose* poseIn, hkaPose* poseOut)
  {
    short firstJointIdx = this.IkSetup->m_firstJointIdx;
    for (int parent = 1; parent < poseIn->Skeleton->Bones.Length; ++parent)
    {
      if (parent == (int) firstJointIdx || HavokPosing.IsBoneDescendantOf(poseOut->Skeleton->ParentIndices, (int) firstJointIdx, parent))
        *((hkaPose) (IntPtr) poseIn).AccessBoneModelSpace(parent, (hkaPose.PropagateOrNot) 1) = poseOut->ModelPose[parent];
    }
  }

  private unsafe void ApplyModelPoseStatic(hkaPose* poseIn, hkaPose* poseOut)
  {
    hkArray<short> parentIndices = poseOut->Skeleton->ParentIndices;
    hkaSkeletonUtils.transformModelPoseToLocalPose(poseOut->Skeleton->Bones.Length, parentIndices.Data, poseOut->ModelPose.Data, poseIn->LocalPose.Data);
    short firstJointIdx = this.IkSetup->m_firstJointIdx;
    short endBoneIdx = this.IkSetup->m_endBoneIdx;
    for (int index = 1; index < poseOut->Skeleton->Bones.Length; ++index)
    {
      if ((index == (int) firstJointIdx ? 1 : (HavokPosing.IsBoneDescendantOf(parentIndices, index, (int) firstJointIdx) ? 1 : 0)) != 0)
      {
        if (!HavokPosing.IsBoneDescendantOf(parentIndices, index, (int) endBoneIdx))
        {
          hkQsTransformf* hkQsTransformfPtr = poseOut->ModelPose.Data + index;
          hkQsTransformf hkQsTransformf = poseIn->ModelPose[index];
          hkQsTransformfPtr->Translation = hkQsTransformf.Translation;
          hkQsTransformfPtr->Rotation = hkQsTransformf.Rotation;
        }
        else
        {
          short boneIx = parentIndices[index];
          Transform localTransform = HavokPosing.GetLocalTransform(poseIn, index);
          Transform modelTransform = HavokPosing.GetModelTransform(poseOut, (int) boneIx);
          modelTransform.Position += Vector3.Transform(localTransform.Position, modelTransform.Rotation);
          modelTransform.Rotation *= localTransform.Rotation;
          modelTransform.Scale *= localTransform.Scale;
          HavokPosing.SetModelTransform(poseOut, index, modelTransform);
        }
      }
    }
  }

  private unsafe void ApplyModelPoseDynamic(hkaPose* poseIn, hkaPose* poseOut)
  {
    hkArray<short> parentIndices = poseOut->Skeleton->ParentIndices;
    short firstJointIdx = this.IkSetup->m_firstJointIdx;
    for (int bone = 1; bone < poseOut->Skeleton->Bones.Length; ++bone)
    {
      if (bone == (int) firstJointIdx || HavokPosing.IsBoneDescendantOf(parentIndices, bone, (int) firstJointIdx))
        *((hkaPose) (IntPtr) poseOut).AccessBoneModelSpace(bone, (hkaPose.PropagateOrNot) 1) = poseIn->ModelPose[bone];
    }
  }

  public bool IsDisposed { get; private set; }

  public void Dispose()
  {
    this.IsDisposed = true;
    this.AllocIkSetup.Dispose();
    GC.SuppressFinalize((object) this);
  }
}
