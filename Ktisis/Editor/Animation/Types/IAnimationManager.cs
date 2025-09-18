// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Types.IAnimationManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;

namespace Ktisis.Editor.Animation.Types;

public interface IAnimationManager {

	bool SpeedControlEnabled { get; set; }

	bool PositionLockEnabled { get; set; }
	void Initialize();

	IAnimationEditor GetAnimationEditor(ActorEntity actor);

	void SetPose(ActorEntity actor, PoseModeEnum poseMode, byte pose = 255 /*0xFF*/);

	bool PlayEmote(ActorEntity actor, uint id);

	bool PlayTimeline(ActorEntity actor, uint id);

	void SetTimelineSpeed(ActorEntity actor, uint slot, float speed);
}
