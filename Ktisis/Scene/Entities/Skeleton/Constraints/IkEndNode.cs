// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.Constraints.IkEndNode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Common.Utility;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Skeleton.Constraints;

public abstract class IkEndNode(
	ISceneManager scene,
	EntityPose pose,
	PartialBoneInfo bone,
	uint partialId
) : BoneNode(scene, pose, bone, partialId), IIkNode {
	private IkNodeGroupBase? Parent => base.Parent as IkNodeGroupBase;

	protected abstract bool IsOverride { get; }

	public virtual bool IsEnabled {
		get {
			var parent = this.Parent;
			return parent != null && __nonvirtual(parent.IsEnabled);
		}
	}

	public unsafe virtual void Enable() {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		var offset = new Transform(skeleton->Transform);
		var transform = this.CalcTransformWorld();
		if (transform != null)
			this.SetTransformTarget(transform, offset, transform);
		this.Parent?.Enable();
	}

	public virtual void Disable() => this.Parent?.Disable();

	public abstract Transform GetTransformTarget(Transform offset, Transform world);

	public abstract void SetTransformTarget(Transform target, Transform offset, Transform world);

	public unsafe override Transform? GetTransform() {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.Pose.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return null;
		var offset = new Transform(skeleton->Transform);
		var world = this.CalcTransformWorld();
		return !this.IsOverride || world == null ? world : this.GetTransformTarget(offset, world);
	}

	public unsafe override void SetTransform(Transform transform) {
		FFXIVClientStructs.FFXIV.Client.Graphics.Render.Skeleton* skeleton = this.Pose.GetSkeleton();
		if ((IntPtr)skeleton == IntPtr.Zero)
			return;
		var offset = new Transform(skeleton->Transform);
		var world = this.CalcTransformWorld();
		if (this.IsOverride && world != null)
			this.SetTransformTarget(transform, offset, world);
		else
			this.SetTransformWorld(transform);
	}

	public override Matrix4x4? GetMatrix() {
		if (!this.IsOverride)
			return this.CalcMatrixWorld();
		return this.GetTransform()?.ComposeMatrix();
	}

	public override void SetMatrix(Matrix4x4 matrix) {
		if (this.IsOverride)
			this.SetTransform(new Transform(matrix));
		else
			this.SetMatrixWorld(matrix);
	}
}
