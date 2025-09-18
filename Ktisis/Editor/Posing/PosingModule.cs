// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.PosingModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using Ktisis.Interop.Hooking;
using Ktisis.Services.Game;
using System;

#nullable enable
namespace Ktisis.Editor.Posing;

public sealed class PosingModule : HookModule
{
  private readonly PosingManager Manager;
  private readonly ActorService _actors;
  [Signature("48 8B C4 48 89 58 18 55 56 57 41 54 41 55 41 56 41 57 48 81 EC ?? ?? ?? ?? 0F 29 70 B8 0F 29 78 A8 44 0F 29 40 ?? 44 0F 29 48 ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 8B B1", DetourName = "SetBoneModelSpace")]
  private Hook<PosingModule.SetBoneModelSpaceDelegate> _setBoneModelSpaceHook;
  [Signature("48 83 EC 18 80 79 38 00", DetourName = "SyncModelSpace")]
  private Hook<PosingModule.SyncModelSpaceDelegate> _syncModelSpaceHook;
  [Signature("40 53 48 83 EC 10 4C 8B 49 28", DetourName = "CalcBoneModelSpace")]
  private Hook<PosingModule.CalcBoneModelSpaceDelegate> _calcBoneModelSpaceHook;
  [Signature("48 8B C4 48 89 58 08 48 89 70 10 F3 0F 11 58", DetourName = "LookAtIK")]
  private Hook<PosingModule.LookAtIKDelegate> _lookAtIKHook;
  [Signature("48 8B C4 55 57 48 83 EC 58", DetourName = "KineDriverDetour")]
  private Hook<PosingModule.KineDriverDelegate> _kineDriverHook;
  [Signature("E8 ?? ?? ?? ?? 0F B6 F8 84 C0 74 12", DetourName = "AnimFrozen")]
  private Hook<PosingModule.AnimFrozenDelegate> _animFrozenHook;
  [Signature("E8 ?? ?? ?? ?? 84 DB 74 3A", DetourName = "UpdatePosDetour")]
  private Hook<PosingModule.UpdatePosDelegate> _updatePosHook;
  [Signature("E8 ?? ?? ?? ?? 48 C1 E5 08", DetourName = "SetSkeletonDetour")]
  private Hook<PosingModule.SetSkeletonDelegate> _setSkeletonHook;
  [Signature("E8 ?? ?? ?? ?? 84 C0 0F 44 FE", DetourName = "DisconnectDetour")]
  private Hook<PosingModule.DisconnectDelegate> _disconnectHook;

  public event SkeletonInitHandler? OnSkeletonInit;

  public event Action? OnDisconnect;

  public PosingModule(IHookMediator hook, PosingManager manager, ActorService actors)
    : base(hook)
  {
    this.Manager = manager;
    this._actors = actors;
  }

  public bool IsEnabled { get; private set; }

  public override void EnableAll()
  {
    base.EnableAll();
    this.IsEnabled = true;
  }

  public override void DisableAll()
  {
    base.DisableAll();
    this.IsEnabled = false;
  }

  private ulong SetBoneModelSpace(
    IntPtr partial,
    ushort boneId,
    IntPtr transform,
    bool enableSecondary,
    bool enablePropagate)
  {
    return (ulong) boneId;
  }

  private unsafe void SyncModelSpace(hkaPose* pose)
  {
    if (!this.Manager.IsSolvingIk)
      return;
    this._syncModelSpaceHook.Original(pose);
  }

  private unsafe IntPtr CalcBoneModelSpace(ref hkaPose pose, int boneIdx)
  {
    return this.Manager.IsSolvingIk ? this._calcBoneModelSpaceHook.Original(ref pose, boneIdx) : (IntPtr) (pose.ModelPose.Data + boneIdx);
  }

  private IntPtr LookAtIK(IntPtr a1, IntPtr a2, IntPtr a3, float a4, IntPtr a5, IntPtr a6)
  {
    return IntPtr.Zero;
  }

  private IntPtr KineDriverDetour(IntPtr a1, IntPtr a2) => IntPtr.Zero;

  private byte AnimFrozen(IntPtr a1, int a2) => 1;

  private void UpdatePosDetour(IntPtr gameObject)
  {
  }

  private unsafe byte SetSkeletonDetour(Skeleton* skeleton, ushort partialId, IntPtr a3)
  {
    byte num = this._setSkeletonHook.Original(skeleton, partialId, a3);
    try
    {
      this.HandleRestoreState(skeleton, partialId);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle SetSkeleton:\n{ex}", Array.Empty<object>());
    }
    return num;
  }

  private unsafe void HandleRestoreState(Skeleton* skeleton, ushort partialId)
  {
    if (!this.Manager.IsValid || !this.IsEnabled || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return;
    PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[partialId];
    hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
    if ((IntPtr) havokPose == IntPtr.Zero)
      return;
    this._syncModelSpaceHook.Original(havokPose);
    IGameObject skeletonOwner = this._actors.GetSkeletonOwner(skeleton);
    if (skeletonOwner == null)
      return;
    Ktisis.Ktisis.Log.Verbose($"Restoring partial {partialId} for {skeletonOwner.Name} ({skeletonOwner.ObjectIndex})", Array.Empty<object>());
    if (partialId == (ushort) 0)
      this._updatePosHook.Original(skeletonOwner.Address);
    SkeletonInitHandler onSkeletonInit = this.OnSkeletonInit;
    if (onSkeletonInit == null)
      return;
    onSkeletonInit(skeletonOwner, skeleton, partialId);
  }

  private IntPtr DisconnectDetour(IntPtr a1)
  {
    try
    {
      Action onDisconnect = this.OnDisconnect;
      if (onDisconnect != null)
        onDisconnect();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error(ex.ToString(), Array.Empty<object>());
    }
    return this._disconnectHook.Original(a1);
  }

  private delegate ulong SetBoneModelSpaceDelegate(
    IntPtr partial,
    ushort boneId,
    IntPtr transform,
    bool enableSecondary,
    bool enablePropagate);

  private unsafe delegate void SyncModelSpaceDelegate(hkaPose* pose);

  private delegate IntPtr CalcBoneModelSpaceDelegate(ref hkaPose pose, int boneIdx);

  private delegate IntPtr LookAtIKDelegate(
    IntPtr a1,
    IntPtr a2,
    IntPtr a3,
    float a4,
    IntPtr a5,
    IntPtr a6);

  private delegate IntPtr KineDriverDelegate(IntPtr a1, IntPtr a2);

  private delegate byte AnimFrozenDelegate(IntPtr a1, int a2);

  private delegate void UpdatePosDelegate(IntPtr gameObject);

  private unsafe delegate byte SetSkeletonDelegate(Skeleton* skeleton, ushort partialId, IntPtr a3);

  private delegate IntPtr DisconnectDelegate(IntPtr a1);
}
