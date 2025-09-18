// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Types.PartialSkeletonInfo
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using Ktisis.Common.Extensions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Editor.Posing.Types;

public class PartialSkeletonInfo
{
  public uint Id;
  public short ConnectedBoneIndex;
  public short ConnectedParentBoneIndex;
  public short[] ParentIds = Array.Empty<short>();

  public PartialSkeletonInfo(uint id) => this.Id = id;

  public unsafe void CopyPartial(uint id, PartialSkeleton partial)
  {
    this.Id = id;
    this.ConnectedBoneIndex = partial.ConnectedBoneIndex;
    this.ConnectedParentBoneIndex = partial.ConnectedParentBoneIndex;
    hkaPose* havokPose = ((PartialSkeleton) ref partial).GetHavokPose(0);
    if ((IntPtr) havokPose != IntPtr.Zero && (IntPtr) havokPose->Skeleton != IntPtr.Zero)
      this.ParentIds = havokPose->Skeleton->ParentIndices.Copy<short>();
    else
      this.ParentIds = Array.Empty<short>();
  }

  public IEnumerable<short> GetParentsOf(int id)
  {
    for (short parent = this.ParentIds[id]; parent != (short) -1; parent = this.ParentIds[(int) parent])
      yield return parent;
  }

  public bool IsBoneDescendantOf(int bone, int descOf)
  {
    for (short parentId = this.ParentIds[bone]; parentId != (short) -1; parentId = this.ParentIds[(int) parentId])
    {
      if ((int) parentId == descOf)
        return true;
    }
    return false;
  }
}
