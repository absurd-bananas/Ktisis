// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.ObjectBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using Ktisis.Scene.Entities.Character;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using Ktisis.Services.Data;
using System;

#nullable enable
namespace Ktisis.Scene.Factory.Builders;

public sealed class ObjectBuilder : 
  EntityBuilder<WorldEntity, IObjectBuilder>,
  IObjectBuilder,
  IEntityBuilder<WorldEntity, IObjectBuilder>,
  IEntityBuilderBase<WorldEntity, IObjectBuilder>
{
  private readonly IPoseBuilder _pose;
  private readonly INameResolver _naming;
  private IntPtr Address = IntPtr.Zero;

  public ObjectBuilder(ISceneManager scene, IPoseBuilder pose, INameResolver naming)
    : base(scene)
  {
    this._pose = pose;
    this._naming = naming;
  }

  protected override IObjectBuilder Builder => (IObjectBuilder) this;

  public IObjectBuilder SetAddress(IntPtr address)
  {
    this.Address = address;
    return (IObjectBuilder) this;
  }

  public unsafe IObjectBuilder SetAddress(Object* pointer)
  {
    this.Address = (IntPtr) pointer;
    return (IObjectBuilder) this;
  }

  private ObjectType GetObjectType() => ((Object) this.Address).GetObjectType();

  private CharacterBase.ModelType GetModelType() => ((CharacterBase) this.Address).GetModelType();

  private void SetFallbackName(string name)
  {
    if (!StringExtensions.IsNullOrEmpty(this.Name))
      return;
    this.Name = name;
  }

  protected override WorldEntity Build()
  {
    if (this.Address == IntPtr.Zero)
      throw new Exception("Attempted to build object from null pointer.");
    ObjectType objectType = this.GetObjectType();
    WorldEntity worldEntity = objectType == 3 ? this.BuildCharaBase() : (objectType != 5 ? this.BuildDefault() : (WorldEntity) new LightEntity(this.Scene));
    this.SetFallbackName(objectType.ToString());
    worldEntity.Name = this.Name;
    worldEntity.Address = this.Address;
    return worldEntity;
  }

  private WorldEntity BuildCharaBase()
  {
    CharacterBase.ModelType modelType = this.GetModelType();
    CharaEntity charaEntity = modelType != 4 ? new CharaEntity(this.Scene, this._pose) : (CharaEntity) this.BuildWeapon();
    this.SetFallbackName(modelType.ToString());
    return (WorldEntity) charaEntity;
  }

  private WeaponEntity BuildWeapon()
  {
    WeaponEntity weaponEntity = new WeaponEntity(this.Scene, this._pose);
    if (!StringExtensions.IsNullOrEmpty(this.Name))
      return weaponEntity;
    string weaponName = this.GetWeaponName();
    if (weaponName == null)
      return weaponEntity;
    this.Name = weaponName;
    return weaponEntity;
  }

  private unsafe string? GetWeaponName()
  {
    Weapon* address = (Weapon*) this.Address;
    return this._naming.GetWeaponName(address->ModelSetId, address->SecondaryId, address->Variant);
  }

  private WorldEntity BuildDefault() => new WorldEntity(this.Scene);
}
