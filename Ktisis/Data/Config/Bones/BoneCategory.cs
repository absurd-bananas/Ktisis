// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Bones.BoneCategory
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using System.Collections.Generic;

#nullable enable
namespace Ktisis.Data.Config.Bones;

public class BoneCategory(string name)
{
  public readonly string Name = name;
  public uint GroupColor = 4294942568;
  public uint BoneColor = uint.MaxValue;
  public bool LinkedColors;
  public bool HideOnPoseEntity;
  public bool IsNsfw;
  public bool IsDefault;
  public string? ParentCategory;
  public int? SortPriority;
  public readonly List<CategoryBone> Bones = new List<CategoryBone>();
  public TwoJointsGroupParams? TwoJointsGroup;
  public CcdGroupParams? CcdGroup;
}
