// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.CharacterManager
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Data.Files;
using Ktisis.Data.Mcdf;
using Ktisis.Editor.Characters.Handlers;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.Editor.Context.Types;
using Ktisis.GameData.Excel.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities.Game;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Editor.Characters;

public class CharacterManager : ICharacterManager, IDisposable
{
  private readonly IEditorContext _context;
  private readonly HookScope _scope;
  private readonly IFramework _framework;
  private readonly Dictionary<ushort, Transform> _savedTransforms = new Dictionary<ushort, Transform>();

  public bool IsValid => this._context.IsValid;

  public McdfManager Mcdf { get; }

  public event DisableDrawHandler? OnDisableDraw;

  public CharacterManager(
    IEditorContext context,
    HookScope scope,
    IFramework framework,
    McdfManager mcdf)
  {
    this._context = context;
    this._scope = scope;
    this._framework = framework;
    this.Mcdf = mcdf;
  }

  private CharacterModule? Module { get; set; }

  public void Initialize()
  {
    Ktisis.Ktisis.Log.Verbose("Initializing character manager...", Array.Empty<object>());
    try
    {
      this.Module = this._scope.Create<CharacterModule>((object) this);
      this.Subscribe();
      this.Module.Initialize();
      this.Module.EnableAll();
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to initialize character module:\n{ex}", Array.Empty<object>());
    }
  }

  private void Subscribe()
  {
    this.Module.OnDisableDraw += new DisableDrawHandler(this.HandleDisableDraw);
    this.Module.OnEnableDraw += new EnableDrawHandler(this.HandleEnableDraw);
  }

  private unsafe void HandleDisableDraw(IGameObject gameObj, DrawObject* drawObj)
  {
    DisableDrawHandler onDisableDraw = this.OnDisableDraw;
    if (onDisableDraw != null)
      onDisableDraw(gameObj, drawObj);
    if ((IntPtr) drawObj == IntPtr.Zero)
      return;
    this._savedTransforms[gameObj.ObjectIndex] = *(Transform*) &drawObj->Position;
  }

  private unsafe void HandleEnableDraw(GameObject* gameObj)
  {
    ushort objectIndex = gameObj->ObjectIndex;
    Transform transform;
    if (!this._savedTransforms.TryGetValue(objectIndex, out transform))
      return;
    if ((IntPtr) gameObj->DrawObject != IntPtr.Zero)
      *(Transform*) &gameObj->DrawObject->Position = transform;
    this._savedTransforms.Remove(objectIndex);
  }

  public ICustomizeEditor GetCustomizeEditor(ActorEntity actor)
  {
    return (ICustomizeEditor) new CustomizeEditor(actor);
  }

  public IEquipmentEditor GetEquipmentEditor(ActorEntity actor)
  {
    return (IEquipmentEditor) new EquipmentEditor(actor);
  }

  private EntityCharaConverter BuildEntityConverter(ActorEntity actor)
  {
    ICustomizeEditor customizeEditor = this.GetCustomizeEditor(actor);
    IEquipmentEditor equipmentEditor = this.GetEquipmentEditor(actor);
    return new EntityCharaConverter(actor, customizeEditor, equipmentEditor);
  }

  public bool TryGetStateForActor(
    IGameObject actor,
    out ActorEntity entity,
    out AppearanceState state)
  {
    ActorEntity entityForActor = this._context.Scene.GetEntityForActor(actor);
    entity = entityForActor;
    state = entityForActor?.Appearance;
    return entityForActor != null;
  }

  public void ApplyStateToGameObject(ActorEntity entity)
  {
    this.GetCustomizeEditor(entity).ApplyStateToGameObject();
    this.GetEquipmentEditor(entity).ApplyStateToGameObject();
  }

  public Task ApplyCharaFile(ActorEntity actor, CharaFile file, SaveModes modes = SaveModes.All, bool gameState = false)
  {
    EntityCharaConverter loader = this.BuildEntityConverter(actor);
    return this._framework.RunOnFrameworkThread((Action) (() =>
    {
      loader.Apply(file, modes);
      if (!gameState)
        return;
      this.ApplyStateToGameObject(actor);
    }));
  }

  public Task<CharaFile> SaveCharaFile(ActorEntity actor)
  {
    return this._framework.RunOnFrameworkThread<CharaFile>((Func<CharaFile>) (() => this.BuildEntityConverter(actor).Save()));
  }

  public Task ApplyNpc(ActorEntity actor, INpcBase npc, SaveModes modes = SaveModes.All, bool gameState = false)
  {
    EntityCharaConverter loader = this.BuildEntityConverter(actor);
    return this._framework.RunOnFrameworkThread((Action) (() =>
    {
      loader.Apply(npc, modes);
      if (!gameState)
        return;
      this.ApplyStateToGameObject(actor);
    }));
  }

  public void Dispose()
  {
    this.Module?.Dispose();
    this.Module = (CharacterModule) null;
    GC.SuppressFinalize((object) this);
  }
}
