// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.IkModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Math.QsTransform;
using Ktisis.Editor.Posing.Ik.Ccd;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Interop;
using Ktisis.Interop.Hooking;
using Ktisis.Structs.Havok;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Ktisis.Editor.Posing.Ik;

public sealed class IkModule : HookModule
{
  private readonly PosingManager Manager;
  private readonly List<IIkController> Controllers = new List<IIkController>();
  [Signature("E8 ?? ?? ?? ?? 48 8D 43 20")]
  private unsafe IntPtr** CcdVfTable = (IntPtr**) null;
  [Signature("E8 ?? ?? ?? ?? 0F 28 55 10")]
  public IkModule.SolveTwoJointsDelegate SolveTwoJoints;
  [Signature("E8 ?? ?? ?? ?? 8B 45 EF 48 8B 7D F7")]
  public IkModule.SolveCcdDelegate SolveCcd;
  [Signature("48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 30 48 8B 01 49 8B E9")]
  public IkModule.InitHkaPoseDelegate InitHkaPose;
  [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC 30 F3 0F 10 81 ?? ?? ?? ?? 48 8B FA", DetourName = "UpdateAnimationDetour")]
  private Hook<IkModule.UpdateAnimationDelegate> UpdateAnimationHook;

  public IkModule(IHookMediator hook, PosingManager manager)
    : base(hook)
  {
    this.Manager = manager;
  }

  public override bool Initialize()
  {
    int num = base.Initialize() ? 1 : 0;
    if (num == 0)
      return num != 0;
    this.EnableAll();
    return num != 0;
  }

  public IIkController CreateController()
  {
    IkController controller = new IkController(this, this.CreateCcdSolver(), this.CreateTwoJointsSolver());
    lock (this.Controllers)
      this.Controllers.Add((IIkController) controller);
    return (IIkController) controller;
  }

  public bool RemoveController(IIkController controller)
  {
    lock (this.Controllers)
      return this.Controllers.Remove(controller);
  }

  public unsafe CcdSolver CreateCcdSolver(int iterations = 8, float gain = 0.5f)
  {
    CcdSolver ccdSolver = new CcdSolver(this, new Alloc<CcdIkSolver>()
    {
      Data = {
        _vfTable = this.CcdVfTable,
        hkRefObject = {
          MemSizeAndRefCount = 4294901761U
        },
        m_iterations = iterations,
        m_gain = gain
      }
    });
    ccdSolver.Setup();
    return ccdSolver;
  }

  public TwoJointsSolver CreateTwoJointsSolver()
  {
    TwoJointsSolver twoJointsSolver = new TwoJointsSolver(this);
    twoJointsSolver.Setup();
    return twoJointsSolver;
  }

  private void UpdateAnimationDetour(IntPtr a1)
  {
    this.UpdateAnimationHook.Original(a1);
    try
    {
      this.UpdateIkPoses();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to update IK poses:\n{ex}", System.Array.Empty<object>());
    }
  }

  private void UpdateIkPoses()
  {
    if (!this.Manager.IsValid)
      return;
    IEnumerable<IIkController> list;
    lock (this.Controllers)
      list = (IEnumerable<IIkController>) this.Controllers.ToList<IIkController>();
    try
    {
      this.Manager.IsSolvingIk = true;
      foreach (IIkController ikController in list)
        ikController.Solve(this.Manager.IsEnabled);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to update IK controllers:\n{ex}", System.Array.Empty<object>());
    }
    finally
    {
      this.Manager.IsSolvingIk = false;
    }
  }

  public unsafe delegate IntPtr SolveTwoJointsDelegate(
    byte* result,
    TwoJointsIkSetup* setup,
    hkaPose* pose);

  public unsafe delegate IntPtr SolveCcdDelegate(
    CcdIkSolver* solver,
    byte* result,
    hkArray<CcdIkConstraint>* constraints,
    hkaPose* hkaPose);

  public unsafe delegate IntPtr InitHkaPoseDelegate(
    hkaPose* pose,
    int space,
    IntPtr unk,
    hkArray<hkQsTransformf>* transforms);

  private delegate void UpdateAnimationDelegate(IntPtr a1);
}
