// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Bones.BoneCategory
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System.Collections.Generic;

namespace Ktisis.Data.Config.Bones;

public class BoneCategory(string name) {
	public readonly List<CategoryBone> Bones = new List<CategoryBone>();
	public readonly string Name = name;
	public uint BoneColor = uint.MaxValue;
	public CcdGroupParams? CcdGroup;
	public uint GroupColor = 4294942568;
	public bool HideOnPoseEntity;
	public bool IsDefault;
	public bool IsNsfw;
	public bool LinkedColors;
	public string? ParentCategory;
	public int? SortPriority;
	public TwoJointsGroupParams? TwoJointsGroup;
}
