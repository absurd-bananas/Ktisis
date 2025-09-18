// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Types.IAnimationEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using Ktisis.Editor.Animation.Game;
using Ktisis.Structs.Actors;

namespace Ktisis.Editor.Animation.Types;

public interface IAnimationEditor {
	bool SpeedControlEnabled { get; set; }

	bool PositionLockEnabled { get; set; }

	bool IsWeaponDrawn { get; }

	bool TryGetModeAndPose(out PoseModeEnum mode, out int pose);

	int GetPoseCount(PoseModeEnum poseMode);

	void SetPose(PoseModeEnum poseMode, byte pose = 255 /*0xFF*/);

	void PlayAnimation(GameAnimation animation, bool playStart = true);

	void PlayTimeline(uint id);

	AnimationTimeline GetTimeline();

	void SetForceTimeline(ushort id);

	void SetTimelineSpeed(uint slot, float speed);

	void ToggleWeapon();
}
