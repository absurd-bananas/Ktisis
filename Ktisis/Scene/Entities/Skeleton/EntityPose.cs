// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.EntityPose
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using FFXIVClientStructs.Havok.Animation.Rig;
using Ktisis.Editor.Posing.Ik;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities.Character;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton;

public class EntityPose : SkeletonGroup, ISkeleton, IConfigurable
{
  private readonly IPoseBuilder _builder;
  public readonly IIkController IkController;
  public bool OverlayVisible;
  private readonly Dictionary<int, PartialSkeletonInfo> Partials = new Dictionary<int, PartialSkeletonInfo>();
  private readonly Dictionary<(int p, int i), BoneNode> BoneMap = new Dictionary<(int, int), BoneNode>();

  public EntityPose(ISceneManager scene, IPoseBuilder builder, IIkController ik)
    : base(scene)
  {
    this._builder = builder;
    this.IkController = ik;
    this.Type = EntityType.Armature;
    this.Name = "Pose";
    this.Pose = this;
  }

  public override void Update()
  {
    if (!this.IsValid)
      return;
    this.UpdatePose();
  }

  public unsafe void Refresh()
  {
    this.Partials.Clear();
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    for (int pIndex = 0; pIndex < (int) skeleton->PartialSkeletonCount; ++pIndex)
    {
      uint partialId = EntityPose.GetPartialId(skeleton->PartialSkeletons[pIndex]);
      this.Clean(pIndex, partialId);
    }
  }

  private unsafe void UpdatePose()
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    for (int index = 0; index < (int) skeleton->PartialSkeletonCount; ++index)
      this.UpdatePartial(skeleton, index);
  }

  private unsafe void UpdatePartial(FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton, int index)
  {
    PartialSkeleton partial = skeleton->PartialSkeletons[index];
    uint partialId = EntityPose.GetPartialId(partial);
    uint num = 0;
    PartialSkeletonInfo partialSkeletonInfo;
    if (this.Partials.TryGetValue(index, out partialSkeletonInfo))
    {
      num = partialSkeletonInfo.Id;
    }
    else
    {
      partialSkeletonInfo = new PartialSkeletonInfo(partialId);
      this.Partials.Add(index, partialSkeletonInfo);
    }
    if ((int) partialId == (int) num)
      return;
    Ktisis.Ktisis.Log.Verbose($"Skeleton of '{this.Parent?.Name ?? "UNKNOWN"}' detected a change in partial #{index} (was {num:X}, now {partialId:X}), rebuilding.", Array.Empty<object>());
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    IBoneTreeBuilder boneTreeBuilder = this._builder.BuildBoneTree(index, partialId, partial);
    if (((CharacterBase) (IntPtr) skeleton->Owner).GetModelType() != 4)
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

  private void BuildBoneMap(int index, uint id)
  {
    foreach ((int, int) key in this.BoneMap.Keys.Where<(int, int)>((Func<(int, int), bool>) (key => key.p == index)))
      this.BoneMap.Remove(key);
    if (id == 0U)
      return;
    foreach (SceneEntity sceneEntity in this.Recurse())
    {
      if (sceneEntity is BoneNode boneNode && boneNode.Info.PartialIndex == index)
        this.BoneMap[(index, boneNode.Info.BoneIndex)] = boneNode;
    }
  }

  private static unsafe uint GetPartialId(PartialSkeleton partial)
  {
    SkeletonResourceHandle* skeletonResourceHandle = partial.SkeletonResourceHandle;
    return (IntPtr) skeletonResourceHandle == IntPtr.Zero ? 0U : skeletonResourceHandle->Id;
  }

  private void FilterTree()
  {
    List<BoneNode> list = this.Recurse().Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is BoneNode)).Cast<BoneNode>().ToList<BoneNode>();
    IEnumerable<BoneNode> first = Enumerable.Empty<BoneNode>();
    if (list.Any<BoneNode>((Func<BoneNode, bool>) (bone => bone.Info.Name == "j_f_ago")))
    {
      IEnumerable<BoneNode> second = list.Where<BoneNode>((Func<BoneNode, bool>) (bone => bone.Info.Name == "j_ago"));
      first = first.Concat<BoneNode>(second);
    }
    char earId;
    if (!this.Scene.Context.Config.Categories.ShowAllVieraEars && this.Parent is ActorEntity parent && parent.TryGetEarIdAsChar(out earId))
    {
      IEnumerable<BoneNode> second = list.Where<BoneNode>((Func<BoneNode, bool>) (bone => bone.IsVieraEarBone() && (int) bone.Info.Name[5] != (int) earId));
      first = first.Concat<BoneNode>(second);
    }
    foreach (SceneEntity sceneEntity in first)
      sceneEntity.Remove();
  }

  private bool HasTail() => this.FindBoneByName("n_sippo_a") != null;

  private bool HasEars()
  {
    return this.FindBoneByName("j_zera_a_l") != null || this.FindBoneByName("j_zerb_a_l") != null || this.FindBoneByName("j_zerc_a_l") != null || this.FindBoneByName("j_zerd_a_l") != null;
  }

  public void CheckFeatures(out bool hasTail, out bool isBunny)
  {
    hasTail = this.HasTail();
    isBunny = this.HasEars();
  }

  public unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* GetSkeleton()
  {
    if (!(this.Parent is CharaEntity parent) || !parent.IsDrawing())
      return (FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton*) null;
    CharacterBase* character = parent.GetCharacter();
    return (IntPtr) character == IntPtr.Zero ? (FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton*) null : character->Skeleton;
  }

  public unsafe hkaPose* GetPose(int index)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return (hkaPose*) null;
    PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[index];
    return ((PartialSkeleton) ref partialSkeleton).GetHavokPose(0);
  }

  public BoneNode? GetBoneFromMap(int partialIx, int boneIx)
  {
    return CollectionExtensions.GetValueOrDefault<(int, int), BoneNode>((IReadOnlyDictionary<(int, int), BoneNode>) this.BoneMap, (partialIx, boneIx));
  }

  public BoneNode? FindBoneByName(string name)
  {
    return this.BoneMap.Values.FirstOrDefault<BoneNode>((Func<BoneNode, bool>) (bone => bone.Info.Name == name));
  }

  public PartialSkeletonInfo? GetPartialInfo(int index)
  {
    return CollectionExtensions.GetValueOrDefault<int, PartialSkeletonInfo>((IReadOnlyDictionary<int, PartialSkeletonInfo>) this.Partials, index);
  }

  public override void Remove()
  {
    try
    {
      this.IkController.Destroy();
    }
    finally
    {
      base.Remove();
    }
  }
}
