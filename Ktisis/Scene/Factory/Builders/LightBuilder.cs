// Decompiled with JetBrains decompiler
// Type: Ktisis.Scene.Factory.Builders.LightBuilder
// Assembly: KtisisPyon, Version=0.3.9.5, Culture=neutral, PublicKeyToken=null
// MVID: 678E6480-A117-4750-B4EA-EC6ECE388B70
// Assembly location: C:\Users\WDAGUtilityAccount\Downloads\KtisisPyon\KtisisPyon.dll

#nullable enable
using System;

using Ktisis.Scene.Entities.World;
using Ktisis.Scene.Factory.Types;
using Ktisis.Scene.Types;
using Ktisis.Structs.Lights;

namespace Ktisis.Scene.Factory.Builders;

public sealed class LightBuilder :
	EntityBuilder<LightEntity, ILightBuilder>,
	ILightBuilder,
	IEntityBuilder<LightEntity, ILightBuilder>,
	IEntityBuilderBase<LightEntity, ILightBuilder> {
	private IntPtr Address = IntPtr.Zero;

	public LightBuilder(ISceneManager scene)
		: base(scene) {
		this.Name = "Light";
	}

	virtual LightBuilder EntityBuilderBase<LightEntity, ILightBuilder>.Builder {
		[PreserveBaseOverrides] get => this;
	}

	public ILightBuilder SetAddress(IntPtr address) {
		this.Address = address;
		return this;
	}

	public unsafe ILightBuilder SetAddress(SceneLight* pointer) {
		this.Address = (IntPtr)pointer;
		return this;
	}

	protected override LightEntity Build() {
		if (this.Address == IntPtr.Zero)
			throw new Exception("Attempted to create light from null pointer.");
		var lightEntity = new LightEntity(this.Scene);
		lightEntity.Name = this.Name;
		lightEntity.Address = this.Address;
		return lightEntity;
	}
}
