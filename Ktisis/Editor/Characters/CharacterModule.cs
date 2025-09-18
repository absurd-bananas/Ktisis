// Decompiled with JetBrains decompiler
// Type: Ktisis.Editor.Characters.CharacterModule
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Editor.Characters.State;
using Ktisis.Editor.Characters.Types;
using Ktisis.Interop.Hooking;
using Ktisis.Scene.Entities.Game;
using Ktisis.Services.Data;
using Ktisis.Services.Game;
using Ktisis.Structs.Actors;
using Ktisis.Structs.Characters;
using System;

#nullable enable
namespace Ktisis.Editor.Characters;

public class CharacterModule : HookModule
{
  private readonly ICharacterManager Manager;
  private readonly ActorService _actors;
  private readonly CustomizeService _discovery;
  private unsafe GameObject* _prepareCharaFor;
  [Signature("40 53 48 83 EC 20 80 B9 ?? ?? ?? ?? ?? 48 8B D9 7D 6E", DetourName = "DisableDrawDetour")]
  private Hook<CharacterModule.DisableDrawDelegate> DisableDrawHook;
  [Signature("E8 ?? ?? ?? ?? 48 8B 8B ?? ?? ?? ?? 48 85 C9 74 33 45 33 C0", DetourName = "EnableDrawDetour")]
  private Hook<CharacterModule.EnableDrawDelegate> EnableDrawHook;
  [Signature("E8 ?? ?? ?? ?? 48 8B 4F 08 48 8B D0 4C 8B 01", DetourName = "CreateCharacterDetour")]
  private Hook<CharacterModule.CreateCharacterDelegate> CreateCharacterHook;

  private bool IsValid => this.Manager.IsValid;

  public event DisableDrawHandler? OnDisableDraw;

  public event EnableDrawHandler? OnEnableDraw;

  public CharacterModule(
    IHookMediator hook,
    ICharacterManager manager,
    ActorService actors,
    CustomizeService discovery)
    : base(hook)
  {
    this.Manager = manager;
    this._actors = actors;
    this._discovery = discovery;
  }

  private unsafe IntPtr DisableDrawDetour(GameObject* chara)
  {
    try
    {
      if ((IntPtr) chara->DrawObject != IntPtr.Zero)
        this.HandleDisableDraw(chara);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle disable draw:\n{ex}", Array.Empty<object>());
    }
    return this.DisableDrawHook.Original(chara);
  }

  private unsafe void HandleDisableDraw(GameObject* chara)
  {
    IGameObject address = this._actors.GetAddress((IntPtr) chara);
    if (address == null)
      return;
    DisableDrawHandler onDisableDraw = this.OnDisableDraw;
    if (onDisableDraw == null)
      return;
    onDisableDraw(address, chara->DrawObject);
  }

  private unsafe IntPtr EnableDrawDetour(GameObject* gameObject)
  {
    if (!this.IsValid || (gameObject->TargetableStatus & 128 /*0x80*/) > 0 & (gameObject->RenderFlags & 33554432 /*0x02000000*/) == 0)
      return this.EnableDrawHook.Original(gameObject);
    IntPtr num = IntPtr.Zero;
    try
    {
      this._prepareCharaFor = gameObject;
      num = this.EnableDrawHook.Original(gameObject);
      EnableDrawHandler onEnableDraw = this.OnEnableDraw;
      if (onEnableDraw != null)
        onEnableDraw(gameObject);
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failed to handle character update:\n{ex}", Array.Empty<object>());
    }
    finally
    {
      this._prepareCharaFor = (GameObject*) null;
    }
    return num;
  }

  private unsafe CharacterBase* CreateCharacterDetour(
    uint model,
    CustomizeContainer* customize,
    EquipmentContainer* equip,
    byte unk)
  {
    try
    {
      if ((IntPtr) customize != IntPtr.Zero)
      {
        if ((IntPtr) equip != IntPtr.Zero)
          this.PreHandleCreate(ref model, customize, equip);
      }
    }
    catch (Exception ex)
    {
      Ktisis.Ktisis.Log.Error($"Failure on PreHandleCreate:\n{ex}", Array.Empty<object>());
    }
    return this.CreateCharacterHook.Original(model, customize, equip, unk);
  }

  private unsafe void PreHandleCreate(
    ref uint model,
    CustomizeContainer* customize,
    EquipmentContainer* equip)
  {
    if (!this.IsValid || (IntPtr) this._prepareCharaFor == IntPtr.Zero)
      return;
    IGameObject address = this._actors.GetAddress((IntPtr) this._prepareCharaFor);
    ActorEntity entity;
    AppearanceState state;
    if (address == null || !this.Manager.TryGetStateForActor(address, out entity, out state))
      return;
    uint? modelId = state.ModelId;
    if (modelId.HasValue)
    {
      ref uint local = ref model;
      modelId = state.ModelId;
      int num = (int) modelId.Value;
      local = (uint) num;
    }
    for (int index1 = 0; index1 < 26; ++index1)
    {
      CustomizeIndex index2 = (CustomizeIndex) index1;
      if (state.Customize.IsSet(index2))
        customize->Bytes[index1] = state.Customize[index2];
    }
    CharacterEx* prepareCharaFor = (CharacterEx*) this._prepareCharaFor;
    if (prepareCharaFor->Mode == (byte) 3 && prepareCharaFor->EmoteMode == EmoteModeEnum.Normal)
      prepareCharaFor->Mode = (byte) 1;
    if (state.Customize.IsSet((CustomizeIndex) 4) || state.Customize.IsSet((CustomizeIndex) 5))
    {
      ushort dataId = this._discovery.CalcDataIdFor(customize->Tribe, customize->Gender);
      bool flag = this._discovery.IsFaceIdValidFor(dataId, (int) customize->FaceType);
      Ktisis.Ktisis.Log.Debug($"Face {customize->FaceType} for {dataId} is valid? {flag}", Array.Empty<object>());
      if (!flag)
      {
        byte faceType = customize->FaceType;
        byte num = customize->Tribe != Tribe.Highlander || faceType >= (byte) 101 ? this._discovery.FindBestFaceTypeFor(dataId, customize->FaceType) : (byte) ((uint) faceType + 100U);
        Ktisis.Ktisis.Log.Debug($"\tSetting {num} as next best face type", Array.Empty<object>());
        state.Customize.SetIfActive((CustomizeIndex) 5, num);
        customize->FaceType = num;
      }
    }
    for (uint index3 = 0; index3 < 10U; ++index3)
    {
      EquipIndex index4 = (EquipIndex) index3;
      if (index4 == EquipIndex.Head && state.HatVisible == EquipmentToggle.Off)
        *equip->GetData(index3) = new EquipmentModelId();
      else if (state.Equipment.IsSet(index4))
        *equip->GetData(index3) = state.Equipment[index4];
    }
    this.Manager.GetEquipmentEditor(entity).ApplyStateFlags();
  }

  private unsafe delegate IntPtr DisableDrawDelegate(GameObject* chara);

  private unsafe delegate IntPtr EnableDrawDelegate(GameObject* gameObject);

  private unsafe delegate CharacterBase* CreateCharacterDelegate(
    uint model,
    CustomizeContainer* customize,
    EquipmentContainer* equip,
    byte unk);
}
