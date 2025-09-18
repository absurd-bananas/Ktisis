// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Types.PartialSkeletonInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

namespace Ktisis.Editor.Posing.Types;

public class PartialSkeletonInfo {
	public short ConnectedBoneIndex;
	public short ConnectedParentBoneIndex;
	public uint Id;
	public short[] ParentIds = Array.Empty<short>();

	public PartialSkeletonInfo(uint id) {
		this.Id = id;
	}

	public unsafe void CopyPartial(uint id, PartialSkeleton partial) {
		this.Id = id;
		this.ConnectedBoneIndex = partial.ConnectedBoneIndex;
		this.ConnectedParentBoneIndex = partial.ConnectedParentBoneIndex;
		hkaPose* havokPose = ((PartialSkeleton) ref partial ).GetHavokPose(0);
		if ((IntPtr)havokPose != IntPtr.Zero && (IntPtr)havokPose->Skeleton != IntPtr.Zero)
			this.ParentIds = havokPose->Skeleton->ParentIndices.Copy<short>();
		else
			this.ParentIds = Array.Empty<short>();
	}

	public IEnumerable<short> GetParentsOf(int id) {
		for (var parent = this.ParentIds[id]; parent != -1; parent = this.ParentIds[parent])
			yield return parent;
	}

	public bool IsBoneDescendantOf(int bone, int descOf) {
		for (var parentId = this.ParentIds[bone]; parentId != -1; parentId = this.ParentIds[parentId]) {
			if (parentId == descOf)
				return true;
		}
		return false;
	}
}
