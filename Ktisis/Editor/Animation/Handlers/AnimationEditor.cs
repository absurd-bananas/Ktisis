// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Handlers.AnimationEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;
using System.Collections.Generic;

using Ktisis.Editor.Animation.Game;
using Ktisis.Editor.Animation.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;

namespace Ktisis.Editor.Animation.Handlers;

public class AnimationEditor(IAnimationManager mgr, ActorEntity actor) : IAnimationEditor {
	private const ushort IdlePose = 3;
	private const ushort DrawWeaponId = 1;
	private const ushort SheatheWeaponId = 2;
	private const uint BattleIdle = 34;
	private const uint BattlePose = 93;
	private readonly static List<uint> IdlePoses;
	private readonly static Dictionary<PoseModeEnum, int> StancePoses;

	static AnimationEditor() {
		var capacity = 7;
		var uintList = new List<uint>(capacity);
		CollectionsMarshal.SetCount<uint>(uintList, capacity);
		Span<uint> span = CollectionsMarshal.AsSpan<uint>(uintList);
		var num1 = 0;
		span[num1] = 0U;
		var num2 = num1 + 1;
		span[num2] = 91U;
		var num3 = num2 + 1;
		span[num3] = 92U;
		var num4 = num3 + 1;
		span[num4] = 107U;
		var num5 = num4 + 1;
		span[num5] = 108U;
		var num6 = num5 + 1;
		span[num6] = 218U;
		var num7 = num6 + 1;
		span[num7] = 219U;
		IdlePoses = uintList;
		StancePoses = new Dictionary<PoseModeEnum, int> {
			{
				PoseModeEnum.SitGround,
				4
			}, {
				PoseModeEnum.SitChair,
				5
			}, {
				PoseModeEnum.Sleeping,
				3
			}
		};
	}

	public bool SpeedControlEnabled {
		get => mgr.SpeedControlEnabled;
		set => mgr.SpeedControlEnabled = value;
	}

	public bool PositionLockEnabled {
		get => mgr.PositionLockEnabled;
		set => mgr.PositionLockEnabled = value;
	}

	public unsafe bool TryGetModeAndPose(out PoseModeEnum mode, out int pose) {
		var character = actor.IsValid ? (CharacterEx*)actor.Character : null;
		if ((IntPtr)character == IntPtr.Zero) {
			mode = PoseModeEnum.None;
			pose = 0;
			return false;
		}
		PoseModeEnum poseModeEnum;
		switch (character->EmoteMode) {
			case EmoteModeEnum.SitGround:
				poseModeEnum = PoseModeEnum.SitGround;
				break;
			case EmoteModeEnum.SitChair:
				poseModeEnum = PoseModeEnum.SitChair;
				break;
			case EmoteModeEnum.Sleeping:
				poseModeEnum = PoseModeEnum.Sleeping;
				break;
			default:
				var mode1 = character->EmoteController.Mode;
				poseModeEnum = mode1 != PoseModeEnum.None ? mode1 : PoseModeEnum.Idle;
				break;
		}
		mode = poseModeEnum;
		pose = character->EmoteController.Pose;
		return true;
	}

	public int GetPoseCount(PoseModeEnum poseMode) => poseMode == PoseModeEnum.Idle || poseMode == PoseModeEnum.None
		? this.IsWeaponDrawn ? 2 : IdlePoses.Count
		: CollectionExtensions.GetValueOrDefault<PoseModeEnum, int>((IReadOnlyDictionary<PoseModeEnum, int>)StancePoses, poseMode, 1);

	public void SetPose(PoseModeEnum poseMode, byte pose = 255 /*0xFF*/) {
		mgr.SetPose(actor, poseMode, pose);
		if (poseMode != PoseModeEnum.Idle && poseMode != PoseModeEnum.None)
			return;
		if (pose == 0)
			mgr.PlayTimeline(actor, this.IsWeaponDrawn ? 34U : 3U);
		else if (this.IsWeaponDrawn) {
			mgr.PlayEmote(actor, 93U);
		} else {
			if (pose >= IdlePoses.Count)
				return;
			var idlePose = IdlePoses[pose];
			if (idlePose == 0U)
				return;
			mgr.PlayEmote(actor, idlePose);
		}
	}

	public void PlayAnimation(GameAnimation animation, bool playStart = true) {
		if (animation is EmoteAnimation emoteAnimation && emoteAnimation.Index == 0 && playStart && mgr.PlayEmote(actor, emoteAnimation.EmoteId))
			return;
		mgr.PlayTimeline(actor, animation.TimelineId);
	}

	public void PlayTimeline(uint id) => mgr.PlayTimeline(actor, id);

	public unsafe AnimationTimeline GetTimeline() {
		var chara = this.GetChara();
		return (IntPtr)chara == IntPtr.Zero ? new AnimationTimeline() : chara->Animation.Timeline;
	}

	public unsafe void SetForceTimeline(ushort id) {
		var chara = this.GetChara();
		if ((IntPtr)chara == IntPtr.Zero)
			return;
		chara->Animation.Timeline.ActionTimelineId = id;
	}

	public void SetTimelineSpeed(uint slot, float speed) => mgr.SetTimelineSpeed(actor, slot, speed);

	public unsafe bool IsWeaponDrawn {
		get {
			var chara = this.GetChara();
			return (IntPtr)chara != IntPtr.Zero && IsWeaponDrawnFor(chara);
		}
	}

	public unsafe void ToggleWeapon() {
		var chara = this.GetChara();
		if ((IntPtr)chara == IntPtr.Zero)
			return;
		this.PlayTimeline(IsWeaponDrawnFor(chara) ? 2U : 1U);
		chara->CombatFlags ^= CombatFlags.WeaponDrawn;
	}

	private unsafe CharacterEx* GetChara() => !actor.IsValid ? null : (CharacterEx*)actor.Character;

	private unsafe static bool IsWeaponDrawnFor(CharacterEx* chara) => chara->CombatFlags.HasFlag(CombatFlags.WeaponDrawn);
}
