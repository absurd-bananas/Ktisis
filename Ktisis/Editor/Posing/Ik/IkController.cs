// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Ik.IkController
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Common.Extensions;
using Ktisis.Data.Config.Bones;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Ik.Ccd;
using Ktisis.Editor.Posing.Ik.TwoJoints;
using Ktisis.Editor.Posing.Ik.Types;
using Ktisis.Interop;
using Ktisis.Scene.Decor;

namespace Ktisis.Editor.Posing.Ik;

public class IkController : IIkController {
	private readonly Alloc<hkaPose> _allocPose = new Alloc<hkaPose>(16UL /*0x10*/);
	private readonly CcdSolver _ccd;
	private readonly IkModule _module;
	private readonly TwoJointsSolver _twoJoints;
	private readonly Dictionary<string, IIkGroup> Groups = new Dictionary<string, IIkGroup>();
	private bool _isDestroyed;
	private bool IsInitialized;
	private ISkeleton? Skeleton;

	public IkController(IkModule module, CcdSolver ccd, TwoJointsSolver twoJoints) {
		this._module = module;
		this._ccd = ccd;
		this._twoJoints = twoJoints;
	}

	private unsafe hkaPose* Pose => this._allocPose.Data;

	public void Setup(ISkeleton skeleton) => this.Skeleton = skeleton;

	public unsafe void Solve(bool frozen = false) {
		if (this.Skeleton == null)
			return;
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.Skeleton.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero)
			return;
		PartialSkeleton partialSkeleton = *skeleton->PartialSkeletons;
		if (((PartialSkeleton) ref partialSkeleton ).HavokPoses.IsEmpty || (IntPtr)partialSkeleton.SkeletonResourceHandle == IntPtr.Zero)
		return;
		hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
		if ((IntPtr)havokPose == IntPtr.Zero || (IntPtr)havokPose->Skeleton == IntPtr.Zero)
			return;
		uint id = partialSkeleton.SkeletonResourceHandle->Id;
		var list = this.Groups.Values.Where(group => group.IsEnabled && (int)group.SkeletonId == (int)id).ToList();
		if (list.Count == 0)
			return;
		this.Solve(havokPose, list, frozen);
	}

	public int GroupCount => this.Groups.Count;

	public IEnumerable<(string name, IIkGroup group)> GetGroups() {
		return this.Groups.Select((Func<KeyValuePair<string, IIkGroup>, (string, IIkGroup)>)(pair => (pair.Key, pair.Value)));
	}

	public unsafe bool TrySetupGroup(string name, CcdGroupParams param, out CcdGroup? group) {
		group = null;
		Ktisis.Ktisis.Log.Verbose("Setting up group for CCD IK: " + name, Array.Empty<object>());
		if (this.Skeleton != null) {
			var skeletonPoseData = SkeletonPoseData.TryGet(this.Skeleton, 0, 0);
			if (skeletonPoseData != null) {
				IIkGroup ikGroup;
				if (this.Groups.TryGetValue(name, out ikGroup))
					group = ikGroup as CcdGroup;
				if (group == null)
					group = new CcdGroup();
				var num1 = skeletonPoseData.TryResolveBone(param.StartBone);
				var num2 = skeletonPoseData.TryResolveBone(param.EndBone);
				if (num1 == -1 || num2 == -1) {
					Ktisis.Ktisis.Log.Warning($"Resolve failed: {num1} {num2}", Array.Empty<object>());
					return false;
				}
				group.StartBoneIndex = num1;
				group.EndBoneIndex = num2;
				Ktisis.Ktisis.Log.Verbose($"Resolved bones: {num1} {num2}", Array.Empty<object>());
				group.SkeletonId = skeletonPoseData.Partial.SkeletonResourceHandle->Id;
				this.Groups[name] = group;
				return true;
			}
		}
		return false;
	}

	public unsafe bool TrySetupGroup(
		string name,
		TwoJointsGroupParams param,
		out TwoJointsGroup? group
	) {
		group = (TwoJointsGroup)null;
		Ktisis.Ktisis.Log.Verbose("Setting up group for TwoJoints IK: " + name, Array.Empty<object>());
		if (this.Skeleton != null) {
			var skeletonPoseData = SkeletonPoseData.TryGet(this.Skeleton, 0, 0);
			if (skeletonPoseData != null) {
				IIkGroup ikGroup;
				if (this.Groups.TryGetValue(name, out ikGroup))
					group = ikGroup as TwoJointsGroup;
				if ((object)group == null)
					group = new TwoJointsGroup {
						HingeAxis = param.Type == TwoJointsType.Leg ? -Vector3.UnitZ : Vector3.UnitZ
					};
				var num1 = skeletonPoseData.TryResolveBone(param.FirstBone);
				var num2 = skeletonPoseData.TryResolveBone(param.SecondBone);
				var num3 = skeletonPoseData.TryResolveBone(param.EndBone);
				if (num1 == -1 || num2 == -1 || num3 == -1)
					return false;
				group.FirstBoneIndex = num1;
				group.FirstTwistIndex = skeletonPoseData.TryResolveBone(param.FirstTwist);
				group.SecondBoneIndex = num2;
				group.SecondTwistIndex = skeletonPoseData.TryResolveBone(param.SecondTwist);
				group.EndBoneIndex = num3;
				Ktisis.Ktisis.Log.Verbose($"Resolved bones: {num1} {num2} {num3} ({group.FirstTwistIndex}, {group.SecondTwistIndex})", Array.Empty<object>());
				group.SkeletonId = skeletonPoseData.Partial.SkeletonResourceHandle->Id;
				this.Groups[name] = (IIkGroup)group;
				return true;
			}
		}
		return false;
	}

	public void Destroy() {
		if (this._isDestroyed)
			throw new Exception("IK controller is already disposed.");
		this._ccd.Dispose();
		this._twoJoints.Dispose();
		this._isDestroyed = this._module.RemoveController(this);
	}

	private unsafe void Initialize(hkaPose* pose) {
		if (this._allocPose.Address == IntPtr.Zero)
			throw new Exception("Allocation for hkaPose failed.");
		this.Pose->Skeleton = pose->Skeleton;
		HavokEx.Initialize<hkQsTransformf>(&this.Pose->LocalPose);
		HavokEx.Initialize<hkQsTransformf>(&this.Pose->ModelPose);
		HavokEx.Initialize<uint>(&this.Pose->BoneFlags);
		HavokEx.Initialize<float>(&this.Pose->FloatSlotValues);
		this.Pose->ModelInSync = (byte)0;
		hkArray<hkQsTransformf>* syncedPoseLocalSpace = ((hkaPose)(IntPtr)pose).GetSyncedPoseLocalSpace();
		var num = this._module.InitHkaPose(this.Pose, 1, (IntPtr)syncedPoseLocalSpace, syncedPoseLocalSpace);
		this.IsInitialized = true;
	}

	private unsafe void Solve(hkaPose* pose, IEnumerable<IIkGroup> groups, bool frozen) {
		if (!this.IsInitialized || pose->Skeleton != this.Pose->Skeleton)
			this.Initialize(pose);
		if (!frozen) {
			((hkaPose)(IntPtr)this.Pose).SetPoseLocalSpace(&pose->LocalPose);
			((hkaPose)(IntPtr)this.Pose).SyncModelSpace();
		}
		foreach (var group1 in groups) {
			TwoJointsGroup group2 = group1 as TwoJointsGroup;
			if ((object)group2 == null) {
				if (group1 is CcdGroup group3)
					this._ccd.SolveGroup(this.Pose, pose, group3, frozen);
			} else
				this._twoJoints.SolveGroup(this.Pose, pose, group2, frozen);
		}
	}
}
