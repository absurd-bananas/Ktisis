// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.Ccd.CcdSolver
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Interop;
using Ktisis.Structs.Havok;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Posing.Ik.Ccd;

public class CcdSolver : IDisposable
{
  private readonly IkModule _module;
  private readonly Alloc<CcdIkSolver> AllocSolver;
  private readonly Alloc<CcdIkConstraint> AllocIkConstraint = new Alloc<CcdIkConstraint>(16UL /*0x10*/);
  private readonly Alloc<hkArray<CcdIkConstraint>> AllocHkArray = new Alloc<hkArray<CcdIkConstraint>>(16UL /*0x10*/);

  private unsafe CcdIkSolver* IkSolver => this.AllocSolver.Data;

  public unsafe CcdIkConstraint* IkConstraint => this.AllocIkConstraint.Data;

  public CcdSolver(IkModule module, Alloc<CcdIkSolver> solver)
  {
    this._module = module;
    this.AllocSolver = solver;
  }

  public unsafe void Setup()
  {
    if (this.AllocIkConstraint.Address == IntPtr.Zero)
      throw new Exception("Allocation for IkConstraint failed.");
    this.IkConstraint->m_startBone = (short) -1;
    this.IkConstraint->m_endBone = (short) -1;
    this.IkConstraint->m_targetMS = Vector4.Zero;
    HavokEx.Initialize<CcdIkConstraint>(this.AllocHkArray.Data, this.IkConstraint, 1);
  }

  public unsafe void Solve(hkaPose* poseIn, hkaPose* poseOut, bool frozen = false)
  {
    if ((IntPtr) poseOut == IntPtr.Zero || (IntPtr) poseOut->Skeleton == IntPtr.Zero)
      return;
    if (frozen)
    {
      ((hkaPose) (IntPtr) poseIn).SetToReferencePose();
      ((hkaPose) (IntPtr) poseIn).SyncModelSpace();
      this.UpdateModelPose(poseIn, poseOut);
    }
    IntPtr num = this._module.SolveCcd(this.IkSolver, &(byte) 0, this.AllocHkArray.Data, poseIn);
    ((hkaPose) (IntPtr) poseIn).SyncModelSpace();
    if (frozen)
      this.ApplyModelPoseStatic(poseIn, poseOut);
    else
      this.ApplyModelPoseDynamic(poseIn, poseOut);
  }

  public unsafe void SolveGroup(hkaPose* poseIn, hkaPose* poseOut, CcdGroup group, bool frozen = false)
  {
    if (!group.IsEnabled)
      return;
    CcdIkSolver* ikSolver = this.IkSolver;
    CcdIkConstraint* ikConstraint = this.IkConstraint;
    int startBoneIndex = (int) group.StartBoneIndex;
    ikConstraint->m_startBone = (short) startBoneIndex;
    int endBoneIndex = (int) group.EndBoneIndex;
    ikConstraint->m_endBone = (short) endBoneIndex;
    Vector4 vector4 = new Vector4(group.TargetPosition, 0.0f);
    ikConstraint->m_targetMS = vector4;
    int iterations = group.Iterations;
    ikSolver->m_iterations = iterations;
    double gain = (double) group.Gain;
    ikSolver->m_gain = (float) gain;
    this.Solve(poseIn, poseOut, frozen);
  }

  private unsafe void UpdateModelPose(hkaPose* poseIn, hkaPose* poseOut)
  {
    short startBone = this.IkConstraint->m_startBone;
    for (int parent = 1; parent < poseIn->Skeleton->Bones.Length; ++parent)
    {
      if (HavokPosing.IsBoneDescendantOf(poseOut->Skeleton->ParentIndices, (int) startBone, parent))
        *((hkaPose) (IntPtr) poseIn).AccessBoneModelSpace(parent, (hkaPose.PropagateOrNot) 1) = poseOut->ModelPose[parent];
    }
  }

  private unsafe void ApplyModelPoseStatic(hkaPose* poseIn, hkaPose* poseOut)
  {
    hkArray<short> parentIndices = poseOut->Skeleton->ParentIndices;
    short startBone = this.IkConstraint->m_startBone;
    for (int index = 1; index < poseOut->Skeleton->Bones.Length; ++index)
    {
      if ((index == (int) startBone ? 1 : (HavokPosing.IsBoneDescendantOf(parentIndices, index, (int) startBone) ? 1 : 0)) != 0)
      {
        Transform modelTransform = HavokPosing.GetModelTransform(poseIn, index);
        HavokPosing.SetModelTransform(poseOut, index, modelTransform);
      }
    }
  }

  private unsafe void ApplyModelPoseDynamic(hkaPose* poseIn, hkaPose* poseOut)
  {
    hkArray<short> parentIndices = poseOut->Skeleton->ParentIndices;
    short startBone = this.IkConstraint->m_startBone;
    for (int bone = 1; bone < poseOut->Skeleton->Bones.Length; ++bone)
    {
      if (bone == (int) startBone || HavokPosing.IsBoneDescendantOf(parentIndices, bone, (int) startBone))
        *((hkaPose) (IntPtr) poseOut).AccessBoneModelSpace(bone, (hkaPose.PropagateOrNot) 1) = poseIn->ModelPose[bone];
    }
  }

  public void Dispose()
  {
    this.AllocSolver.Dispose();
    this.AllocIkConstraint.Dispose();
    this.AllocHkArray.Dispose();
    GC.SuppressFinalize((object) this);
  }
}
