// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.EntityPose
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Ktisis.Editor.Posing.Ik;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities.Character;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Skeleton;

public class EntityPose : SkeletonGroup, ISkeleton, IConfigurable {
	private readonly IPoseBuilder _builder;
	private readonly Dictionary<(int p, int i), BoneNode> BoneMap = new Dictionary<(int, int), BoneNode>();
	public readonly IIkController IkController;
	private readonly Dictionary<int, PartialSkeletonInfo> Partials = new Dictionary<int, PartialSkeletonInfo>();
	public bool OverlayVisible;

	public EntityPose(ISceneManager scene, IPoseBuilder builder, IIkController ik)
		: base(scene) {
		this._builder = builder;
		this.IkController = ik;
		this.Type = EntityType.Armature;
		this.Name = "Pose";
		this.Pose = this;
	}

	public unsafe void Refresh() {
		this.Partials.Clear();
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		for (var pIndex = 0; pIndex < (int)skeleton->PartialSkeletonCount; ++pIndex) {
			var partialId = GetPartialId(skeleton->PartialSkeletons[pIndex]);
			this.Clean(pIndex, partialId);
		}
	}

	public unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* GetSkeleton() {
		if (!(this.Parent is CharaEntity parent) || !parent.IsDrawing())
			return (FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton*)null;
		CharacterBase* character = parent.GetCharacter();
		return (IntPtr)character == IntPtr.Zero ? (FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton*)null : character->Skeleton;
	}

	public unsafe hkaPose* GetPose(int index) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return (hkaPose*)null;
		PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[index];
		return ((PartialSkeleton) ref partialSkeleton ).GetHavokPose(0);
	}

	public override void Update() {
		if (!this.IsValid)
			return;
		this.UpdatePose();
	}

	private unsafe void UpdatePose() {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		for (var index = 0; index < (int)skeleton->PartialSkeletonCount; ++index)
			this.UpdatePartial(skeleton, index);
	}

	private unsafe void UpdatePartial(FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton, int index) {
		PartialSkeleton partial = skeleton->PartialSkeletons[index];
		var partialId = GetPartialId(partial);
		uint num = 0;
		PartialSkeletonInfo partialSkeletonInfo;
		if (this.Partials.TryGetValue(index, out partialSkeletonInfo)) {
			num = partialSkeletonInfo.Id;
		} else {
			partialSkeletonInfo = new PartialSkeletonInfo(partialId);
			this.Partials.Add(index, partialSkeletonInfo);
		}
		if ((int)partialId == (int)num)
			return;
		Ktisis.Ktisis.Log.Verbose($"Skeleton of '{this.Parent?.Name ?? "UNKNOWN"}' detected a change in partial #{index} (was {num:X}, now {partialId:X}), rebuilding.", Array.Empty<object>());
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		var boneTreeBuilder = this._builder.BuildBoneTree(index, partialId, partial);
		if (((CharacterBase)(IntPtr)skeleton->Owner).GetModelType() != 4)
			boneTreeBuilder.BuildCategoryMap();
		else
			boneTreeBuilder.BuildBoneList();
		if (num != 0U)
			this.Clean(index, partialId);
		partialSkeletonInfo.CopyPartial(partialId, partial);
		if (partialId != 0U)
			boneTreeBuilder.BindTo(this);
		this.FilterTree();
		this.BuildBoneMap(index, partialId);
		stopwatch.Stop();
		Ktisis.Ktisis.Log.Debug($"Rebuild took {stopwatch.Elapsed.TotalMilliseconds:00.00}ms", Array.Empty<object>());
	}

	private void BuildBoneMap(int index, uint id) {
		foreach (var key in this.BoneMap.Keys.Where((Func<(int, int), bool>)(key => key.p == index)))
			this.BoneMap.Remove(key);
		if (id == 0U)
			return;
		foreach (var sceneEntity in this.Recurse()) {
			if (sceneEntity is BoneNode boneNode && boneNode.Info.PartialIndex == index)
				this.BoneMap[(index, boneNode.Info.BoneIndex)] = boneNode;
		}
	}

	private unsafe static uint GetPartialId(PartialSkeleton partial) {
		SkeletonResourceHandle* skeletonResourceHandle = partial.SkeletonResourceHandle;
		return (IntPtr)skeletonResourceHandle == IntPtr.Zero ? 0U : skeletonResourceHandle->Id;
	}

	private void FilterTree() {
		var list = this.Recurse().Where(entity => entity is BoneNode).Cast<BoneNode>().ToList();
		var first = Enumerable.Empty<BoneNode>();
		if (list.Any(bone => bone.Info.Name == "j_f_ago")) {
			var second = list.Where(bone => bone.Info.Name == "j_ago");
			first = first.Concat(second);
		}
		char earId;
		if (!this.Scene.Context.Config.Categories.ShowAllVieraEars && this.Parent is ActorEntity parent && parent.TryGetEarIdAsChar(out earId)) {
			var second = list.Where(bone => bone.IsVieraEarBone() && bone.Info.Name[5] != earId);
			first = first.Concat(second);
		}
		foreach (SceneEntity sceneEntity in first)
			sceneEntity.Remove();
	}

	private bool HasTail() => this.FindBoneByName("n_sippo_a") != null;

	private bool HasEars() => this.FindBoneByName("j_zera_a_l") != null || this.FindBoneByName("j_zerb_a_l") != null || this.FindBoneByName("j_zerc_a_l") != null || this.FindBoneByName("j_zerd_a_l") != null;

	public void CheckFeatures(out bool hasTail, out bool isBunny) {
		hasTail = this.HasTail();
		isBunny = this.HasEars();
	}

	public BoneNode? GetBoneFromMap(int partialIx, int boneIx) => CollectionExtensions.GetValueOrDefault<(int, int), BoneNode>((IReadOnlyDictionary<(int, int), BoneNode>)this.BoneMap, (partialIx, boneIx));

	public BoneNode? FindBoneByName(string name) {
		return this.BoneMap.Values.FirstOrDefault(bone => bone.Info.Name == name);
	}

	public PartialSkeletonInfo? GetPartialInfo(int index) => CollectionExtensions.GetValueOrDefault<int, PartialSkeletonInfo>((IReadOnlyDictionary<int, PartialSkeletonInfo>)this.Partials, index);

	public override void Remove() {
		try {
			this.IkController.Destroy();
		} finally {
			base.Remove();
		}
	}
}
