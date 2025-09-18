// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Entities.Game.ActorEntity
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Common.Extensions;
using Ktisis.Editor.Characters.State;
using Ktisis.Scene.Decor;
using Ktisis.Scene.Entities.Character;
using Ktisis.Scene.Factory.Builders;
using Ktisis.Scene.Modules.Actors;
using Ktisis.Scene.Types;
using Ktisis.Structs.Characters;
using System;

#nullable enable
namespace Ktisis.Scene.Entities.Game;

public class ActorEntity : CharaEntity, IDeletable
{
  public readonly IGameObject Actor;
  public bool IsObjectVisible = true;

  public bool IsManaged { get; set; }

  public override bool IsValid => base.IsValid && this.Actor.IsValid();

  public ActorEntity(ISceneManager scene, IPoseBuilder pose, IGameObject actor)
    : base(scene, pose)
  {
    this.Type = EntityType.Actor;
    this.Actor = actor;
  }

  public override void Update()
  {
    if (!this.IsObjectValid)
      return;
    this.UpdateChara();
    base.Update();
  }

  private unsafe void UpdateChara()
  {
    Ktisis.Structs.Characters.CharacterBaseEx* characterBaseEx = this.CharacterBaseEx;
    Ktisis.Structs.Characters.CharacterBaseEx* characterBaseExPtr = characterBaseEx;
    if (this.Address != (IntPtr) characterBaseExPtr)
      this.Address = (IntPtr) characterBaseExPtr;
    if ((IntPtr) characterBaseEx == IntPtr.Zero)
      return;
    WetnessState? wetness = this.Appearance.Wetness;
    if (!wetness.HasValue)
      return;
    WetnessState valueOrDefault = wetness.GetValueOrDefault();
    characterBaseEx->Wetness = valueOrDefault;
  }

  public AppearanceState Appearance { get; } = new AppearanceState();

  private unsafe CustomizeData* GetCustomize()
  {
    Human* human = this.GetHuman();
    if ((IntPtr) human != IntPtr.Zero)
      return &human->Customize;
    FFXIVClientStructs.FFXIV.Client.Game.Character.Character* character = this.Character;
    return (IntPtr) character != IntPtr.Zero ? &character->DrawData.CustomizeData : (CustomizeData*) null;
  }

  public unsafe byte GetCustomizeValue(CustomizeIndex index)
  {
    if (this.Appearance.Customize.IsSet(index))
      return this.Appearance.Customize[index];
    Human* human = this.GetHuman();
    return (IntPtr) human == IntPtr.Zero ? (byte) 0 : ((CustomizeData) ref human->Customize)[(int) (byte) index];
  }

  public bool IsViera() => this.GetCustomizeValue((CustomizeIndex) 0) == (byte) 8;

  public bool TryGetEarId(out byte id)
  {
    if (!this.IsViera())
    {
      id = (byte) 0;
      return false;
    }
    id = this.GetCustomizeValue((CustomizeIndex) 22);
    return true;
  }

  public bool TryGetEarIdAsChar(out char id)
  {
    byte id1;
    int num = this.TryGetEarId(out id1) ? 1 : 0;
    id = (char) (96U /*0x60*/ + (uint) id1);
    return num != 0;
  }

  public unsafe GameObject* CsGameObject => (GameObject*) this.Actor.Address;

  public unsafe FFXIVClientStructs.FFXIV.Client.Game.Character.Character* Character
  {
    get
    {
      return (IntPtr) this.CsGameObject == IntPtr.Zero || !((GameObject) (IntPtr) this.CsGameObject).IsCharacter() ? (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*) null : (FFXIVClientStructs.FFXIV.Client.Game.Character.Character*) this.CsGameObject;
    }
  }

  public override unsafe FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Object* GetObject()
  {
    return (IntPtr) this.CsGameObject == IntPtr.Zero ? (FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Object*) null : (FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Object*) this.CsGameObject->DrawObject;
  }

  public override unsafe CharacterBase* GetCharacter()
  {
    if (!this.IsObjectValid)
      return (CharacterBase*) null;
    DrawObject* drawObject = (IntPtr) this.CsGameObject != IntPtr.Zero ? this.CsGameObject->DrawObject : (DrawObject*) null;
    return (IntPtr) drawObject == IntPtr.Zero || ((FFXIVClientStructs.FFXIV.Client.Graphics.Scene.Object) ref drawObject->Object).GetObjectType() != 3 ? (CharacterBase*) null : (CharacterBase*) drawObject;
  }

  public unsafe Human* GetHuman()
  {
    CharacterBase* character = this.GetCharacter();
    return (IntPtr) character != IntPtr.Zero && ((CharacterBase) (IntPtr) character).GetModelType() == 1 ? (Human*) character : (Human*) null;
  }

  public void Redraw() => this.Actor.Redraw();

  public bool Delete()
  {
    this.Scene.GetModule<ActorModule>().Delete(this);
    return false;
  }

  public unsafe void ToggleObjectVisibility()
  {
    if ((IntPtr) this.GetObject() == IntPtr.Zero)
      return;
    ((DrawObject) (IntPtr) this.CsGameObject->DrawObject).IsVisible = !this.IsObjectVisible;
    this.IsObjectVisible = !this.IsObjectVisible;
  }
}
