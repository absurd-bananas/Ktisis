// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Character.CharaEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.Havok.Animation.Rig;
using FFXIVClientStructs.Havok.Common.Base.Container.String;
using Ktisis.Common.Utility;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities.Skeleton;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Types;
using Ktisis.Structs.Attachment;
using System;

#nullable enable
namespace Ktisis.Scene.Entities.Character;

public class CharaEntity : WorldEntity, IAttachable, ICharacter
{
  private readonly IPoseBuilder _pose;

  public CharaEntity(ISceneManager scene, IPoseBuilder pose)
    : base(scene)
  {
    this._pose = pose;
  }

  public EntityPose? Pose { get; private set; }

  public override void Setup()
  {
    base.Setup();
    this.Pose = this._pose.Add((IComposite) this);
  }

  public override void Update()
  {
    if (!this.IsDrawing())
      return;
    base.Update();
  }

  public unsafe bool IsDrawing()
  {
    CharacterBase* character = this.GetCharacter();
    return (IntPtr) character != IntPtr.Zero && (character->StateFlags & 65280L) > 0L;
  }

  public unsafe Ktisis.Structs.Characters.CharacterBaseEx* CharacterBaseEx
  {
    get => (Ktisis.Structs.Characters.CharacterBaseEx*) this.GetCharacter();
  }

  public virtual unsafe CharacterBase* GetCharacter() => (CharacterBase*) this.GetObject();

  public unsafe Attach* GetAttach()
  {
    if ((IntPtr) this.CharacterBaseEx == IntPtr.Zero)
      return (Attach*) null;
    Attach* attachPtr = &this.CharacterBaseEx->Attach;
    return (IntPtr) attachPtr->Param == IntPtr.Zero ? (Attach*) null : attachPtr;
  }

  public virtual unsafe bool IsAttached()
  {
    Attach* attach = this.GetAttach();
    return (IntPtr) attach != IntPtr.Zero && attach->IsActive();
  }

  public unsafe PartialBoneInfo? GetParentBone()
  {
    Attach* attach = this.GetAttach();
    if ((IntPtr) attach == IntPtr.Zero)
      return (PartialBoneInfo) null;
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* parentSkeleton = attach->GetParentSkeleton();
    if ((IntPtr) parentSkeleton == IntPtr.Zero || (IntPtr) parentSkeleton->PartialSkeletons == IntPtr.Zero || ((PartialSkeleton) (IntPtr) parentSkeleton->PartialSkeletons).HavokPoses.IsEmpty)
      return (PartialBoneInfo) null;
    hkaPose* havokPose = ((PartialSkeleton) (IntPtr) parentSkeleton->PartialSkeletons).GetHavokPose(0);
    if ((IntPtr) havokPose == IntPtr.Zero || (IntPtr) havokPose->Skeleton == IntPtr.Zero)
      return (PartialBoneInfo) null;
    ushort index;
    if (!AttachUtility.TryGetParentBoneIndex(attach, out index))
      return (PartialBoneInfo) null;
    hkaSkeleton* skeleton = havokPose->Skeleton;
    return new PartialBoneInfo()
    {
      Name = ((hkStringPtr) ref skeleton->Bones[(int) index].Name).String ?? string.Empty,
      BoneIndex = (int) index,
      ParentIndex = (int) skeleton->ParentIndices[(int) index],
      PartialIndex = 0
    };
  }

  public virtual unsafe void Detach()
  {
    Attach* attach = this.GetAttach();
    if ((IntPtr) attach == IntPtr.Zero)
      return;
    AttachUtility.Detach(attach);
  }

  public override unsafe void SetTransform(Transform trans)
  {
    Attach* attach = this.GetAttach();
    if ((IntPtr) attach != IntPtr.Zero && attach->IsActive())
    {
      Transform transform = this.GetTransform();
      AttachUtility.SetTransformRelative(attach, trans, transform);
      if (transform.Scale == trans.Scale)
        return;
      transform.Scale = trans.Scale;
      base.SetTransform(transform);
    }
    else
      base.SetTransform(trans);
  }
}
