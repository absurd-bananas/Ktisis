// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Types.BoneEnumerator
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.Array;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using System;
using System.Collections.Generic;

#nullable enable
namespace Ktisis.Editor.Posing.Types;

public class BoneEnumerator
{
  protected readonly int Index;
  protected PartialSkeleton Partial;

  public BoneEnumerator(int index, PartialSkeleton partial)
  {
    this.Index = index;
    this.Partial = partial;
  }

  protected unsafe hkaSkeleton* GetSkeleton()
  {
    hkaPose* havokPose = ((PartialSkeleton) ref this.Partial).GetHavokPose(0);
    return (IntPtr) havokPose == IntPtr.Zero ? (hkaSkeleton*) null : havokPose->Skeleton;
  }

  public unsafe IEnumerable<PartialBoneInfo> EnumerateBones()
  {
    hkaSkeleton* skeleton = this.GetSkeleton();
    return this.EnumerateBones(skeleton->Bones, skeleton->ParentIndices);
  }

  private IEnumerable<PartialBoneInfo> EnumerateBones(
    hkArray<hkaBone> bones,
    hkArray<short> parents)
  {
    for (int i = 1; i < bones.Length; ++i)
    {
      string str = ((hkStringPtr) ref bones[i].Name).String;
      if (!StringExtensions.IsNullOrEmpty(str) && (this.Index != 0 || !(str == "j_ago")) && parents[i] != (short) -1)
        yield return new PartialBoneInfo()
        {
          Name = str,
          BoneIndex = i,
          ParentIndex = (int) parents[i],
          PartialIndex = this.Index
        };
    }
  }
}
