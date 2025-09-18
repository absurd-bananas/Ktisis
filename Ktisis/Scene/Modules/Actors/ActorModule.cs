// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Modules.Actors.ActorModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Common.Extensions;
using Ktisis.Common.Utility;
using Ktisis.Editor.Characters.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Interop.Ipc;
using Ktisis.Scene.Entities;
using Ktisis.Scene.Entities.Game;
using Ktisis.Scene.Types;
using Ktisis.Services.Game;
using Ktisis.Structs.GPose;
using System;
using System.Linq;
using System.Threading.Tasks;

#nullable enable
namespace Ktisis.Scene.Modules.Actors;

public class ActorModule : SceneModule
{
  private readonly ActorService _actors;
  private readonly IClientState _clientState;
  private readonly IFramework _framework;
  private readonly GroupPoseModule _gpose;
  private readonly ActorSpawner _spawner;
  [Signature("40 56 57 48 83 EC 38 48 89 5C 24 ??", DetourName = "AddCharacterDetour")]
  private Hook<ActorModule.AddCharacterDelegate>? AddCharacterHook;
  [Signature("45 33 D2 4C 8D 81 ?? ?? ?? ?? 41 8B C2 4C 8B C9 49 3B 10")]
  private ActorModule.RemoveCharacterDelegate _removeCharacter;

  public ActorModule(
    IHookMediator hook,
    ISceneManager scene,
    ActorService actors,
    IClientState clientState,
    IFramework framework,
    GroupPoseModule gpose)
    : base(hook, scene)
  {
    this._actors = actors;
    this._clientState = clientState;
    this._framework = framework;
    this._gpose = gpose;
    this._spawner = hook.Create<ActorSpawner>();
  }

  public override void Setup()
  {
    foreach (IGameObject gposeActor in this._actors.GetGPoseActors())
      this.AddActor(gposeActor, false);
    this.Subscribe();
    this.EnableAll();
    this._spawner.TryInitialize();
  }

  private void Subscribe()
  {
    this.Scene.Context.Characters.OnDisableDraw += new DisableDrawHandler(this.OnDisableDraw);
  }

  private unsafe void OnDisableDraw(IGameObject gameObject, DrawObject* drawObject)
  {
    if (!this.IsInit || !this.Scene.IsValid)
      return;
    ActorEntity entityForActor = this.Scene.GetEntityForActor(gameObject);
    if (entityForActor == null)
      return;
    entityForActor.Address = IntPtr.Zero;
    Ktisis.Ktisis.Log.Debug($"Invalidated object address for entity '{entityForActor.Name}' ({gameObject.ObjectIndex})", Array.Empty<object>());
  }

  public async Task<ActorEntity> Spawn()
  {
    IPlayerCharacter localPlayer = this._clientState.LocalPlayer;
    ActorEntity actorEntity1 = localPlayer != null ? this.AddSpawnedActor(await this._spawner.CreateActor((IGameObject) localPlayer)) : throw new Exception("Local player not found.");
    actorEntity1.Actor.SetName(PlayerNameUtil.CalcActorName(actorEntity1.Actor.ObjectIndex));
    actorEntity1.Actor.SetWorld((ushort) localPlayer.CurrentWorld.RowId);
    this.ReassignParentIndex(actorEntity1.Actor);
    ActorEntity actorEntity2 = actorEntity1;
    localPlayer = (IPlayerCharacter) null;
    return actorEntity2;
  }

  public async Task<ActorEntity> AddFromOverworld(IGameObject actor)
  {
    ActorEntity actorEntity = this._spawner.IsInit ? this.AddSpawnedActor(await this._spawner.CreateActor(actor)) : throw new Exception("Actor spawner is uninitialized.");
    actorEntity.Actor.SetTargetable(true);
    return actorEntity;
  }

  private ActorEntity AddSpawnedActor(IntPtr address)
  {
    ActorEntity actorEntity = this.AddActor(address, false);
    if (actorEntity == null)
      throw new Exception("Failed to create entity for spawned actor.");
    actorEntity.IsManaged = true;
    return actorEntity;
  }

  public unsafe void Delete(ActorEntity actor)
  {
    if (this._gpose.IsPrimaryActor(actor))
    {
      Ktisis.Ktisis.Log.Warning("Refusing to delete primary actor.", Array.Empty<object>());
    }
    else
    {
      GPoseState* gpose = this._gpose.GetGPoseState();
      if ((IntPtr) gpose == IntPtr.Zero)
        return;
      GameObject* gameObject = (GameObject*) actor.Actor.Address;
      this._framework.RunOnFrameworkThread((Action) (() =>
      {
        ClientObjectManager* clientObjectManagerPtr = ClientObjectManager.Instance();
        ushort indexByObject = (ushort) ((ClientObjectManager) (IntPtr) clientObjectManagerPtr).GetIndexByObject(gameObject);
        IntPtr num = this._removeCharacter(gpose, gameObject);
        if (indexByObject == ushort.MaxValue)
          return;
        ((ClientObjectManager) (IntPtr) clientObjectManagerPtr).DeleteObjectByIndex(indexByObject, (byte) 1);
      }));
      actor.Remove();
    }
  }

  private void ReassignParentIndex(IGameObject gameObject)
  {
    IpcManager ipc = this.Scene.Context.Plugin.Ipc;
    if (ipc.IsPenumbraActive)
      ipc.GetPenumbraIpc().SetAssignedParentIndex(gameObject, (int) gameObject.ObjectIndex);
    if (!ipc.IsCustomizeActive)
      return;
    CustomizeIpcProvider customizeIpc = ipc.GetCustomizeIpc();
    if (!customizeIpc.IsCompatible())
      return;
    customizeIpc.SetCutsceneParentIndex((int) gameObject.ObjectIndex, (int) gameObject.ObjectIndex);
  }

  private ActorEntity? AddActor(IntPtr address, bool addCompanion)
  {
    IGameObject address1 = this._actors.GetAddress(address);
    if (address1 != null && address1.ObjectIndex != (ushort) 200)
      return this.AddActor(address1, addCompanion);
    Ktisis.Ktisis.Log.Warning($"Actor address at 0x{address:X} is invalid.", Array.Empty<object>());
    return (ActorEntity) null;
  }

  private ActorEntity? AddActor(IGameObject actor, bool addCompanion)
  {
    if (!actor.IsValid())
    {
      Ktisis.Ktisis.Log.Warning($"Actor address at 0x{actor.Address:X} is invalid.", Array.Empty<object>());
      return (ActorEntity) null;
    }
    ActorEntity actorEntity = this.Scene.Factory.BuildActor(actor).Add();
    if (!addCompanion)
      return actorEntity;
    this.AddCompanion(actor);
    return actorEntity;
  }

  private unsafe void AddCompanion(IGameObject owner)
  {
    FFXIVClientStructs.FFXIV.Client.Game.Character.Character* address1 = (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*) owner.Address;
    if ((IntPtr) address1 == IntPtr.Zero || (IntPtr) address1->CompanionObject == IntPtr.Zero)
      return;
    IGameObject address2 = this._actors.GetAddress((IntPtr) address1->CompanionObject);
    if (address2 == null || address2.ObjectIndex == (ushort) 0 || !address2.IsValid())
      return;
    this.Scene.Factory.BuildActor(address2).Add();
  }

  public unsafe void RefreshGPoseActors()
  {
    foreach (ActorEntity actorEntity in this.Scene.Children.Where<SceneEntity>((Func<SceneEntity, bool>) (entity => entity is ActorEntity)).Cast<ActorEntity>().ToList<ActorEntity>())
    {
      if (actorEntity.IsValid)
      {
        ActorEntity entityForActor = this.Scene.GetEntityForActor(actorEntity.Actor);
        if (entityForActor != null && (IntPtr) entityForActor.Character == IntPtr.Zero)
          this.Delete(entityForActor);
      }
    }
    foreach (IGameObject gposeActor in this._actors.GetGPoseActors())
    {
      if (this.Scene.GetEntityForActor(gposeActor) == null)
        this.AddActor(gposeActor, false);
    }
  }

  private void AddCharacterDetour(IntPtr gpose, IntPtr address, ulong id, IntPtr a4)
  {
    this.AddCharacterHook.Original(gpose, address, id, a4);
    if (!this.CheckValid())
      return;
    try
    {
      if (id == 3758096384UL /*0xE0000000*/)
        return;
      this.AddActor(address, true);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle character add for 0x{address:X}:\n{ex}", Array.Empty<object>());
    }
  }

  public override void Dispose()
  {
    base.Dispose();
    this._spawner.Dispose();
    GC.SuppressFinalize((object) this);
  }

  private delegate void AddCharacterDelegate(IntPtr a1, IntPtr a2, ulong a3, IntPtr a4);

  private unsafe delegate IntPtr RemoveCharacterDelegate(GPoseState* gpose, GameObject* gameObject);
}
