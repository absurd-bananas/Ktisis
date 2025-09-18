// Decompiled with JetBrains decompiler
// Type: Ktisis.Data.Config.Sections.CategoryConfig
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Data.Config.Bones;

namespace Ktisis.Data.Config.Sections;

public class CategoryConfig {
	public readonly List<BoneCategory> CategoryList = new List<BoneCategory>();
	public bool ShowAllVieraEars;
	public bool ShowFriendlyBoneNames = true;
	public bool ShowNsfwBones = true;

	public BoneCategory? Default { get; set; }

	public void AddCategory(BoneCategory category) {
		Ktisis.Ktisis.Log.Debug("Registering category: " + category.Name, Array.Empty<object>());
		if (category.IsDefault)
			this.Default = category;
		var boneCategory = category;
		boneCategory.SortPriority.GetValueOrDefault();
		if (!boneCategory.SortPriority.HasValue) {
			var count = this.CategoryList.Count;
			boneCategory.SortPriority = count;
		}
		this.CategoryList.Add(category);
	}

	public BoneCategory? GetByName(string name) {
		return this.CategoryList.Find(category => category.Name == name);
	}

	public BoneCategory? GetByNameOrDefault(string name) => this.GetByName(name) ?? this.Default;

	public BoneCategory? GetForBoneName(string name) {
		return this.CategoryList.Find(category => category.Bones.Any(bone => bone.Name == name));
	}

	public BoneCategory? GetForBoneNameOrDefault(string name) => this.GetForBoneName(name) ?? this.Default;

	public unsafe BoneCategory? ResolveBestCategory(hkaSkeleton* skeleton, int index) {
		if ((IntPtr)skeleton == IntPtr.Zero)
			return null;
		for (; index > -1; index = (int)skeleton->ParentIndices[index]) {
			string name = ((hkStringPtr) ref skeleton ->Bones[index].Name).string;
			if (name != null) {
				var forBoneName = this.GetForBoneName(name);
				if (forBoneName != null)
					return forBoneName;
				if (name.StartsWith("j_ex_h"))
					return this.GetByNameOrDefault("Hair");
			} else
				break;
		}
		return this.Default;
	}
}
