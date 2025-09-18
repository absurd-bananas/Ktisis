// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Skeleton.Constraints.CcdEndNode
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Common.Utility;
using Ktisis.Editor.Posing.Ik.Ccd;
using Ktisis.Editor.Posing.Types;
using Ktisis.Scene.Decor.Ik;
using Ktisis.Scene.Types;

namespace Ktisis.Scene.Entities.Skeleton.Constraints;

public class CcdEndNode : IkEndNode, ICcdNode, IIkNode {

	public CcdEndNode(
		ISceneManager scene,
		EntityPose pose,
		PartialBoneInfo bone,
		uint partialId,
		CcdGroup group
	)
		: base(scene, pose, bone, partialId) {
		this.Group = group;
	}

	protected override bool IsOverride => this.IsEnabled;
	public CcdGroup Group { get; }

	public override Transform GetTransformTarget(Transform offset, Transform world) {
		offset.Position += this.Group.TargetPosition.ModelToWorldPos(offset);
		offset.Rotation = world.Rotation;
		offset.Scale = world.Scale;
		return offset;
	}

	public unsafe override void SetTransformTarget(
		Transform target,
		Transform offset,
		Transform world
	) {
		if ((IntPtr)this.Pose.GetSkeleton() == IntPtr.Zero)
			return;
		this.Group.TargetPosition = target.Position.WorldToModelPos(offset);
		world.Rotation = target.Rotation;
		world.Scale = target.Scale;
		this.SetTransformWorld(world);
	}
}
