// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Data.SkeletonPoseData
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Scene.Decor;

namespace Ktisis.Editor.Posing.Data;

public class SkeletonPoseData {
	public PartialSkeleton Partial;
	public unsafe hkaPose* Pose;
	public unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* Skeleton;

	public unsafe short TryResolveBone(IEnumerable<string> names) {
		return Enumerable.FirstOrDefault<short>(names.Select(name => HavokPosing.TryGetBoneNameIndex(this.Pose, name)), (Func<short, bool>)(index => index != -1), (short)-1);
	}

	public unsafe static SkeletonPoseData? TryGet(
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton,
		int partialIndex,
		int poseIndex
	) {
		if ((IntPtr)skeleton == IntPtr.Zero || (IntPtr)skeleton->PartialSkeletons == IntPtr.Zero || partialIndex > (int)skeleton->PartialSkeletonCount)
			return null;
		PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[partialIndex];
		if (((PartialSkeleton) ref partialSkeleton ).HavokPoses.IsEmpty || (IntPtr)partialSkeleton.SkeletonResourceHandle == IntPtr.Zero)
		return null;
		hkaPose* havokPose = ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(poseIndex);
		if ((IntPtr)havokPose == IntPtr.Zero || (IntPtr)havokPose->Skeleton == IntPtr.Zero)
			return null;
		return new SkeletonPoseData {
			Skeleton = skeleton,
			Partial = partialSkeleton,
			Pose = havokPose
		};
	}

	public unsafe static SkeletonPoseData? TryGet(
		ISkeleton skeleton,
		int partialIndex,
		int poseIndex
	) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton1 = skeleton.GetSkeleton();
		return (IntPtr)skeleton1 == IntPtr.Zero ? null : TryGet(skeleton1, partialIndex, poseIndex);
	}
}
