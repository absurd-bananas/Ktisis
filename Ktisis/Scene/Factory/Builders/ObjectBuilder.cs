// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.ObjectBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Scene.Entities.Character;
using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using Ktisis.Services.Data;

namespace Ktisis.Scene.Factory.Builders;

public sealed class ObjectBuilder :
	EntityBuilder<WorldEntity, IObjectBuilder>,
	IObjectBuilder,
	IEntityBuilder<WorldEntity, IObjectBuilder>,
	IEntityBuilderBase<WorldEntity, IObjectBuilder> {
	private readonly INameResolver _naming;
	private readonly IPoseBuilder _pose;
	private IntPtr Address = IntPtr.Zero;

	public ObjectBuilder(ISceneManager scene, IPoseBuilder pose, INameResolver naming)
		: base(scene) {
		this._pose = pose;
		this._naming = naming;
	}

	protected override IObjectBuilder Builder => this;

	public IObjectBuilder SetAddress(IntPtr address) {
		this.Address = address;
		return this;
	}

	public unsafe IObjectBuilder SetAddress(object* pointer) {
		this.Address = (IntPtr)pointer;
		return this;
	}

	private ObjectType GetObjectType() => ((object)this.Address).GetObjectType();

	private CharacterBase.ModelType GetModelType() => ((CharacterBase)this.Address).GetModelType();

	private void SetFallbackName(string name) {
		if (!StringExtensions.IsNullOrEmpty(this.Name))
			return;
		this.Name = name;
	}

	protected override WorldEntity Build() {
		if (this.Address == IntPtr.Zero)
			throw new Exception("Attempted to build object from null pointer.");
		ObjectType objectType = this.GetObjectType();
		var worldEntity = objectType == 3 ? this.BuildCharaBase() : objectType != 5 ? this.BuildDefault() : new LightEntity(this.Scene);
		this.SetFallbackName(objectType.ToString());
		worldEntity.Name = this.Name;
		worldEntity.Address = this.Address;
		return worldEntity;
	}

	private WorldEntity BuildCharaBase() {
		CharacterBase.ModelType modelType = this.GetModelType();
		var charaEntity = modelType != 4 ? new CharaEntity(this.Scene, this._pose) : this.BuildWeapon();
		this.SetFallbackName(modelType.ToString());
		return charaEntity;
	}

	private WeaponEntity BuildWeapon() {
		var weaponEntity = new WeaponEntity(this.Scene, this._pose);
		if (!StringExtensions.IsNullOrEmpty(this.Name))
			return weaponEntity;
		var weaponName = this.GetWeaponName();
		if (weaponName == null)
			return weaponEntity;
		this.Name = weaponName;
		return weaponEntity;
	}

	private unsafe string? GetWeaponName() {
		Weapon* address = (Weapon*)this.Address;
		return this._naming.GetWeaponName(address->ModelSetId, address->SecondaryId, address->Variant);
	}

	private WorldEntity BuildDefault() => new WorldEntity(this.Scene);
}
