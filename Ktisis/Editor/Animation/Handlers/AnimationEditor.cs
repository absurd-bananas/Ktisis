// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.Handlers.AnimationEditor
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Ktisis.Editor.Animation.Game;
using Ktisis.Editor.Animation.Types;
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable enable
namespace Ktisis.Editor.Animation.Handlers;

public class AnimationEditor(IAnimationManager mgr, ActorEntity actor) : IAnimationEditor
{
  private static readonly List<uint> IdlePoses;
  private static readonly Dictionary<PoseModeEnum, int> StancePoses;
  private const ushort IdlePose = 3;
  private const ushort DrawWeaponId = 1;
  private const ushort SheatheWeaponId = 2;
  private const uint BattleIdle = 34;
  private const uint BattlePose = 93;

  private unsafe CharacterEx* GetChara()
  {
    return !actor.IsValid ? (CharacterEx*) null : (CharacterEx*) actor.Character;
  }

  public bool SpeedControlEnabled
  {
    get => mgr.SpeedControlEnabled;
    set => mgr.SpeedControlEnabled = value;
  }

  public bool PositionLockEnabled
  {
    get => mgr.PositionLockEnabled;
    set => mgr.PositionLockEnabled = value;
  }

  public unsafe bool TryGetModeAndPose(out PoseModeEnum mode, out int pose)
  {
    CharacterEx* character = actor.IsValid ? (CharacterEx*) actor.Character : (CharacterEx*) null;
    if ((IntPtr) character == IntPtr.Zero)
    {
      mode = PoseModeEnum.None;
      pose = 0;
      return false;
    }
    PoseModeEnum poseModeEnum;
    switch (character->EmoteMode)
    {
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
        PoseModeEnum mode1 = character->EmoteController.Mode;
        poseModeEnum = mode1 != PoseModeEnum.None ? mode1 : PoseModeEnum.Idle;
        break;
    }
    mode = poseModeEnum;
    pose = (int) character->EmoteController.Pose;
    return true;
  }

  public int GetPoseCount(PoseModeEnum poseMode)
  {
    return poseMode == PoseModeEnum.Idle || poseMode == PoseModeEnum.None ? (this.IsWeaponDrawn ? 2 : AnimationEditor.IdlePoses.Count) : CollectionExtensions.GetValueOrDefault<PoseModeEnum, int>((IReadOnlyDictionary<PoseModeEnum, int>) AnimationEditor.StancePoses, poseMode, 1);
  }

  public void SetPose(PoseModeEnum poseMode, byte pose = 255 /*0xFF*/)
  {
    mgr.SetPose(actor, poseMode, pose);
    if (poseMode != PoseModeEnum.Idle && poseMode != PoseModeEnum.None)
      return;
    if (pose == (byte) 0)
      mgr.PlayTimeline(actor, this.IsWeaponDrawn ? 34U : 3U);
    else if (this.IsWeaponDrawn)
    {
      mgr.PlayEmote(actor, 93U);
    }
    else
    {
      if ((int) pose >= AnimationEditor.IdlePoses.Count)
        return;
      uint idlePose = AnimationEditor.IdlePoses[(int) pose];
      if (idlePose == 0U)
        return;
      mgr.PlayEmote(actor, idlePose);
    }
  }

  public void PlayAnimation(GameAnimation animation, bool playStart = true)
  {
    if (animation is EmoteAnimation emoteAnimation && emoteAnimation.Index == 0 && playStart && mgr.PlayEmote(actor, emoteAnimation.EmoteId))
      return;
    mgr.PlayTimeline(actor, animation.TimelineId);
  }

  public void PlayTimeline(uint id) => mgr.PlayTimeline(actor, id);

  public unsafe AnimationTimeline GetTimeline()
  {
    CharacterEx* chara = this.GetChara();
    return (IntPtr) chara == IntPtr.Zero ? new AnimationTimeline() : chara->Animation.Timeline;
  }

  public unsafe void SetForceTimeline(ushort id)
  {
    CharacterEx* chara = this.GetChara();
    if ((IntPtr) chara == IntPtr.Zero)
      return;
    chara->Animation.Timeline.ActionTimelineId = id;
  }

  public void SetTimelineSpeed(uint slot, float speed) => mgr.SetTimelineSpeed(actor, slot, speed);

  public unsafe bool IsWeaponDrawn
  {
    get
    {
      CharacterEx* chara = this.GetChara();
      return (IntPtr) chara != IntPtr.Zero && AnimationEditor.IsWeaponDrawnFor(chara);
    }
  }

  public unsafe void ToggleWeapon()
  {
    CharacterEx* chara = this.GetChara();
    if ((IntPtr) chara == IntPtr.Zero)
      return;
    this.PlayTimeline(AnimationEditor.IsWeaponDrawnFor(chara) ? 2U : 1U);
    chara->CombatFlags ^= CombatFlags.WeaponDrawn;
  }

  private static unsafe bool IsWeaponDrawnFor(CharacterEx* chara)
  {
    return chara->CombatFlags.HasFlag((Enum) CombatFlags.WeaponDrawn);
  }

  static AnimationEditor()
  {
    int capacity = 7;
    List<uint> uintList = new List<uint>(capacity);
    CollectionsMarshal.SetCount<uint>(uintList, capacity);
    Span<uint> span = CollectionsMarshal.AsSpan<uint>(uintList);
    int num1 = 0;
    span[num1] = 0U;
    int num2 = num1 + 1;
    span[num2] = 91U;
    int num3 = num2 + 1;
    span[num3] = 92U;
    int num4 = num3 + 1;
    span[num4] = 107U;
    int num5 = num4 + 1;
    span[num5] = 108U;
    int num6 = num5 + 1;
    span[num6] = 218U;
    int num7 = num6 + 1;
    span[num7] = 219U;
    AnimationEditor.IdlePoses = uintList;
    AnimationEditor.StancePoses = new Dictionary<PoseModeEnum, int>()
    {
      {
        PoseModeEnum.SitGround,
        4
      },
      {
        PoseModeEnum.SitChair,
        5
      },
      {
        PoseModeEnum.Sleeping,
        3
      }
    };
  }
}
