// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.BoneNode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using FFXIVClientStructs.FFXIV.Client.Graphics.Render;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.Havok.Animation.Rig;
using Ktisis.Common.Utility;
using Ktisis.Editor.Posing;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Types;
using Ktisis.Structs.Attachment;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton;

public class BoneNode : SkeletonNode, ITransform, IVisibility, IAttachTarget
{
  public PartialBoneInfo Info;
  public uint PartialId;

  public bool Visible { get; set; }

  public BoneNode(ISceneManager scene, EntityPose pose, PartialBoneInfo bone, uint partialId)
    : base(scene)
  {
    this.Type = EntityType.BoneNode;
    this.Pose = pose;
    this.Info = bone;
    this.PartialId = partialId;
  }

  public unsafe hkaPose* GetPose() => this.Pose.GetPose(this.Info.PartialIndex);

  public unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* GetSkeleton()
  {
    return this.Pose.GetSkeleton();
  }

  public bool MatchesId(int pId, int bId)
  {
    return this.Info.PartialIndex == pId && this.Info.BoneIndex == bId;
  }

  protected unsafe Matrix4x4? GetMatrixModel()
  {
    hkaPose* pose = this.GetPose();
    return (IntPtr) pose == IntPtr.Zero ? new Matrix4x4?() : new Matrix4x4?(HavokPosing.GetMatrix(pose, this.Info.BoneIndex));
  }

  protected unsafe Matrix4x4? CalcMatrixWorld()
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    Matrix4x4? nullable;
    if ((IntPtr) skeleton != IntPtr.Zero)
    {
      nullable = this.GetMatrixModel();
      if (nullable.HasValue)
      {
        Matrix4x4 valueOrDefault = nullable.GetValueOrDefault();
        Transform transform = new Transform(skeleton->Transform);
        valueOrDefault.Translation *= transform.Scale;
        Matrix4x4 matrix4x4 = Matrix4x4.Transform(valueOrDefault, transform.Rotation);
        matrix4x4.Translation += transform.Position;
        return new Matrix4x4?(matrix4x4);
      }
    }
    nullable = new Matrix4x4?();
    return nullable;
  }

  protected unsafe void SetMatrixWorld(Matrix4x4 matrix)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    hkaPose* pose = (IntPtr) skeleton != IntPtr.Zero ? this.GetPose() : (hkaPose*) null;
    if ((IntPtr) pose == IntPtr.Zero)
      return;
    Transform transform = new Transform(skeleton->Transform);
    matrix.Translation -= transform.Position;
    matrix = Matrix4x4.Transform(matrix, Quaternion.Inverse(transform.Rotation));
    matrix.Translation /= transform.Scale;
    HavokPosing.SetMatrix(pose, this.Info.BoneIndex, matrix);
  }

  protected void SetTransformWorld(Transform transform)
  {
    this.SetMatrixWorld(transform.ComposeMatrix());
  }

  public Transform? CalcTransformWorld()
  {
    Matrix4x4? nullable = this.CalcMatrixWorld();
    return !nullable.HasValue ? (Transform) null : new Transform(nullable.Value);
  }

  public Transform? GetTransformModel()
  {
    Matrix4x4? matrixModel = this.GetMatrixModel();
    return !matrixModel.HasValue ? (Transform) null : new Transform(matrixModel.Value);
  }

  public unsafe bool IsBoneChildOf(BoneNode node)
  {
    if (this.Pose != node.Pose)
      return false;
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return false;
    if (this.Info.PartialIndex == node.Info.PartialIndex)
      return this.Info.ParentIndex == node.Info.BoneIndex;
    if (node.Info.PartialIndex != 0)
      return false;
    PartialSkeleton partialSkeleton = skeleton->PartialSkeletons[this.Info.PartialIndex];
    return this.Info.BoneIndex == (int) partialSkeleton.ConnectedBoneIndex && node.Info.BoneIndex == (int) partialSkeleton.ConnectedParentBoneIndex;
  }

  public unsafe bool IsBoneDescendantOf(BoneNode node)
  {
    if (this.Pose != node.Pose)
      return false;
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero || (IntPtr) skeleton->PartialSkeletons == IntPtr.Zero)
      return false;
    PartialSkeleton partialSkeleton1 = skeleton->PartialSkeletons[this.Info.PartialIndex];
    int partialIndex1 = this.Info.PartialIndex;
    int partialIndex2 = node.Info.PartialIndex;
    hkaPose* havokPose;
    int bone;
    int boneIndex;
    if (partialIndex1 != partialIndex2)
    {
      if (partialIndex1 == 0 || partialIndex2 != 0)
        return false;
      PartialSkeleton partialSkeleton2 = *skeleton->PartialSkeletons;
      havokPose = ((PartialSkeleton) ref partialSkeleton2).GetHavokPose(0);
      bone = (int) partialSkeleton1.ConnectedParentBoneIndex;
      boneIndex = node.Info.BoneIndex;
      if (bone == boneIndex)
        return true;
    }
    else
    {
      havokPose = ((PartialSkeleton) ref partialSkeleton1).GetHavokPose(0);
      bone = this.Info.BoneIndex;
      boneIndex = node.Info.BoneIndex;
    }
    return (IntPtr) havokPose != IntPtr.Zero && (IntPtr) havokPose->Skeleton != IntPtr.Zero && HavokPosing.IsBoneDescendantOf(havokPose->Skeleton->ParentIndices, bone, boneIndex);
  }

  public bool IsVieraEarBone()
  {
    return this.Info.Name.Length >= 7 && this.Info.Name.StartsWith("j_zer") && this.Info.Name[6] == '_';
  }

  public virtual Transform? GetTransform() => this.CalcTransformWorld();

  public virtual void SetTransform(Transform transform) => this.SetTransformWorld(transform);

  public virtual Matrix4x4? GetMatrix() => this.CalcMatrixWorld();

  public virtual void SetMatrix(Matrix4x4 matrix) => this.SetMatrixWorld(matrix);

  public unsafe bool TryAcceptAttach(IAttachable child)
  {
    if (this.Info.PartialIndex > 0)
      return false;
    Attach* attach = child.GetAttach();
    CharacterBase* character = child.GetCharacter();
    if ((IntPtr) attach == IntPtr.Zero || (IntPtr) character == IntPtr.Zero)
      return false;
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton1 = this.GetSkeleton();
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton2 = character->Skeleton;
    if ((IntPtr) skeleton1 == IntPtr.Zero || (IntPtr) skeleton2 == IntPtr.Zero)
      return false;
    AttachUtility.SetBoneAttachment(skeleton1, skeleton2, attach, (ushort) this.Info.BoneIndex);
    return true;
  }
}
