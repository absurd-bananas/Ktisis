// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Posing.Types.IPosingManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Threading.Tasks;

using Ktisis.Data.Files;
using Ktisis.Editor.Posing.Attachment;
using Ktisis.Editor.Posing.Data;
using Ktisis.Editor.Posing.Ik;
using Ktisis.Editor.Transforms.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Entities.Skeleton;

namespace Ktisis.Editor.Posing.Types;

public interface IPosingManager : IDisposable {
	bool IsValid { get; }

	IAttachManager Attachments { get; }

	bool IsEnabled { get; }

	void Initialize();

	void SetEnabled(bool enable);

	IIkController CreateIkController();

	Task ApplyReferencePose(EntityPose pose);

	Task ApplyPoseFile(
		EntityPose pose,
		PoseFile file,
		PoseTransforms transforms = PoseTransforms.Rotation,
		bool selectedBones = false,
		bool anchorGroups = false,
		bool selectedBonesIncludeChildren = false,
		BoneTypeInclusion boneTypeInclusion = BoneTypeInclusion.Both
	);

	Task<PoseFile> SavePoseFile(EntityPose pose);

	ActorEntity? GetActorFromTarget(ITransformTarget? target);

	Task ApplyPoseFlip(ActorEntity target);
}
