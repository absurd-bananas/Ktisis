// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.Constraints.IkEndNode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Common.Utility;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Types;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Scene.Entities.Skeleton.Constraints;

public abstract class IkEndNode(
  ISceneManager scene,
  EntityPose pose,
  PartialBoneInfo bone,
  uint partialId) : BoneNode(scene, pose, bone, partialId), IIkNode
{
  private IkNodeGroupBase? Parent => base.Parent as IkNodeGroupBase;

  public virtual bool IsEnabled
  {
    get
    {
      IkNodeGroupBase parent = this.Parent;
      return parent != null && __nonvirtual (parent.IsEnabled);
    }
  }

  public virtual unsafe void Enable()
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    Transform offset = new Transform(skeleton->Transform);
    Transform transform = this.CalcTransformWorld();
    if (transform != null)
      this.SetTransformTarget(transform, offset, transform);
    this.Parent?.Enable();
  }

  public virtual void Disable() => this.Parent?.Disable();

  protected abstract bool IsOverride { get; }

  public abstract Transform GetTransformTarget(Transform offset, Transform world);

  public abstract void SetTransformTarget(Transform target, Transform offset, Transform world);

  public override unsafe Transform? GetTransform()
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.Pose.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return (Transform) null;
    Transform offset = new Transform(skeleton->Transform);
    Transform world = this.CalcTransformWorld();
    return !this.IsOverride || world == null ? world : this.GetTransformTarget(offset, world);
  }

  public override unsafe void SetTransform(Transform transform)
  {
    FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.Pose.GetSkeleton();
    if ((IntPtr) skeleton == IntPtr.Zero)
      return;
    Transform offset = new Transform(skeleton->Transform);
    Transform world = this.CalcTransformWorld();
    if (this.IsOverride && world != null)
      this.SetTransformTarget(transform, offset, world);
    else
      this.SetTransformWorld(transform);
  }

  public override Matrix4x4? GetMatrix()
  {
    if (!this.IsOverride)
      return this.CalcMatrixWorld();
    return this.GetTransform()?.ComposeMatrix();
  }

  public override void SetMatrix(Matrix4x4 matrix)
  {
    if (this.IsOverride)
      this.SetTransform(new Transform(matrix));
    else
      this.SetMatrixWorld(matrix);
  }
}
