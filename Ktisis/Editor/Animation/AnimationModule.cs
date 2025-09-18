// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.AnimationModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;
using System;
using System.Numerics;

#nullable enable
namespace Ktisis.Editor.Animation;

public class AnimationModule(IHookMediator hook) : HookModule(hook)
{
  [Signature("83 FA 0E 73 22", DetourName = "SetTimelineSpeedDetour")]
  private Hook<AnimationModule.SetTimelineSpeedDelegate>? SetTimelineSpeedHook;
  [Signature("E8 ?? ?? ?? ?? 84 DB 74 3A", DetourName = "UpdatePosDetour")]
  private Hook<AnimationModule.UpdatePosDelegate> UpdatePosHook;
  [Signature("E8 ?? ?? ?? ?? 88 45 68")]
  public AnimationModule.PlayEmoteDelegate PlayEmote;
  [Signature("E8 ?? ?? ?? ?? F6 46 10 01")]
  private AnimationModule.SetEmoteModeDelegate SetEmoteMode;
  [Signature("E8 ?? ?? ?? ?? 0F BE 53 20")]
  private AnimationModule.EmoteControllerUpdateDrawOffsetDelegate EmoteControllerUpdateDrawOffset;
  [Signature("E8 ?? ?? ?? ?? 80 7B 17 01")]
  private AnimationModule.CancelTimelineDelegate CancelTimeline;
  [Signature("E8 ?? ?? ?? ?? 4C 8B BC 24 ?? ?? ?? ?? 4C 8D 9C 24 ?? ?? ?? ?? 49 8B 5B 40")]
  public AnimationModule.SetTimelineIdDelegate SetTimelineId;

  public bool SpeedControlEnabled { get; set; }

  public bool PositionLockEnabled { get; set; }

  public unsafe void SetTimelineSpeed(AnimationTimeline* timeline, uint slot, float speed)
  {
    Hook<AnimationModule.SetTimelineSpeedDelegate> timelineSpeedHook = this.SetTimelineSpeedHook;
    if (timelineSpeedHook == null)
      return;
    timelineSpeedHook.Original(timeline, slot, speed);
  }

  private unsafe void SetTimelineSpeedDetour(AnimationTimeline* timeline, uint slot, float speed)
  {
    if (this.SpeedControlEnabled && ((CharacterEx*) ((IntPtr) timeline - new IntPtr(2608)))->IsGPose)
      return;
    this.SetTimelineSpeedHook.Original(timeline, slot, speed);
  }

  private unsafe void UpdatePosDetour(CharacterEx* chara)
  {
    if (this.PositionLockEnabled && chara->IsGPose)
      return;
    this.UpdatePosHook.Original(chara);
  }

  public unsafe void SetPose(ActorEntity actor, PoseModeEnum poseMode, byte pose)
  {
    EmoteModeEnum emoteModeEnum;
    switch (poseMode)
    {
      case PoseModeEnum.Battle:
        emoteModeEnum = EmoteModeEnum.Normal;
        break;
      case PoseModeEnum.SitChair:
        emoteModeEnum = EmoteModeEnum.SitChair;
        break;
      case PoseModeEnum.SitGround:
        emoteModeEnum = EmoteModeEnum.SitGround;
        break;
      case PoseModeEnum.Sleeping:
        emoteModeEnum = EmoteModeEnum.Sleeping;
        break;
      default:
        emoteModeEnum = EmoteModeEnum.Normal;
        break;
    }
    EmoteModeEnum mode = emoteModeEnum;
    CharacterEx* character = actor.IsValid ? (CharacterEx*) actor.Character : (CharacterEx*) null;
    if ((IntPtr) character == IntPtr.Zero)
      return;
    int num1 = mode == EmoteModeEnum.SitChair ? 1 : 0;
    Vector3 vector3_1;
    Vector3 vector3_2;
    if (num1 != 0)
    {
      vector3_1 = character->DrawObjectOffset;
      vector3_2 = character->CameraOffsetSmooth;
    }
    else
    {
      vector3_1 = Vector3.Zero;
      vector3_2 = Vector3.Zero;
    }
    byte pose1 = character->EmoteController.Pose;
    if (pose == byte.MaxValue)
      pose = pose1 != byte.MaxValue ? pose1 : (byte) 0;
    IntPtr num2 = this.CancelTimeline(&character->Animation, IntPtr.Zero, IntPtr.Zero);
    int num3 = this.SetEmoteMode(&character->EmoteController, mode) ? 1 : 0;
    character->EmoteController.Mode = poseMode;
    character->EmoteController.Pose = pose;
    if (num1 == 0)
      return;
    character->EmoteController.IsDrawObjectOffset = false;
    IntPtr num4 = this.EmoteControllerUpdateDrawOffset(&character->EmoteController);
    character->DrawObjectOffset = vector3_1;
    character->CameraOffsetSmooth = vector3_2;
  }

  private unsafe delegate void SetTimelineSpeedDelegate(
    AnimationTimeline* timeline,
    uint slot,
    float speed);

  private unsafe delegate void UpdatePosDelegate(CharacterEx* chara);

  public unsafe delegate bool PlayEmoteDelegate(
    EmoteController* controller,
    IntPtr id,
    IntPtr option,
    IntPtr chair);

  private unsafe delegate bool SetEmoteModeDelegate(EmoteController* a1, EmoteModeEnum mode);

  private unsafe delegate IntPtr EmoteControllerUpdateDrawOffsetDelegate(EmoteController* a1);

  private unsafe delegate IntPtr CancelTimelineDelegate(
    AnimationContainer* a1,
    IntPtr a2,
    IntPtr a3);

  public unsafe delegate bool SetTimelineIdDelegate(AnimationTimeline* a1, ushort a2, IntPtr a3);
}
