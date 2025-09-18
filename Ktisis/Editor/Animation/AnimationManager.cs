// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Animation.AnimationManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game;
using Dalamud.Plugin.Services;
using Ktisis.Editor.Animation.Handlers;
using Ktisis.Editor.Animation.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities.Game;
using Ktisis.Structs.Actors;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;

#nullable enable
namespace Ktisis.Editor.Animation;

public class AnimationManager : IAnimationManager
{
  private readonly IEditorContext _ctx;
  private readonly HookScope _scope;
  private readonly IDataManager _data;
  private readonly IFramework _framework;

  private AnimationModule? Module { get; set; }

  private ExcelSheet<ActionTimeline>? Timelines { get; set; }

  public AnimationManager(
    IEditorContext ctx,
    HookScope scope,
    IDataManager data,
    IFramework framework)
  {
    this._ctx = ctx;
    this._scope = scope;
    this._data = data;
    this._framework = framework;
  }

  public void Initialize()
  {
    Ktisis.Ktisis.Log.Verbose("Initializing character manager...", Array.Empty<object>());
    try
    {
      this.Module = this._scope.Create<AnimationModule>();
      this.Module.Initialize();
      this.Module.EnableAll();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to initialize animation module:\n{ex}", Array.Empty<object>());
    }
    this.Timelines = this._data.GetExcelSheet<ActionTimeline>(new ClientLanguage?(), (string) null);
  }

  public bool SpeedControlEnabled
  {
    get
    {
      AnimationModule module = this.Module;
      return module != null && module.SpeedControlEnabled;
    }
    set
    {
      if (this.Module == null)
        return;
      this.Module.SpeedControlEnabled = value;
    }
  }

  public bool PositionLockEnabled
  {
    get
    {
      AnimationModule module = this.Module;
      return module != null && module.PositionLockEnabled;
    }
    set
    {
      if (this.Module == null)
        return;
      this.Module.PositionLockEnabled = value;
    }
  }

  public IAnimationEditor GetAnimationEditor(ActorEntity actor)
  {
    return (IAnimationEditor) new AnimationEditor((IAnimationManager) this, actor);
  }

  public void SetPose(ActorEntity actor, PoseModeEnum poseMode, byte pose = 255 /*0xFF*/)
  {
    this._framework.RunOnFrameworkThread((Action) (() => this.Module?.SetPose(actor, poseMode, pose)));
  }

  public unsafe bool PlayEmote(ActorEntity actor, uint id)
  {
    CharacterEx* character = (CharacterEx*) actor.Character;
    if ((IntPtr) character == IntPtr.Zero)
      return false;
    character->Animation.Timeline.ActionTimelineId = (ushort) 0;
    character->EmoteController.IsForceDefaultPose = false;
    return this.Module.PlayEmote(&character->EmoteController, (IntPtr) id, IntPtr.Zero, IntPtr.Zero);
  }

  public unsafe bool PlayTimeline(ActorEntity actor, uint id)
  {
    ActionTimeline? row = this.Timelines?.GetRow(id);
    if (!row.HasValue)
      return false;
    CharacterEx* character = actor.IsValid ? (CharacterEx*) actor.Character : (CharacterEx*) null;
    if ((IntPtr) character == IntPtr.Zero)
      return false;
    character->Animation.Timeline.ActionTimelineId = (ushort) 0;
    ActionTimeline actionTimeline = row.Value;
    if (((ActionTimeline) ref actionTimeline).Pause)
    {
      character->Mode = (byte) 3;
      character->EmoteMode = EmoteModeEnum.Normal;
    }
    else if (character->Mode == (byte) 3 && character->EmoteMode == EmoteModeEnum.Normal)
      character->Mode = (byte) 1;
    return this.Module != null && this.Module.SetTimelineId(&character->Animation.Timeline, (ushort) id, IntPtr.Zero);
  }

  public unsafe void SetTimelineSpeed(ActorEntity actor, uint slot, float speed)
  {
    CharacterEx* character = actor.IsValid ? (CharacterEx*) actor.Character : (CharacterEx*) null;
    if ((IntPtr) character == IntPtr.Zero)
      return;
    this.Module?.SetTimelineSpeed(&character->Animation.Timeline, slot, speed);
  }
}
